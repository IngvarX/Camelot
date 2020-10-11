using System;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;

namespace Camelot.Tests.Common.Extensions
{
    public static class AutoMockerExtensions
    {
        public static void MockLogError(this AutoMocker autoMocker) =>
            autoMocker
                .Setup<ILogger>(m => m.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)))
                .Verifiable();

        public static void VerifyLogError(this AutoMocker autoMocker, Times times) =>
            autoMocker
                .Verify<ILogger>(m => m.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), times);
    }
}