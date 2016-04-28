using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BalloonStore;
using BalloonStore.Conversations;

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
using System.Security.Cryptography;

namespace BalloonStoreTesting
{
    [TestFixture]
    public class BalloonStoreTester
    {
        TestBalloonStore balloonStore = null;
        const int GAME_ID = 3;
        const int GM_ID = 2;
        const int BALLOONS = 1;

        [SetUp]
        public void SetUp()
        {
            BalloonStoreOptions options = new BalloonStoreOptions();
            options.GameId = GAME_ID;
            options.GameManagerId = GM_ID;
            options.NumBalloons = BALLOONS;
            options.StoreIndex = 1;
            options.SetDefaults();

            balloonStore = new TestBalloonStore(options);
            balloonStore.CommSubsystem.ConversationFactory.DefaultMaxRetries = 1;
        }

        [Test]
        public void BalloonStore_TestConstructor()
        {
            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Initializing));
            Assert.That(balloonStore.MyProcessInfo.Type, Is.EqualTo(ProcessInfo.ProcessType.BalloonStore));
            Assert.That(balloonStore.WaterSources, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(balloonStore.BalloonStores, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(balloonStore.UmbrellaSuppliers, Is.EquivalentTo(new List<GameProcessData>()));
            Assert.That(balloonStore.Players, Is.EquivalentTo(new List<GameProcessData>()));
        }

        [Test]
        public void BalloonStore_TestGetConversation()
        {
            // the first conversation a Player makes should be Logging in
            RequestReply conv = balloonStore.GetConversation();

            Assert.That(conv, Is.TypeOf<LoginConversation>());
            Assert.That(conv.TargetEndPoint, Is.Not.Null);
            Assert.That(conv.ToProcessId, Is.EqualTo(0));

            // registered BalloonStore should not try any conversation and should immediately try joining its game
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            conv = balloonStore.GetConversation();

            Assert.That(conv, Is.Null);
            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.JoiningGame));

            // BalloonStore joining game will send a request by proxy to the game manager id
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
            conv = balloonStore.GetConversation();

            Assert.That(conv, Is.TypeOf<JoinGameConversation>());
            Assert.That(conv.TargetEndPoint, Is.Null);
            Assert.That(conv.ToProcessId, Is.EqualTo(GM_ID));

            // BalloonStore in game should try nothing until the game starts
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            conv = balloonStore.GetConversation();

            Assert.That(conv, Is.Null);

            // BalloonStore playing a game should do nothing
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            balloonStore.CreateBalloons();
            conv = balloonStore.GetConversation();

            Assert.That(conv, Is.Null);

            // BalloonStore should leave a game when it has no more balloons
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            balloonStore.Balloons.MarkAsUsed(0);
            balloonStore.Game = new GameInfo() { GameManagerId = GM_ID };
            conv = balloonStore.GetConversation();

            Assert.That(conv, Is.TypeOf<LeaveGameConversation>());
            Assert.That(conv.TargetEndPoint, Is.Null);
            Assert.That(conv.ToProcessId, Is.EqualTo(GM_ID));

            // BalloonStore that is just left a game will then log out
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
            conv = balloonStore.GetConversation();

