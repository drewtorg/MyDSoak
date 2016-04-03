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
        public void Player_TestLogin()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();

            player.RegistryEndPoint = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            RequestReply conv = player.GetConversation();
            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            LoginRequest req = env.ActualMessage as LoginRequest;

            Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            Assert.That(req.ProcessType, Is.EqualTo(ProcessInfo.ProcessType.Player));

            //send a bad response type
            Envelope res = new Envelope()
            {
                EndPoint = new PublicEndPoint("127.0.0.1:15000"),
                Message = new LoginRequest()
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
                EndPoint = new PublicEndPoint("127.0.0.1:15000"),
                Message = new LoginReply()
                {
                    ProxyEndPoint = new PublicEndPoint("127.0.0.1:12000"),
                    Success = false,
                    ProcessInfo = new ProcessInfo() { ProcessId = 2, Status = ProcessInfo.StatusCode.Registered }
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
                EndPoint = new PublicEndPoint("127.0.0.1:15000"),
                Message = new LoginReply()
                {
                    ProxyEndPoint = new PublicEndPoint("127.0.0.1:12000"),
                    Success = true,
                    ProcessInfo = new ProcessInfo() { ProcessId = 2, Status = ProcessInfo.StatusCode.Registered }
                }
            };

            mockRegistry.Send(res);

            Thread.Sleep(2000);

            Assert.That(player.ProxyEndPoint.HostAndPort, Is.EqualTo("127.0.0.1:12000"));
            Assert.That(player.MyProcessInfo.ProcessId, Is.EqualTo(2));
            Assert.That(player.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            mockRegistry.Stop();
        }
        //[Test]
        //public void Player_TestEverything()
        //{
        //    TestCommunicator mockRegistry = new TestCommunicator();

        //    IdentityInfo info = new IdentityInfo()
        //    {
        //        Alias = "TestAlias",
        //        ANumber = "A000",
        //        FirstName = "TestFirst",
        //        LastName = "TestLast"
        //    };

        //    //make a player with the mock registry
        //    Player.Player player = new Player.Player(new PlayerOptions()
        //    {
        //        Alias = "TestAlias",
        //        ANumber = "A000",
        //        FirstName = "TestFirst",
        //        LastName = "TestLast",
        //        EndPoint = mockRegistry.Ep.HostAndPort
        //    });
        //    player.CommSubsystem.Communicator = new TestCommunicator();
        //    PublicEndPoint playerEp = ((TestCommunicator)player.CommSubsystem.Communicator).Ep;

        //    //player will start by logging in
        //    player.Start();

        //    //pick up the sent login and verify it all came across okay
        //    Envelope loginEnvelope = mockRegistry.Receive(2000);
        //    MessageNumber expectedId = new MessageNumber() { Pid = 0, Seq = 1 };

        //    Assert.That(loginEnvelope, Is.Not.Null);
        //    //Assert.That(loginEnvelope.Ep, Is.EqualTo(mockRegistry.Ep));
        //    Assert.That(loginEnvelope.Message, Is.TypeOf(typeof(LoginRequest)));

        //    LoginRequest loginRequest = loginEnvelope.Message as LoginRequest;

        //    Assert.That(loginRequest.ConvId, Is.EqualTo(expectedId));
        //    Assert.That(loginRequest.MsgId, Is.EqualTo(expectedId));
        //    Assert.That(loginRequest.ProcessLabel, Is.EqualTo("Drew Torgeson"));
        //    Assert.That(loginRequest.ProcessType, Is.EqualTo(ProcessInfo.ProcessType.Player));

        //    //prepare to send a login reply
        //    MessageNumber replyConv = new MessageNumber() { Pid = 0, Seq = 1 };
        //    MessageNumber replyMsg = new MessageNumber() { Pid = 1, Seq = 2 };

        //    LoginReply loginReply = new LoginReply()
        //    {
        //        ProcessInfo = new ProcessInfo()
        //        {
        //            ProcessId = 4,
        //            EndPoint = playerEp
        //        },
        //        Success = true,
        //        Note = "Test",
        //        ConvId = replyConv,
        //        MsgId = replyMsg
        //    };

        //    mockRegistry.Send(new Envelope()
        //    {
        //        Message = loginReply,
        //        Ep = playerEp
        //    });

        //    //the player will now send a game list request
        //    Envelope gameListEnvelope = mockRegistry.Receive(2000);

        //    expectedId = new MessageNumber() { Pid = 4, Seq = 1 };

        //    Assert.That(gameListEnvelope, Is.Not.Null);
        //    Assert.That(gameListEnvelope.Message, Is.TypeOf(typeof(GameListRequest)));

        //    GameListRequest gameListRequest = gameListEnvelope.Message as GameListRequest;

        //    Assert.That(gameListRequest.ConvId, Is.EqualTo(expectedId));
        //    Assert.That(gameListRequest.MsgId, Is.EqualTo(expectedId));
        //    Assert.That(gameListRequest.StatusFilter, Is.EqualTo((int)GameInfo.StatusCode.Available));

        //    //prepare a game list reply
        //    replyConv = new MessageNumber() { Pid = 4, Seq = 1 };
        //    replyMsg = new MessageNumber() { Pid = 1, Seq = 3 };

        //    GameInfo[] games = new GameInfo[]
        //    {
        //        new GameInfo()
        //        {
        //            GameId = 1,
        //            GameManager = new ProcessInfo()
        //            {
        //                EndPoint = mockRegistry.Ep,
        //                ProcessId = 5
        //            },
        //            Status = GameInfo.StatusCode.Available,
        //        }
        //    };

        //    GameListReply gameListReply = new GameListReply()
        //    {
        //        Success = true,
        //        Note = "Test",
        //        ConvId = replyConv,
        //        MsgId = replyMsg,
        //        GameInfo = games
        //    };

        //    mockRegistry.Send(new Envelope()
        //    {
        //        Message = gameListReply,
        //        Ep = playerEp
        //    });

        //    //the player will now send a join game request
        //    Envelope joinGameEnvelope = mockRegistry.Receive(2000);

        //    expectedId = new MessageNumber() { Pid = 4, Seq = 2 };

        //    Assert.That(joinGameEnvelope, Is.Not.Null);
        //    Assert.That(joinGameEnvelope.Message, Is.TypeOf(typeof(JoinGameRequest)));

        //    JoinGameRequest joinGameRequest = joinGameEnvelope.Message as JoinGameRequest;

        //    Assert.That(joinGameRequest.ConvId, Is.EqualTo(expectedId));
        //    Assert.That(joinGameRequest.MsgId, Is.EqualTo(expectedId));
        //    Assert.That(joinGameRequest.GameId, Is.EqualTo(1));

        //    //prepare a join game reply
        //    replyConv = new MessageNumber() { Pid = 4, Seq = 2 };
        //    replyMsg = new MessageNumber() { Pid = 2, Seq = 4 };

        //    JoinGameReply joinGameReply = new JoinGameReply()
        //    {
        //        Success = true,
        //        Note = "Test",
        //        ConvId = replyConv,
        //        MsgId = replyMsg,
        //        GameId = 1,
        //        InitialLifePoints = 20
        //    };

        //    mockRegistry.Send(new Envelope()
        //    {
        //        Message = joinGameReply,
        //        Ep = playerEp
        //    });

        //    //the player will now wait a few seconds, let's send an alive request
        //    MessageNumber aliveConvId = new MessageNumber() { Pid = 1, Seq = 4 };
        //    Envelope aliveRequest = new Envelope()
        //    {
        //        Ep = playerEp,
        //        Message = new AliveRequest()
        //        {
        //            ConvId = aliveConvId,
        //            MsgId = aliveConvId
        //        }
        //    };

        //    mockRegistry.Send(aliveRequest);

        //    //now we wait for the player to respond
        //    Envelope aliveEnvelope = mockRegistry.Receive(2000);

        //    expectedId = new MessageNumber() { Pid = 4, Seq = 3 };

        //    Assert.That(aliveEnvelope, Is.Not.Null);
        //    Assert.That(aliveEnvelope.Message, Is.TypeOf(typeof(Reply)));

        //    Reply aliveReply = aliveEnvelope.Message as Reply;

        //    Assert.That(aliveReply.ConvId, Is.EqualTo(aliveConvId));
        //    Assert.That(aliveReply.MsgId, Is.EqualTo(expectedId));
        //    Assert.That(aliveReply.Success, Is.True);


        //    //the player will now send a log out request
        //    Envelope logoutEnvelope = mockRegistry.Receive(17000);

        //    expectedId = new MessageNumber() { Pid = 4, Seq = 4 };

        //    Assert.That(logoutEnvelope, Is.Not.Null);
        //    Assert.That(logoutEnvelope.Message, Is.TypeOf(typeof(LogoutRequest)));

        //    LogoutRequest logoutRequest = logoutEnvelope.Message as LogoutRequest;

        //    Assert.That(logoutRequest.ConvId, Is.EqualTo(expectedId));
        //    Assert.That(logoutRequest.MsgId, Is.EqualTo(expectedId));


        //    //prepare a log out reply
        //    replyConv = new MessageNumber() { Pid = 4, Seq = 4 };
        //    replyMsg = new MessageNumber() { Pid = 1, Seq = 5 };

        //    Reply logout = new Reply()
        //    {
        //        Success = true,
        //        ConvId = replyConv,
        //        MsgId = replyMsg
        //    };

        //    mockRegistry.Send(new Envelope()
        //    {
        //        Message = logout,
        //        Ep = playerEp
        //    });
        //}
    }
}
