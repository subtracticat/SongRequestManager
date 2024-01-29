using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongRequestManagerUT.Utils;

namespace SongRequestManagerUT.Tests
{
    [TestClass]
    public class ModQueueManagement : TestSuiteBase
    {
        [TestInitialize]
        public override void BeforeEach()
        {
            base.BeforeEach();

            MockBotSettings.Data.RequestQueueOpen = true;
        }

        [TestMethod]
        public void TestModAdd()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!modadd 4e4e").FromUser("Moderator1").AsModerator().Build());

            Assert.AreEqual(2, MockRequestQueue.Data.Requests.Count);

            Assert.AreEqual("acbe", MockRequestQueue.Data.Requests[0].Song.ID);
            Assert.AreEqual("User1", MockRequestQueue.Data.Requests[0].RequestedBy);

            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[1].Song.ID);
            Assert.AreEqual("Moderator1", MockRequestQueue.Data.Requests[1].RequestedBy);
            Assert.IsFalse(MockRequestQueue.Data.Requests[1].IsModPromoted);
        }

        [TestMethod]
        public void TestModAddFor()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!modadd 4e4e User2").FromUser("Moderator1").AsModerator().Build());

            Assert.AreEqual(2, MockRequestQueue.Data.Requests.Count);

            Assert.AreEqual("acbe", MockRequestQueue.Data.Requests[0].Song.ID);
            Assert.AreEqual("User1", MockRequestQueue.Data.Requests[0].RequestedBy);
            Assert.IsFalse(MockRequestQueue.Data.Requests[0].IsModPromoted);

            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[1].Song.ID);
            Assert.AreEqual("User2", MockRequestQueue.Data.Requests[1].RequestedBy);
            Assert.IsFalse(MockRequestQueue.Data.Requests[1].IsModPromoted);
        }

        [TestMethod]
        public void TestAtt()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!att 4e4e").FromUser("Moderator1").AsModerator().Build());

            Assert.AreEqual(2, MockRequestQueue.Data.Requests.Count);

            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[0].Song.ID);
            Assert.AreEqual("Moderator1", MockRequestQueue.Data.Requests[0].RequestedBy);
            Assert.IsTrue(MockRequestQueue.Data.Requests[0].IsModPromoted);

            Assert.AreEqual("acbe", MockRequestQueue.Data.Requests[1].Song.ID);
            Assert.AreEqual("User1", MockRequestQueue.Data.Requests[1].RequestedBy);
            Assert.IsFalse(MockRequestQueue.Data.Requests[1].IsModPromoted);
        }

        [TestMethod]
        public void TestAttFor()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!att 4e4e User2").FromUser("Moderator1").AsModerator().Build());

            Assert.AreEqual(2, MockRequestQueue.Data.Requests.Count);

            Assert.AreEqual("4e4e", MockRequestQueue.Data.Requests[0].Song.ID);
            Assert.AreEqual("User2", MockRequestQueue.Data.Requests[0].RequestedBy);
            Assert.IsTrue(MockRequestQueue.Data.Requests[0].IsModPromoted);

            Assert.AreEqual("acbe", MockRequestQueue.Data.Requests[1].Song.ID);
            Assert.AreEqual("User1", MockRequestQueue.Data.Requests[1].RequestedBy);
            Assert.IsFalse(MockRequestQueue.Data.Requests[1].IsModPromoted);
        }
    }
}