            Assert.That(conv, Is.TypeOf<LogoutConversation>());
        }
        
        [Test]
        public void BalloonStore_TestCreateBalloons()
        {
            balloonStore.CreateBalloons();

            Assert.That(balloonStore.Balloons.AvailableCount, Is.EqualTo(BALLOONS));
            Assert.That(balloonStore.Balloons.Get(0).Id, Is.EqualTo(0));
            Assert.That(balloonStore.Balloons.Get(0).IsFilled, Is.EqualTo(false));
        }

        [Test]
        public void BalloonStore_TestLoginConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.RegistryEndPoint = mockEp;

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            RequestReply conv = balloonStore.GetConversation();
            conv.Launch();
            
            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            LoginRequest req = env.ActualMessage as LoginRequest;
            
            Assert.That(req.ProcessType, Is.EqualTo(ProcessInfo.ProcessType.BalloonStore));

            //send a bad response type
            Envelope res = new Envelope()
            {
                Message = new LoginRequest(),
                IPEndPoint = balloonStoreEP
            };

            mockRegistry.Send(res);

            Thread.Sleep(1100);

            Assert.That(balloonStore.ProxyEndPoint, Is.Null);
            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Initializing));

            conv = balloonStore.GetConversation();
            conv.Launch();

            env = mockRegistry.Receive(1000);

            //send a false success
            res = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
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

            Assert.That(balloonStore.ProxyEndPoint, Is.Null);
            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Initializing));

            conv = balloonStore.GetConversation();
            conv.Launch();

            env = mockRegistry.Receive(100);

            //send a good response
            res = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
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

            Thread.Sleep(100);

            // now execute the next id conversation
            env = mockRegistry.Receive(2000);

            Assert.That(env, Is.Not.Null);

            NextIdRequest idReq = env.ActualMessage as NextIdRequest;

            Assert.That(idReq.NumberOfIds, Is.EqualTo(BALLOONS));

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new NextIdReply()
                {
                    Success = true,
                    NextId = 5,
                    ConvId = idReq.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 4 }
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.ProxyEndPoint, Is.EqualTo(mockEp));
            Assert.That(balloonStore.MyProcessInfo.ProcessId, Is.EqualTo(2));
            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));
            Assert.That(balloonStore.Balloons.AvailableCount, Is.EqualTo(BALLOONS));
            Assert.That(balloonStore.Balloons.Get(5), Is.Not.Null);

            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_TestAliveConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            balloonStore.RegistryEndPoint = mockEp;
            balloonStore.GetConversation().Launch(); // launch a bogus conversation in order to get the balloon store endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new AliveRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 }
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(2000);

            Assert.That(env, Is.Not.Null);

            Reply reply = env.ActualMessage as Reply;
            
            Assert.That(reply.Success, Is.True);
        }

        [Test]
        public void BalloonStore_TestBuyBalloonConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            balloonStore.RegistryEndPoint = mockEp;
            balloonStore.GetConversation().Launch(); // launch a bogus conversation in order to get the balloon store endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEp = env.IPEndPoint;

            Penny penny = new Penny() { Id = 1 };

           // test not in game
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new BuyBalloonRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Penny = penny
                }
            };

            mockRegistry.Send(env);

            BalloonReply reply = mockRegistry.Receive(2000).ActualMessage as BalloonReply;

            Assert.That(reply.Balloon, Is.Null);
            Assert.That(reply.Success, Is.False);
            Assert.That(reply.Note, Is.StringContaining("You are not in the Game"));

            // test no balloons
            balloonStore.Players.Add(new GameProcessData() { ProcessId = 1 });
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new BuyBalloonRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Penny = penny
                }
            };

            mockRegistry.Send(env);

            reply = mockRegistry.Receive(2000).ActualMessage as BalloonReply;

            Assert.That(reply.Balloon, Is.Null);
            Assert.That(reply.Success, Is.False);
            Assert.That(reply.Note, Is.StringContaining("No balloons left in inventory"));

            // test cached penny
            balloonStore.CachedPennies.Add(penny);
            balloonStore.CreateBalloons();
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new BuyBalloonRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Penny = penny
                }
            };

            mockRegistry.Send(env);

            reply = mockRegistry.Receive(2000).ActualMessage as BalloonReply;

            Assert.That(reply.Balloon, Is.Null);
            Assert.That(reply.Success, Is.False);
            Assert.That(reply.Note, Is.StringContaining("Invalid Penny, I've seen this one before"));

            balloonStore.CachedPennies.Remove(penny);

            // test invalid signature
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = rsa.ExportParameters(false);
            balloonStore.PennyBankPublicKey = new PublicKey()
            {
                Exponent = parameters.Exponent,
                Modulus = parameters.Modulus
            };

            penny.DigitalSignature = new byte[] { 1, 2, 3 };

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new BuyBalloonRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Penny = penny
                }
            };

            mockRegistry.Send(env);

            reply = mockRegistry.Receive(2000).ActualMessage as BalloonReply;

            Assert.That(reply.Balloon, Is.Null);
            Assert.That(reply.Success, Is.False);
            Assert.That(reply.Note, Is.StringContaining("Invalid Penny, the signature doesn't check out"));

            // test valid signature, but failed ValidatePennyConversation
            balloonStore.PennyBankEndPoint = balloonStore.RegistryEndPoint;
            RSAPKCS1SignatureFormatter rsaSigner = new RSAPKCS1SignatureFormatter(rsa);
            rsaSigner.SetHashAlgorithm("SHA1");
            SHA1Managed hasher = new SHA1Managed();
            byte[] bytes = penny.DataBytes();
            byte[] hash = hasher.ComputeHash(bytes);

            penny.DigitalSignature = rsaSigner.CreateSignature(hash);

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new BuyBalloonRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Penny = penny
                }
            };

            mockRegistry.Send(env);


            // balloon store would have started a PennyValidation conversation
            PennyValidation request = mockRegistry.Receive(2000).ActualMessage as PennyValidation;

            Assert.That(request.MarkAsUsedIfValid, Is.True);
            Assert.That(request.Pennies[0].Id, Is.EqualTo(penny.Id));

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new Reply()
                {
                    ConvId = request.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = false
                }
            };

            reply = mockRegistry.Receive(2000).ActualMessage as BalloonReply;

            Assert.That(reply.Balloon, Is.Null);
            Assert.That(reply.Success, Is.False);
            Assert.That(reply.Note, Is.StringContaining("Invalid Penny, PennyBank said no"));

            // test successful conversation

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new BuyBalloonRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Penny = penny
                }
            };

            mockRegistry.Send(env);


            // balloon store would have started a PennyValidation conversation
            request = mockRegistry.Receive(500).ActualMessage as PennyValidation;

            Assert.That(request.MarkAsUsedIfValid, Is.True);
            Assert.That(request.Pennies[0].Id, Is.EqualTo(penny.Id));

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new Reply()
                {
                    ConvId = request.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            reply = mockRegistry.Receive(2000).ActualMessage as BalloonReply;

            Assert.That(reply.Balloon, Is.Not.Null);
            Assert.That(reply.Success, Is.True);
            Assert.That(reply.Note, Is.StringContaining("Nice doing business"));
            Assert.That(balloonStore.CachedPennies.Any(p => p.Id == penny.Id), Is.True);

            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_TestGameStatusNotification()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            balloonStore.RegistryEndPoint = mockEp;
            balloonStore.GetConversation().Launch(); // launch a bogus conversation in order to get the balloon store endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEp = env.IPEndPoint;

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;

            GameInfo gameEnding = new GameInfo()
            {
                Status = GameInfo.StatusCode.Ending
            };
            
            GameInfo gameComplete = new GameInfo()
            {
                Status = GameInfo.StatusCode.Complete
            };

            GameInfo gameInProgress = new GameInfo()
            {
                Status = GameInfo.StatusCode.InProgress
            };

            // test game ending
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Game = gameEnding
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.LeavingGame));

            // test game complete
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 2 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 },
                    Game = gameComplete
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.LeavingGame));

            // test game in progress
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 3 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 3 },
                    Game = gameInProgress
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.PlayingGame));
            
            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_TestJoinGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.ProxyEndPoint = mockEp;

            //test wrong status
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;

            JoinGameConversation conv = balloonStore.CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
            conv.ToProcessId = 2;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
            conv = balloonStore.CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
            conv.ToProcessId = GM_ID;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            JoinGameRequest req = env.ActualMessage as JoinGameRequest;
            
            Assert.That(req.GameId, Is.EqualTo(3));
            Assert.That(req.Process.LabelAndId, Is.EqualTo(balloonStore.MyProcessInfo.LabelAndId));

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new JoinGameReply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true,
                    GameId = 3,
                    InitialLifePoints = 100
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.Game.GameManagerId, Is.EqualTo(GM_ID));
            Assert.That(balloonStore.Game.GameId, Is.EqualTo(GAME_ID));
            Assert.That(balloonStore.Game.Status, Is.EqualTo(GameInfo.StatusCode.Available));
            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.JoinedGame));

            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_LeaveGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.ProxyEndPoint = mockEp;

            //test wrong status
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;

            LeaveGameConversation conv = balloonStore.CommSubsystem.ConversationFactory.CreateFromConversationType<LeaveGameConversation>();
            conv.ToProcessId = GM_ID;

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            conv = balloonStore.CommSubsystem.ConversationFactory.CreateFromConversationType<LeaveGameConversation>();
            conv.ToProcessId = GM_ID;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            LeaveGameRequest req = env.ActualMessage as LeaveGameRequest;

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new Reply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Terminating));

            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_TestLogoutConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.RegistryEndPoint = mockEp;

            //test wrong status
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;

            LogoutConversation conv = balloonStore.CommSubsystem.ConversationFactory.CreateFromConversationType<LogoutConversation>();
            conv.TargetEndPoint = mockEp;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            conv = balloonStore.CommSubsystem.ConversationFactory.CreateFromConversationType<LogoutConversation>();
            conv.TargetEndPoint = mockEp;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            LogoutRequest req = env.ActualMessage as LogoutRequest;

            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new Reply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Terminating));

            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_TestShutdownConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            balloonStore.RegistryEndPoint = mockEp;
            balloonStore.GetConversation().Launch(); // launch a bogus conversation in order to get the balloon store endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            // test successful conversation
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new ShutdownRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 }
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Terminating));

            mockRegistry.Stop();
        }

        [Test]
        public void BalloonStore_TestStartGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            balloonStore.RegistryEndPoint = mockEp;
            balloonStore.GetConversation().Launch(); // launch a bogus conversation in order to get the balloon store endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint balloonStoreEP = env.IPEndPoint;

            // test successful conversation
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new StartGame()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.PlayingGame));

            // test failed conversation
            balloonStore.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = balloonStoreEP,
                Message = new StartGame()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 2 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 },
                    Success = false
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(balloonStore.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.LeavingGame));

            mockRegistry.Stop();
        }
    }
}
