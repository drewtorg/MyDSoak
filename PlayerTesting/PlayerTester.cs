using System;
using System.Collections;
using System.Collections.Generic;

using Player;
using SharedObjects;

using System.Net.Sockets;

using NUnit.Framework;
using System.Net;
using System.Threading;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace PlayerTesting
{
    [TestFixture]
    public class PlayerTester
    {
        [Test]
        public void PlayerConstructor()
        {
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };
            Player.OldPlayer player = new Player.OldPlayer("127.0.0.1:12000", info);

            Assert.That(player.Game, Is.Null);
            Assert.That(player.Identity, Is.EqualTo(info));
            Assert.That(player.Observers, Is.EquivalentTo(new List<IObserver>()));
            Assert.That(player.Process, Is.Null);
            Assert.That(player.RegistryEndPoint.HostAndPort, Is.EqualTo("127.0.0.1:12000"));
        }

        [Test]
        public void PlayerSubscribe()
        {
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };
            Player.OldPlayer player = new Player.OldPlayer("127.0.0.1:12000", info);

            IObserver form = new PlayerForm();

            player.Subscribe(form);

            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Subscribe(form);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Subscribe(null);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));
        }

        [Test]
        public void PlayerUnsubscribe()
        {
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };
            Player.OldPlayer player = new Player.OldPlayer("127.0.0.1:12000", info);

            IObserver form = new PlayerForm();
            IObserver form2 = new PlayerForm();

            player.Unsubscribe(form);
            Assert.That(player.Observers.Count, Is.EqualTo(0));

            player.Subscribe(form);

            player.Unsubscribe(null);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Unsubscribe(form2);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Unsubscribe(form);
            Assert.That(player.Observers.Count, Is.EqualTo(0));
        }

        [Test]
        public void PlayerLogin()
        {
            //make a mock registry
            UdpClient mockClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            int mockClientPort = ((IPEndPoint)mockClient.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEP = new PublicEndPoint()
            {
                Host = "127.0.0.1",
                Port = mockClientPort
            };

            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };

            //make a player with the mock registry
            TestablePlayer player = new TestablePlayer("127.0.0.1:12000", info);
            player.RegistryEndPoint = mockClientEP;

            //make the player login
            player.SendLoginRequest();

            //have the mock registry receive the message
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = mockClient.Receive(ref senderEP);

            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);

            Message msg = Message.Decode(bytes);

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is LoginRequest);

            LoginRequest request = msg as LoginRequest;
            StringAssert.AreEqualIgnoringCase(player.Identity.Alias, request.Identity.Alias);
            StringAssert.AreEqualIgnoringCase(player.Identity.ANumber, request.Identity.ANumber);
            StringAssert.AreEqualIgnoringCase(player.Identity.FirstName, request.Identity.FirstName);
            StringAssert.AreEqualIgnoringCase(player.Identity.LastName, request.Identity.LastName);

            LoginReply reply = new LoginReply()
            {
                ProcessInfo = new ProcessInfo()
                {
                    ProcessId = 4,
                    EndPoint = mockClientEP
                },
                Success = true,
                Note = "Test"
            };
            bytes = reply.Encode();

            //have the mock registry send a login reply
            mockClient.Send(bytes, bytes.Length, senderEP);

            //make the player receive the login reply
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            bytes = player.PlayerClient.Receive(ref ep);

            msg = Message.Decode(bytes);

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is LoginReply);

            LoginReply reply2 = msg as LoginReply;
            Assert.AreEqual(reply.Success, reply2.Success);
            StringAssert.AreEqualIgnoringCase(reply.Note, reply2.Note);
            Assert.AreEqual(reply.ProcessInfo.ProcessId, reply2.ProcessInfo.ProcessId);
            Assert.AreEqual(reply.ProcessInfo.EndPoint, reply2.ProcessInfo.EndPoint);
        }

        [Test]
        public void PlayerGameListRequest()
        {
            //make a mock registry
            UdpClient mockClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            int mockClientPort = ((IPEndPoint)mockClient.Client.LocalEndPoint).Port;
            PublicEndPoint mockClientEP = new PublicEndPoint()
            {
                Host = "127.0.0.1",
                Port = mockClientPort
            };

            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };

            //make a player with the mock registry
            TestablePlayer player = new TestablePlayer("127.0.0.1:12000", info);
            player.RegistryEndPoint = mockClientEP;

            //make the player send a game list request
            player.SendGameListRequest();

            //have the mock registry receive the message
            IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = mockClient.Receive(ref senderEP);

            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);

            Message msg = Message.Decode(bytes);

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is GameListRequest);

            GameListRequest request = msg as GameListRequest;
            Assert.AreEqual((int)GameInfo.StatusCode.Available, request.StatusFilter);

            GameListReply reply = new GameListReply()
            {
                Success = true,
                Note = "Test",
                GameInfo = new GameInfo[] 
                {
                    new GameInfo()
                    {
                        GameId = 1,
                        Status = GameInfo.StatusCode.Available,
                        Label = "Test"
                    }
                }
            };
            bytes = reply.Encode();

            //have the mock registry send a login reply
            mockClient.Send(bytes, bytes.Length, senderEP);

            //make the player receive the login reply
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            bytes = player.PlayerClient.Receive(ref ep);

            msg = Message.Decode(bytes);

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg is GameListReply);

            GameListReply reply2 = msg as GameListReply;
            Assert.AreEqual(reply.Success, reply2.Success);
            StringAssert.AreEqualIgnoringCase(reply.Note, reply2.Note);
            Assert.AreEqual(reply.GameInfo.Length, reply2.GameInfo.Length);
            Assert.AreEqual(reply.GameInfo[0].GameId, reply2.GameInfo[0].GameId);
            Assert.AreEqual(reply.GameInfo[0].Label, reply2.GameInfo[0].Label);
            Assert.AreEqual(reply.GameInfo[0].Status, reply2.GameInfo[0].Status);
        }
    }

    public class TestablePlayer : Player.OldPlayer
    {
        public TestablePlayer(string endpoint, IdentityInfo info) : base(endpoint, info) { }

        public UdpClient PlayerClient { get { return client; } }
    }
}
