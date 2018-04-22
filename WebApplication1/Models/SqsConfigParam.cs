using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace WebApplication1.Models
{
    public class SqsConfigParam
    {
        public string ServiceUrl { get; set; }
        public int VisibilityTimeout { get; set; }
        public int ReceiveMessageWaitTimeSeconds { get; set; }

        public Dictionary<string, string> QueueAttributes => new Dictionary<string, string>()
        {
            {QueueAttributeName.VisibilityTimeout, VisibilityTimeout.ToString()},
            {QueueAttributeName.ReceiveMessageWaitTimeSeconds, ReceiveMessageWaitTimeSeconds.ToString()}
        };
    }

    public class AwsUrl
    {
        public string Value { get; }

        protected AwsUrl(string url)
        {
            Value = url;
        }
    }

    public class QueueUrl : AwsUrl
    {
        private QueueUrl(string url) : base(url)
        {
        }

        /// <summary>
        /// Queueを生成します。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="queueName"></param>
        /// <param name="queueAttributes"></param>
        /// <returns></returns>
        public static async Task<QueueUrl> Build(IAmazonSQS client, string queueName,
            Dictionary<string, string> queueAttributes)
        {
            try
            {
                var queueUrl = await client.GetQueueUrlAsync(queueName);
                return new QueueUrl(queueUrl.QueueUrl);
            }
            catch (QueueDoesNotExistException ex)
            {
                var createQueueRequest = new CreateQueueRequest
                {
                    QueueName = queueName,
                    Attributes = queueAttributes
                };
                var queue = await client.CreateQueueAsync(createQueueRequest);
                return new QueueUrl(queue.QueueUrl);
            }
        }
    }
}