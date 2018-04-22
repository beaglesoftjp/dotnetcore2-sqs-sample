using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class SqsController : Controller
    {
        
        private readonly ILogger<SqsController> _logger;
        
        private readonly AwsSqs _awsSqs;
        private const string QueueName = "MySampleQueue";

        public SqsController(AwsSqs awsSqs, ILogger<SqsController> logger)
        {
            _awsSqs = awsSqs;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<string> CreateMessage([FromBody] string message)
        {
            var queue = await _awsSqs.GetQueueUrlAsync(QueueName);
            var request = new SendMessageRequest
            {
                QueueUrl = queue.Value,
                MessageBody = message
            };

            var client = _awsSqs.Client;
            var response = await client.SendMessageAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var result = new
                {
                    StatusCode = "正常終了",
                    Message = "SQSへメッセージの登録に成功しました。"
                };
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = new
                {
                    StatusCode = "エラー",
                    Message = "SQSへメッセージの登録に失敗しました。"
                };
                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// 引数のメッセージをバッチ登録します。
        /// </summary>
        [HttpPost("batchcreate")]
        public async Task<string> CreateMessages([FromBody] string[] messages)
        {
            var queueUrl = await _awsSqs.GetQueueUrlAsync(QueueName);
            var sendMessageBatchRequest = new SendMessageBatchRequest
            {
                Entries = messages
                    .Select((message, index) => new SendMessageBatchRequestEntry(index.ToString(), message)).ToList(),
                QueueUrl = queueUrl.Value
            };

            var client = _awsSqs.Client;
            var response = await client.SendMessageBatchAsync(sendMessageBatchRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var result = new
                {
                    StatusCode = "正常終了",
                    Message = "SQSへメッセージの登録に成功しました。",
                    SuccessfulResponse = response.Successful,
                    FailedResponse = response.Failed
                };
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = new
                {
                    StatusCode = "エラー",
                    Message = "SQSへメッセージの登録に失敗しました。"
                };
                return JsonConvert.SerializeObject(result);
            }
        }

        [HttpGet("receive")]
        public async Task<string> ReceiveMessages()
        {
            var queueUrl = await _awsSqs.GetQueueUrlAsync(QueueName);
            var receiveMessageRequest = new ReceiveMessageRequest {QueueUrl = queueUrl.Value};
            var response = await _awsSqs.Client.ReceiveMessageAsync(receiveMessageRequest);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var result = new
                {
                    StatusCode = "正常終了",
                    Message = "SQSへメッセージの受信に成功しました。",
                    QueueMessages = response.Messages,
                };
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = new
                {
                    StatusCode = "エラー",
                    Message = $"SQSへメッセージの受信に失敗しました。[{response.HttpStatusCode}]"
                };
                return JsonConvert.SerializeObject(result);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMessage()
        {
            var queueUrl = await _awsSqs.GetQueueUrlAsync(QueueName);
            var receiveMessageRequest = new ReceiveMessageRequest {QueueUrl = queueUrl.Value};
            var response = await _awsSqs.Client.ReceiveMessageAsync(receiveMessageRequest);
            
            var deleteResults = new List<dynamic>();
            foreach (var message in response.Messages)
            {
                _logger.LogDebug($"messageId:{message.MessageId} / ReceiptHandle:{message.ReceiptHandle}");
                
                var deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl.Value,
                    ReceiptHandle = message.ReceiptHandle
                };

                var deleteResponse = await _awsSqs.Client.DeleteMessageAsync(deleteRequest);
                if (deleteResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    deleteResults.Add(new {MessageId = message.MessageId, Status = "success"});
                }
                else
                {
                    deleteResults.Add(new {MessageId = message.MessageId, Status = "fail"});
                }
            }

            if (deleteResults.All(result => result.Status == "success"))
            {
                var result = new
                {
                    StatusCode = "正常終了",
                    Message = "SQSへメッセージの削除に成功しました。",
                };
                return Ok(JsonConvert.SerializeObject(result));
            }
            else
            {
                return  new StatusCodeResult(500);
            }
        }
    }
}