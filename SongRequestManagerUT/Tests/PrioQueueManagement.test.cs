using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SongRequestManager.Queue;
using SongRequestManagerUT.Utils;

namespace SongRequestManagerUT.Tests
{
    [TestClass]
    public class PrioQueueManagement : TestSuiteBase
    {
        [TestInitialize]
        public override void BeforeEach()
        {
            base.BeforeEach();

            MockBotSettings.Data.RequestQueueOpen = true;
        }

        [TestMethod]
        public void TestPromoteForPrioEventAfterRequest()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User2").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual("acbe", requests[1].Song.ID);
            Assert.AreEqual("acde", requests[2].Song.ID);

            MockChat.SendPrioEvent("User2", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual("acde", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);

            MockChat.SendPrioEvent("User3", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 4.99f });

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestPromoteForRequestAfterPrioEvent()
        {
            MockChat.SendPrioEvent("User2", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });
            MockChat.SendPrioEvent("User3", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 4.99f });

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(1, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);

            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User2").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);

            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestPrioDoesNotReplaceMTT()
        {
            MockChat.SendPrioEvent("User3", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 4.99f });

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User2").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual("acbe", requests[1].Song.ID);

            MockChat.SendCommand(CommandCreator.Create("!mtt acbe").AsModerator().Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);

            Assert.AreEqual(0, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);

            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(0, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestPrioDoesNotReplaceATT()
        {
            MockChat.SendPrioEvent("User3", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 4.99f });

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!att acbe User2").AsModerator().Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);

            Assert.AreEqual(0, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);

            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(0, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestPromoteForRequestAfterManualPrio()
        {
            MockChat.SendCommand(CommandCreator.Create("!addprio User2 9.99").AsModerator().Build());
            MockChat.SendCommand(CommandCreator.Create("!addprio User3 4.99").AsModerator().Build());

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(1, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);

            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User2").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);

            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestPromoteForManualPrioAfterRequest()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User2").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual("acbe", requests[1].Song.ID);
            Assert.AreEqual("acde", requests[2].Song.ID);

            MockChat.SendCommand(CommandCreator.Create("!addprio User2 9.99").AsModerator().Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual("acde", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);

            MockChat.SendCommand(CommandCreator.Create("!addprio User3 4.99").AsModerator().Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestAddValueToExistingPrioRequest()
        {
            MockChat.SendPrioEvent("User2", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });
            MockChat.SendPrioEvent("User3", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 4.99f });

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(1, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);

            MockChat.SendCommand(CommandCreator.Create("!bsr acbe").FromUser("User2").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("4e4e", requests[1].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(0, requests[1].PriorityValue);

            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acbe", requests[0].Song.ID);
            Assert.AreEqual("acde", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.AreEqual(4.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);

            MockChat.SendPrioEvent("User3", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 20f });

            Assert.AreEqual(3, requests.Count);
            Assert.AreEqual("acde", requests[0].Song.ID);
            Assert.AreEqual("acbe", requests[1].Song.ID);
            Assert.AreEqual("4e4e", requests[2].Song.ID);

            Assert.AreEqual(24.99f, requests[0].PriorityValue);
            Assert.AreEqual(9.99f, requests[1].PriorityValue);
            Assert.AreEqual(0, requests[2].PriorityValue);
        }

        [TestMethod]
        public void TestRefundPrioRequestOnSelfRemove()
        {
            MockChat.SendPrioEvent("User1", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(1, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!remove").FromUser("User1").Build());

            Assert.AreEqual(0, requests.Count);
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
            Assert.AreEqual(9.99f, MockPriorityTracker.Data.PriorityItems["user1"].GetTotalValue());
        }

        [TestMethod]
        public void TestRefundPrioRequestOnModRemove()
        {
            MockChat.SendPrioEvent("User1", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User1").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(1, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!remove 4e4e").AsModerator().Build());

            Assert.AreEqual(0, requests.Count);
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
            Assert.AreEqual(9.99f, MockPriorityTracker.Data.PriorityItems["user1"].GetTotalValue());
        }

        [TestMethod]
        public void TestPrioTransferWithoutPrio()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User3").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User2").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!bump @User2").FromUser("User1").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
        }

        [TestMethod]
        public void TestPrioTransferById()
        {
            MockChat.SendPrioEvent("User1", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User2").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User3").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!bump 4e4e").FromUser("User1").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
        }

        [TestMethod]
        public void TestPrioTransferByUsername()
        {
            MockChat.SendPrioEvent("User1", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User2").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User3").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!bump @User3").FromUser("User1").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
        }

        [TestMethod]
        public void TestPrioTransferByHexUsername()
        {
            MockChat.SendPrioEvent("User1", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User2").Build());
            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("deadbeef").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[1].Song.ID);
            Assert.AreEqual(0, requests[1].PriorityValue);
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!bump @deadbeef").FromUser("User1").Build());

            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
        }

        [TestMethod]
        public void TestPrioTransferWithFutureRequest()
        {
            MockChat.SendCommand(CommandCreator.Create("!bsr acde").FromUser("User2").Build());

            MockChat.SendPrioEvent("User1", new PriorityEvent { Type = PriorityEventType.Test, Timestamp = DateTime.Now, Value = 9.99f });
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));

            MockChat.SendCommand(CommandCreator.Create("!bump @User3").FromUser("User1").Build());

            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
            Assert.IsTrue(MockPriorityTracker.Data.PriorityItems.ContainsKey("user3"));

            MockChat.SendCommand(CommandCreator.Create("!bsr 4e4e").FromUser("User3").Build());

            var requests = MockRequestQueue.Data.Requests;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("4e4e", requests[0].Song.ID);
            Assert.AreEqual(9.99f, requests[0].PriorityValue);
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user1"));
            Assert.IsFalse(MockPriorityTracker.Data.PriorityItems.ContainsKey("user3"));
        }
    }
}
