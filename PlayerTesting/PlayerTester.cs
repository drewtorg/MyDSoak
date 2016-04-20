using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player;
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
            player.CommSubsystem.ConversationFactory.DefaultMaxRetries = 1;
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

            player.Balloons.Enqueue(new Balloon() { IsFilled = false });
            player.WaterSources.Add(new GameProcessData() { ProcessId = 4 });
            conv = player.Play();

            Assert.That(conv, Is.TypeOf<FillBalloonConversation>());
            Assert.That(conv.ToProcessId, Is.EqualTo(4));
            Assert.That(conv.TargetEndPoint, Is.Null);

            Balloon balloon = null;
            player.Balloons.TryDequeue(out balloon);
            balloon.IsFilled = true;
            player.Balloons.Enqueue(balloon);
            player.OtherPlayers.Add(new GameProcessData() { ProcessId = 6, LifePoints = 100 });
            player.Game.GameManagerId = 5;
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

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
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

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
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

            Assert.That(env, Is.Not.Null);

            Reply reply = env.ActualMessage as Reply;

            //Assert.That(reply.ConvId.Pid, Is.EqualTo(1));
            //Assert.That(reply.ConvId.Seq, Is.EqualTo(1));
            //Assert.That(reply.MsgId.Pid, Is.EqualTo(2));
            //Assert.That(reply.MsgId.Seq, Is.EqualTo(1));
            Assert.That(reply.Success, Is.True);
        }

        [Test]
        public void Player_TestBuyBalloonConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.ProxyEndPoint = mockEp;

            Penny penny = new Penny() { Id = 1 };

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;

            BuyBalloonConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<BuyBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no pennies
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.BalloonStores.Add(new GameProcessData() { Type = ProcessInfo.ProcessType.BalloonStore });

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<BuyBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no balloon stores
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.BalloonStores.Clear();
            player.Pennies.Push(penny);

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<BuyBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test succesful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.BalloonStores.Add(new GameProcessData() { ProcessId = 3, Type = ProcessInfo.ProcessType.BalloonStore });
            player.Pennies.Push(penny);

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<BuyBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            BuyBalloonRequest req = env.ActualMessage as BuyBalloonRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            Assert.That(req.Penny.Id, Is.EqualTo(penny.Id));

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new BalloonReply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Balloon = new Balloon() { Id = 1, IsFilled = false },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.Balloons.Count, Is.GreaterThan(0));

            Balloon balloon = null;
            player.Balloons.TryDequeue(out balloon);

            Assert.That(balloon.Id, Is.EqualTo(1));
            Assert.That(balloon.IsFilled, Is.False);

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestFillBalloonConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.ProxyEndPoint = mockEp;

            Penny penny1 = new Penny() { Id = 1 };
            Penny penny2 = new Penny() { Id = 2 };
            Balloon balloon = new Balloon() { Id = 1, IsFilled = false };

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;

            FillBalloonConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no pennies
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.WaterSources.Add(new GameProcessData() { Type = ProcessInfo.ProcessType.WaterServer });

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no water sources
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.WaterSources.Clear();
            player.Pennies.Push(penny1);
            player.Pennies.Push(penny2);

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no balloon
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.WaterSources.Add(new GameProcessData() { ProcessId = 3, Type = ProcessInfo.ProcessType.WaterServer });
            player.Pennies.Push(penny1);
            player.Pennies.Push(penny2);

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test succesful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.Balloons.Enqueue(balloon);
            player.Pennies.Push(penny1);
            player.Pennies.Push(penny2);

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            FillBalloonRequest req = env.ActualMessage as FillBalloonRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            Assert.That(req.Pennies[0].Id, Is.EqualTo(penny2.Id));
            Assert.That(req.Pennies[1].Id, Is.EqualTo(penny1.Id));
            Assert.That(req.Balloon.Id, Is.EqualTo(balloon.Id));
            Assert.That(req.Balloon.IsFilled, Is.EqualTo(balloon.IsFilled));

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new BalloonReply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Balloon = new Balloon() { Id = 1, IsFilled = true },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.Balloons.Count, Is.GreaterThan(0));

            balloon = null;
            player.Balloons.TryDequeue(out balloon);

            Assert.That(balloon.Id, Is.EqualTo(1));
            Assert.That(balloon.IsFilled, Is.True);

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestGameListConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.RegistryEndPoint = mockEp;

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;

            GameListConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<GameListConversation>();
            conv.TargetEndPoint = mockEp;
            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<GameListConversation>();
            conv.TargetEndPoint = mockEp;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            GameListRequest req = env.ActualMessage as GameListRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            //Assert.That(req.MsgId.Pid, Is.EqualTo(0));
            //Assert.That(req.MsgId.Seq, Is.EqualTo(1));
            Assert.That(req.StatusFilter, Is.EqualTo(4));

            GameInfo game = new GameInfo()
            {
                GameId = 0,
                GameManagerId = 3,
                MinPlayers = 2,
                MaxPlayers = 3
            };


            GameInfo[] games = new GameInfo[1];
            games[0] = game;

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new GameListReply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true,
                    GameInfo = games
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.PotentialGames.Count, Is.GreaterThan(0));
            Assert.That(player.PotentialGames[0].GameId, Is.EqualTo(game.GameId));
            Assert.That(player.PotentialGames[0].GameManagerId, Is.EqualTo(game.GameManagerId));
            Assert.That(player.PotentialGames[0].MinPlayers, Is.EqualTo(game.MinPlayers));
            Assert.That(player.PotentialGames[0].MaxPlayers, Is.EqualTo(game.MaxPlayers));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestGameStatusNotification()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;

            GameInfo gameEnding = new GameInfo()
            {
                Status = GameInfo.StatusCode.Ending
            };

            int[] winners1 = new int[1];
            winners1[0] = 0;
            GameInfo gameWin = new GameInfo()
            {
                Status = GameInfo.StatusCode.Complete,
                Winners = winners1
            };

            int[] winners2 = new int[2];
            winners2[0] = 0;
            winners2[1] = 2;
            GameInfo gameTie = new GameInfo()
            {
                Status = GameInfo.StatusCode.Complete,
                Winners = winners2
            };

            int[] winners3 = new int[1];
            winners3[0] = 2;
            GameInfo gameLost = new GameInfo()
            {
                Status = GameInfo.StatusCode.Complete,
                Winners = winners3
            };

            // test game ending
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Game = gameEnding
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.LeavingGame));

            // test game complete and player wins
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 2 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 },
                    Game = gameWin
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Won));

            // test game complete and player ties
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 3 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 3 },
                    Game = gameTie
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Tied));

            // test game complete and player loses
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new GameStatusNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 4 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 4 },
                    Game = gameLost
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Lost));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestHitByBalloonConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;

            // test wrong state
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            player.GameData = new GameProcessData() { LifePoints = 10 };
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new HitNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    ByPlayerId = 3
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.GameData.LifePoints, Is.EqualTo(10));

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.GameData = new GameProcessData() { LifePoints = 10 };
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new HitNotification()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    ByPlayerId = 3
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.GameData.LifePoints, Is.EqualTo(9));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestJoinGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.ProxyEndPoint = mockEp;

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;

            JoinGameConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
            conv.ToProcessId = 2;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no potential games
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
            conv.ToProcessId = 2;
            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            player.PotentialGames.Add(new GameInfo() { GameManagerId = 3 });
            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
            conv.ToProcessId = player.PotentialGames[0].GameManagerId;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            JoinGameRequest req = env.ActualMessage as JoinGameRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            //Assert.That(req.MsgId.Pid, Is.EqualTo(0));
            //Assert.That(req.MsgId.Seq, Is.EqualTo(1));
            Assert.That(req.GameId, Is.EqualTo(0));
            Assert.That(req.Process.LabelAndId, Is.EqualTo(player.MyProcessInfo.LabelAndId));

            env = new Envelope()
            {
                IPEndPoint = playerEp,
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

            Assert.That(player.Game.GameManagerId, Is.EqualTo(3));
            Assert.That(player.GameData.HasUmbrellaRaised, Is.False);
            Assert.That(player.GameData.HitPoints, Is.EqualTo(0));
            Assert.That(player.GameData.LifePoints, Is.EqualTo(100));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_LeaveGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.ProxyEndPoint = mockEp;

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;

            LeaveGameConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<LeaveGameConversation>();
            conv.ToProcessId = 2;

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<LeaveGameConversation>();
            conv.ToProcessId = 2;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            LeaveGameRequest req = env.ActualMessage as LeaveGameRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            //Assert.That(req.MsgId.Pid, Is.EqualTo(0));
            //Assert.That(req.MsgId.Seq, Is.EqualTo(1));

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new Reply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Registered));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestLogoutConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.RegistryEndPoint = mockEp;

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;

            LogoutConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<LogoutConversation>();
            conv.TargetEndPoint = mockEp;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<LogoutConversation>();
            conv.TargetEndPoint = mockEp;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            LogoutRequest req = env.ActualMessage as LogoutRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            //Assert.That(req.MsgId.Pid, Is.EqualTo(0));
            //Assert.That(req.MsgId.Seq, Is.EqualTo(1));

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new Reply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Terminating));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestReadyToStartConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            // test wrong state
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new ReadyToStart()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    GameId = 1
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new ReadyToStart()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    GameId = 1
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            env = mockRegistry.Receive(2000);

            Assert.That(env, Is.Not.Null);

            Reply reply = env.ActualMessage as Reply;

            Assert.That(reply.Success, Is.True);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.PlayingGame));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestShutdownConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new ShutdownRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 }
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);
            
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.Terminating));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestStartGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new StartGame()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.PlayingGame));

            // test failed conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new StartGame()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 2 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 },
                    Success = false
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.LeavingGame));

            mockRegistry.Stop();
        }

        [Test]
        public void ExitGameConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            // test wrong state
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new ExitGameRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    GameId = 1
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new ExitGameRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    GameId = 1
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            env = mockRegistry.Receive(1000);

            Reply reply = env.ActualMessage as Reply;

            Assert.That(reply.Success, Is.True);
            Assert.That(player.MyProcessInfo.Status, Is.EqualTo(ProcessInfo.StatusCode.LeavingGame));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestThrowBalloonConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.ProxyEndPoint = mockEp;

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;

            ThrowBalloonConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<ThrowBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no filled balloons
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            player.OtherPlayers.Add(new GameProcessData() { LifePoints = 10 });
            player.Balloons.Enqueue(new Balloon() { Id = 1, IsFilled = false });

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<ThrowBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no other players
            player.OtherPlayers[0].LifePoints = 0;
            player.Balloons.Enqueue(new Balloon() { Id = 2, IsFilled = true });

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<ThrowBalloonConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test successful conversation
            player.OtherPlayers[0].LifePoints = 10;
            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<ThrowBalloonConversation>();
            conv.TargetEndPoint = mockEp;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            ThrowBalloonRequest req = env.ActualMessage as ThrowBalloonRequest;

            //Assert.That(req.ConvId.Pid, Is.EqualTo(0));
            //Assert.That(req.ConvId.Seq, Is.EqualTo(1));
            //Assert.That(req.MsgId.Pid, Is.EqualTo(0));
            //Assert.That(req.MsgId.Seq, Is.EqualTo(1));
            Assert.That(req.Balloon.Id, Is.EqualTo(2));
            Assert.That(req.Balloon.IsFilled, Is.True);

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new Reply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.GameData.HitPoints, Is.EqualTo(1));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestAllowanceDistributionConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            // test wrong state
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new AllowanceDeliveryRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    NumberOfPennies = 100,
                    PortNumber = 16000
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new AllowanceDeliveryRequest()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    NumberOfPennies = 1,
                    PortNumber = 16000
                }
            };

            mockRegistry.Send(env);

            TcpListener server = new TcpListener(IPAddress.Any, 16000);
            server.Start();

            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            NetworkStreamExtensions.WriteStreamMessage(stream, new Penny() { Id = 1 });

            Thread.Sleep(2000);

            env = mockRegistry.Receive(1000);

            Reply reply = env.ActualMessage as Reply;

            Assert.That(reply.Success, Is.True);
            Assert.That(player.Pennies.Count, Is.EqualTo(1));
            Assert.That(player.Pennies.First(), Is.EqualTo(1));

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestUmbrellaConversations()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.ProxyEndPoint = mockEp;

            //test wrong status
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;

            RaiseUmbrellaConversation conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<RaiseUmbrellaConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            Envelope env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            //test no umbrella
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;

            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<RaiseUmbrellaConversation>();
            conv.ToProcessId = 3;

            conv.Launch();

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);
            
            //test successful RaiseUmbrellaConversation
            conv = player.CommSubsystem.ConversationFactory.CreateFromConversationType<RaiseUmbrellaConversation>();
            conv.TargetEndPoint = mockEp;
            player.Umbrella = new Umbrella() { Id = 1 };

            conv.Launch();

            env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            RaiseUmbrellaRequest req = env.ActualMessage as RaiseUmbrellaRequest;
            
            Assert.That(req.Umbrella.Id, Is.EqualTo(1));

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new Reply()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    Success = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.UmbrellaRaised, Is.True);

            //now test LowerUmbrellaConversation
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new UmbrellaLoweredNotification()
                {
                    ConvId = req.ConvId,
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 },
                    UmbrellaId = 1
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(2000);

            Assert.That(player.UmbrellaRaised, Is.False);

            mockRegistry.Stop();
        }

        [Test]
        public void Player_TestBidConversation()
        {
            Communicator mockRegistry = new Communicator() { MinPort = 13000, MaxPort = 13050 };

            mockRegistry.Start();
            PublicEndPoint mockEp = new PublicEndPoint(String.Format("127.0.0.1:{0}", mockRegistry.Port));

            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            player.RegistryEndPoint = mockEp;
            player.GetConversation().Launch(); // launch a bogus conversation in order to get the player endpoint

            Envelope env = mockRegistry.Receive(1000);

            IPEndPoint playerEp = env.IPEndPoint;

            // test wrong state
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new AuctionAnnouncement()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MinimumBid = 100
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(1000);

            Assert.That(env, Is.Null);

            // test successful conversation
            player.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            Penny[] pennies = new Penny []{ new Penny() { Id = 1 }, new Penny() { Id = 2 }, new Penny() { Id = 3 } };
            player.Pennies.PushRange(pennies);
            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new AuctionAnnouncement()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MinimumBid = 2
                }
            };

            mockRegistry.Send(env);

            env = mockRegistry.Receive(2000);

            Bid bid = env.ActualMessage as Bid;

            Assert.That(bid.Success, Is.True);
            Assert.That(bid.Pennies.Length, Is.EqualTo(2));
            Assert.That(player.Pennies.Count, Is.EqualTo(1));

            Umbrella umbrella = new Umbrella() { Id = 1 };

            env = new Envelope()
            {
                IPEndPoint = playerEp,
                Message = new BidAck()
                {
                    ConvId = new MessageNumber() { Pid = 1, Seq = 1 },
                    MsgId = new MessageNumber() { Pid = 1, Seq = 2 },
                    Success = true,
                    Umbrella = umbrella,
                    Won = true
                }
            };

            mockRegistry.Send(env);

            Thread.Sleep(1000);

            Assert.That(player.Umbrella.Id, Is.EqualTo(umbrella.Id));
            Assert.That(player.UmbrellaRaised, Is.False);

            mockRegistry.Stop();
        }
    }
}
