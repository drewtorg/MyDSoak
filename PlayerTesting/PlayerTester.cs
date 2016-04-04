using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player;
using Player.States;
using Player.Conversations;

using NUnit.Framework;
using System.Net.Sockets;
using SharedObjects;
using System.Net;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using CommSub;
using System.Threading;
using CommSub.Conversations.InitiatorConversations;

namespace PlayerTesting
{
    [TestFixture]
    public class PlayerTester
    {
        TestPlayer player = null;

        [SetUp]
        public void SetUp()
        {
            PlayerOptions options = new PlayerOptions();
            options.SetDefaults();

            player = new TestPlayer()
            {
                Options = options
            };
            player.Initialize();
            player.CommSubsystem.ConversationFactory.DefaultMaxRetries = 2;
        }

        [Test]
        public void Player_TestConstructor()
        {
            TestPlayer player = new TestPlayer();

            Assert.That(player.Label, Is.EqualTo("Drew's Player"));
            Assert.That(player.PotentialGames, Is.EquivalentTo(new List<GameInfo>()));
            Assert.That(player.Pennies, Is.EquivalentTo(new Stack<Penny>()));
            Assert.That(player.Balloons, Is.EquivalentTo(new List<Balloon>()));
            Assert.That(player.WaterSources, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(player.BalloonStores, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(player.UmbrellaSuppliers, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(player.OtherPlayers, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.NotInitialized));
        }

        [Test]
        public void Player_TestGetConversation()
        {
            // the first conversation a Player makes should be Logging in
            RequestReply conv = player.GetConversation();

            Assert.That(conv, Is.TypeOf<LoginConversation>());
            Assert.That(conv.TargetEndPoint, Is.Not.Null);
            Assert.That(conv.ToProcessId, Is.EqualTo(0));

            // registered Player should try GameListConversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            conv = player.GetConversation();

            Assert.That(conv, Is.TypeOf<GameListConversation>());
            Assert.That(conv.TargetEndPoint, Is.Not.Null);
            Assert.That(conv.ToProcessId, Is.EqualTo(0));

            // Player joining game without any potential games should try nothing
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);

            // Player joining game with at least one potential game should try JoinGameConversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
            player.PotentialGames.Add(new GameInfo { GameManagerId = 2 });
            conv = player.GetConversation();

            Assert.That(conv, Is.TypeOf<JoinGameConversation>());
            Assert.That(conv.TargetEndPoint, Is.Null);
            Assert.That(conv.ToProcessId, Is.EqualTo(2));

            // player in game should try nothing until the game starts
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);

            // Player that won a game should try pause for 3 seconds and then start back at the beginning
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Won;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            // Player that won a game should try pause for 3 seconds and then start back at the beginning
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Won;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            // Player that lost a game should try pause for 3 seconds and then start back at the beginning
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Lost;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            // Player that tied a game should try pause for 3 seconds and then start back at the beginning
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Tied;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            // Player that is terminating should not start a conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;
            conv = player.GetConversation();

            Assert.That(conv, Is.Null);
        }

        [Test]
        public void Player_TestPlay()
        {
            RequestReply conv = player.Play();

            Assert.That(conv, Is.Null);

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            conv = player.Play();

            Assert.That(conv, Is.Null);

            player.BalloonStores.Add(new GameProcessData() { ProcessId = 3 });
            conv = player.Play();

            Assert.That(conv, Is.TypeOf<BuyBalloonConversation>());
            Assert.That(conv.ToProcessId, Is.EqualTo(3));
            Assert.That(conv.TargetEndPoint, Is.Null);

            player.Balloons.Add(new Balloon() { IsFilled = false });
            player.WaterSources.Add(new GameProcessData() { ProcessId = 4 });
            conv = player.Play();

            Assert.That(conv, Is.TypeOf<FillBalloonConversation>());
            Assert.That(conv.ToProcessId, Is.EqualTo(4));
            Assert.That(conv.TargetEndPoint, Is.Null);

            player.Balloons[0].IsFilled = true;
            player.OtherPlayers.Add(new GameProcessData() { ProcessId = 5, LifePoints = 100 });
            conv = player.Play();

            Assert.That(conv, Is.TypeOf<ThrowBalloonConversation>());
            Assert.That(conv.ToProcessId, Is.EqualTo(5));
            Assert.That(conv.TargetEndPoint, Is.Null);

            player.OtherPlayers[0].LifePoints = 0;
            conv = player.Play();

            Assert.That(conv, Is.Null);
        }

        [Test]
        public void Player_TestLoginConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.RegistryEndPoint = mockEp;

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            RequestReply conv = player.GetConversation();
            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            LoginRequest req = env.ActualMessage as LoginRequest;

            Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            Assert.That(req.ProcessType, Is.EqualTo(ProcessInfo.ProcessType.Player));

            //send a bad response type
            Envelope res = new Envelope()
            {
                Message = new LoginRequest(),
                IPEndPoint = playerEp
            };

            mockRegistry.Send(res);

            Thread.Sleep(1100);

            Assert.That(player.ProxyEndPoint, Is.Null);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Initializing));

            conv = player.GetConversation();
            conv.Launch();

            //send a false success
            res = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new LoginReply()
                {
                    ProxyEndPoint = mockEp,
                    Success = false,
                    ProcessInfo = new ProcessInfo() { ProcessId = 2, Status = ProcessInfo.StatusCode.Registered },
                    ConvId = new MessageNumber() { Pid = 0, Seq = 2 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 }
                }
            };

            mockRegistry.Send(res);

            Thread.Sleep(1100);

            Assert.That(player.ProxyEndPoint, Is.Null);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Initializing));

            conv = player.GetConversation();
            conv.Launch();

            //send a good response
            res = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new LoginReply()
                {
                    ProxyEndPoint = mockEp,
                    Success = true,
                    ProcessInfo = new ProcessInfo() { ProcessId = 2, Status = ProcessInfo.StatusCode.Registered },
                    ConvId = new MessageNumber() { Pid = 0, Seq = 3 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 3 }
                }
            };

            mockRegistry.Send(res);

            Thread.Sleep(2000);

            Assert.That(player.ProxyEndPoint, Is.EqualTo(mockEp));
            Assert.That(player.MyProcessInfo.ProcessId, Is.EqualTo(2));
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            mockRegistry.Stop();
        }
       
        [Test]
        public void Player_TestAliveConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            MessageNumber.LocalProcessId = 2;

            IPEndPoint playerEp = new PublicEndPoint("127.0.0.1:15000").IPEndPoint;

            Envelope env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new AliveRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 }
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(2000);

            Reply reply = env.ActualMessage as Reply;

            Assert.That(reply.ConvId.Pid, Is.EqualTo(1));
            Assert.That(reply.ConvId.Seq, Is.EqualTo(1));
            Assert.That(reply.MsgId.Pid, Is.EqualTo(2));
            Assert.That(reply.MsgId.Seq, Is.EqualTo(1));
            Assert.That(reply.Success, Is.True);
        }
    }
}
