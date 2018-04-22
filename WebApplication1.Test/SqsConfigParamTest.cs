using Amazon.SQS;
using WebApplication1.Models;
using Xunit;

namespace WebApplication1.Test
{
    public class SqsConfigParamTest
    {
        private SqsConfigParam target = new SqsConfigParam();
        
        [Fact]
        public void QueueAttributesのVisibilityTimeoutが正しく設定されていること()
        {
            target.VisibilityTimeout = 99;
            Assert.Equal("99", target.QueueAttributes[QueueAttributeName.VisibilityTimeout]);
        }
        
        [Fact]
        public void QueueAttributesのReceiveMessageWaitTimeSecondsが正しく設定されていること()
        {
            target.ReceiveMessageWaitTimeSeconds = 88;
            Assert.Equal("88", target.QueueAttributes[QueueAttributeName.ReceiveMessageWaitTimeSeconds]);
        }
    }
}