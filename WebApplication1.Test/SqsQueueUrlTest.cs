using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using WebApplication1.Models;
using Xunit;

namespace WebApplication1.Test
{
    public class SqsQueueUrlTest
    {
        private readonly Mock<IAmazonSQS> _clientMock;

        public SqsQueueUrlTest()
        {
            _clientMock = new Mock<IAmazonSQS>();
        }

        [Fact]
        public async Task Queueが存在するときBuildにより指定したQueue名のURLが取得できること()
        {
            var response = new GetQueueUrlResponse();
            response.QueueUrl = "hoge";
            _clientMock.Setup(c => c.GetQueueUrlAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(response);

            var actual = await QueueUrl.Build(_clientMock.Object, "test", new Dictionary<string, string>());
            Assert.NotNull(actual);
            Assert.Equal(response.QueueUrl, actual.Value);
        }

        [Fact]
        public async Task Queueが存在しないときBuildにより指定したQueue名のQueueが作成されURLが取得できること()
        {
            _clientMock.Setup(c => c.GetQueueUrlAsync(It.IsAny<string>(), CancellationToken.None))
                .Throws(new QueueDoesNotExistException("test_message"));

            var createQueueResponse = new CreateQueueResponse()
            {
                QueueUrl = "fuga"
            };
            _clientMock.Setup(c => c.CreateQueueAsync(It.IsAny<CreateQueueRequest>(), CancellationToken.None))
                .ReturnsAsync(createQueueResponse);

            var actual = await QueueUrl.Build(_clientMock.Object, "test", new Dictionary<string, string>());
            Assert.NotNull(actual);
            Assert.Equal(createQueueResponse.QueueUrl, actual.Value);
        }
    }
}