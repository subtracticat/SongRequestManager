using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongRequestManagerUT.Utils;

namespace SongRequestManagerUT.Tests
{
    [TestClass]
    public class QueueOpenClose : TestSuiteBase
    {
        [TestMethod]
        public void TestOpen()
        {
            MockBotSettings.Data.RequestQueueOpen = false;

            MockChat.SendCommand(CommandCreator.Create("!open").AsModerator().Build());

            Assert.AreEqual(true, MockBotSettings.Data.RequestQueueOpen);
            Assert.AreEqual(1, MockChat.Messages.Count);
        }

        [TestMethod]
        public void TestOpenAlreadyOpen()
        {
            MockBotSettings.Data.RequestQueueOpen = true;

            MockChat.SendCommand(CommandCreator.Create("!open").AsModerator().Build());

            Assert.AreEqual(true, MockBotSettings.Data.RequestQueueOpen);
            Assert.AreEqual(1, MockChat.Messages.Count);
        }

        [TestMethod]
        public void TestOpenByNonModerator()
        {
            MockBotSettings.Data.RequestQueueOpen = false;

            MockChat.SendCommand(CommandCreator.Create("!open").Build());

            Assert.AreEqual(false, MockBotSettings.Data.RequestQueueOpen);
            Assert.AreEqual(0, MockChat.Messages.Count);
        }

        [TestMethod]
        public void TestClose()
        {
            MockBotSettings.Data.RequestQueueOpen = true;

            MockChat.SendCommand(CommandCreator.Create("!close").AsModerator().Build());

            Assert.AreEqual(false, MockBotSettings.Data.RequestQueueOpen);
            Assert.AreEqual(1, MockChat.Messages.Count);
        }

        [TestMethod]
        public void TestCloseAlreadyClosed()
        {
            MockBotSettings.Data.RequestQueueOpen = false;

            MockChat.SendCommand(CommandCreator.Create("!close").AsModerator().Build());

            Assert.AreEqual(false, MockBotSettings.Data.RequestQueueOpen);
            Assert.AreEqual(1, MockChat.Messages.Count);
        }

        [TestMethod]
        public void TestCloseByNonModerator()
        {
            MockBotSettings.Data.RequestQueueOpen = true;

            MockChat.SendCommand(CommandCreator.Create("!close").Build());

            Assert.AreEqual(true, MockBotSettings.Data.RequestQueueOpen);
            Assert.AreEqual(0, MockChat.Messages.Count);
        }
    }
}
