using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace WebApplication1.Models
{
    public class AwsSqs
    {
        public string ServiceUrl { get; set; }
        public int VisibilityTimeout { get; set; }
        public int ReceiveMessageWaitTimeSeconds { get; set; }

        private AmazonSQSClient _client;
        private Dictionary<string, string> _queueAttributes = new Dictionary<string, string>();

        public AmazonSQSClient Client
        {
            get
            {
                if (_client != null)
                {
                    return _client;
                }

                var sqsConfig = new AmazonSQSConfig {ServiceURL = ServiceUrl};
                _client = new AmazonSQSClient(sqsConfig);
                return _client;
            }
        }

        /// <summary>
        /// Queueを生成します。
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task<QueueUrl> GetQueueUrlAsync(string queueName)
        {
            var createQueueRequest = GetCreateQueueRequest(queueName);

            try
            {
                var queueUrl = await Client.GetQueueUrlAsync(queueName);
                return new QueueUrl(queueUrl.QueueUrl);
            }
            catch (QueueDoesNotExistException ex)
            {
                var queue = await Client.CreateQueueAsync(createQueueRequest);
                return new QueueUrl(queue.QueueUrl);
            }
        }

        private CreateQueueRequest GetCreateQueueRequest(string queueName)
        {
            if (_queueAttributes.Keys.Count == 0)
            {
                _queueAttributes = new Dictionary<string, string>
                {
                    {QueueAttributeName.VisibilityTimeout, VisibilityTimeout.ToString()},
                    {QueueAttributeName.ReceiveMessageWaitTimeSeconds, ReceiveMessageWaitTimeSeconds.ToString()}
                };
            }

            var createQueueRequest = new CreateQueueRequest
            {
                QueueName = queueName,
                Attributes = _queueAttributes
            };

            return createQueueRequest;
        }
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
        public QueueUrl(string url) : base(url)
        {
        }
    }
}