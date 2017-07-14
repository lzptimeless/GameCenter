using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.UnitTest
{
    [TestClass]
    public class FileLoggerTest
    {
        private FileLogger _fileLogger;

        [TestInitialize]
        public void Initialize()
        {
            MemoryStream s = new MemoryStream();
            _fileLogger = new FileLogger(s, int.MaxValue);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _fileLogger.Dispose();
        }

        /// <summary>
        /// 内部写入异常时，可通过LastError获取这个异常
        /// </summary>
        [TestMethod]
        public void Debug_Throw_GetLastError()
        {
            Mock<Stream> streamMock = new Mock<Stream>(MockBehavior.Strict);
            streamMock.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Throws(new InvalidOperationException("Write error."));
            var fileLogger = new FileLogger(streamMock.Object, int.MaxValue);

            fileLogger.Debug("Test log content.");
            // 等待FileLogger内部的异步写入操作完成
            Thread.Sleep(200);

            Assert.IsInstanceOfType(fileLogger.LastError, typeof(InvalidOperationException));
        }

        /// <summary>
        /// 内部写入缓慢时，不影响调用点的执行速度
        /// </summary>
        [TestMethod]
        public void Debug_WriteSlowly_CallFast()
        {
            Mock<Stream> streamMock = new Mock<Stream>(MockBehavior.Strict);
            streamMock.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0)).Callback(() => Thread.Sleep(1000));
            var fileLogger = new FileLogger(streamMock.Object, int.MaxValue);

            int startTick = Environment.TickCount;
            fileLogger.Debug("Test log content.");
            int duringTick = Environment.TickCount - startTick;

            Assert.IsTrue(duringTick < 100);
        }
    }
}
