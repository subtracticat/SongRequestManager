using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongRequestManagerUT.Utils;

namespace SongRequestManagerUT.Tests
{
    [TestClass]
    public class SongRequests : TestSuiteBase
    {
        [TestInitialize]
        public override void BeforeEach()
        {
            base.BeforeEach();

            MockBotSettings.Data.RequestQueueOpen = true;
        }

        [TestMethod]
        public void TestAddNewRequestQueueClosed()
        {
            MockBotSettings.Data.RequestQueueOpen = false;

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").Build());

            Assert.AreEqual(0, MockRequestQueue.Data.Requests.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("queue is closed"));
        }

        [TestMethod]
        public void TestAddNewRequest()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").Build());

            Assert.AreEqual(1, MockRequestQueue.Data.Requests.Count);
            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[0].Song.ID);

            Assert.AreEqual(1, MockChat.Messages.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("No Strings Attached"));
        }

        [TestMethod]
        public void TestAddNewRequestWithExistingRequest()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").Build());

            Assert.AreEqual(1, MockRequestQueue.Data.Requests.Count);
            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[0].Song.ID);

            Assert.AreEqual(1, MockChat.Messages.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("No Strings Attached"));

            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").Build());

            Assert.AreEqual(1, MockRequestQueue.Data.Requests.Count);
            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[0].Song.ID);

            Assert.AreEqual(2, MockChat.Messages.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("replace your current request"));
        }

        [TestMethod]
        public void TestAddNewRequestSongBlocked()
        {
            MockSongModerationSettings.Data.Bans.Add("4e4e");

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").Build());

            Assert.AreEqual(0, MockRequestQueue.Data.Requests.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("blocked"));
        }

        [TestMethod]
        public void TestAddNewRequestSongBlockedCaseMismatch()
        {
            MockSongModerationSettings.Data.Bans.Add("4e4e");

            MockChat.SendCommand(CommandCreator.Create("!bsr 4E4E").Build());

            Assert.AreEqual(0, MockRequestQueue.Data.Requests.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("blocked"));
        }

        [TestMethod]
        public void TestAddNewRequestTooLong()
        {
            MockBotSettings.Data.MaximumSongLength = 2;

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").Build());

            Assert.AreEqual(0, MockRequestQueue.Data.Requests.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("longer than the maximum allowed song length"));
        }

        [TestMethod]
        public void TestAddNewRequestDoesNotExist()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr 1").Build());

            Assert.AreEqual(0, MockRequestQueue.Data.Requests.Count);

            Assert.AreEqual(1, MockChat.Messages.Count);
            Assert.IsTrue(MockChat.Messages.Last().Message.Contains("double check the ID and try again"));
        }
    }
}
