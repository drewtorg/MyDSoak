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
using CommSubTesting;
using System.Threading;

namespace PlayerTesting
{
    [TestFixture]
    public class PlayerTester
    {
        [Test]
        public void Player_TestEverything()
        {
            TestCommunicator mockRegistry = new TestCommunicator();
            
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };

            //make a player with the mock registry
            Player.Player player = new Player.Player(new PlayerOptions()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast",
                EndPoint = mockRegistry.Ep.HostAndPort
            });
            player.CommSubsystem.Communicator = new TestCommunicator();
            PublicEndPoint playerEp = ((TestCommunicator)player.CommSubsystem.Communicator).Ep;

            //player will start by logging in
            player.Start();

            //pick up the sent login and verify it all came across okay
            Envelope loginEnvelope = mockRegistry.Receive(2000);
            MessageNumber expectedId = new MessageNumber() { Pid = 0, Seq = 1 };

            Assert.That(loginEnvelope, Is.Not.Null);
            //Assert.That(loginEnvelope.Ep, Is.EqualTo(mockRegistry.Ep));
            Assert.That(loginEnvelope.Message, Is.TypeOf(typeof(LoginRequest)));

            LoginRequest loginRequest = loginEnvelope.Message as LoginRequest;
            
            Assert.That(loginRequest.ConvId, Is.EqualTo(expectedId));
            Assert.That(loginRequest.MsgId, Is.EqualTo(expectedId));
            Assert.That(loginRequest.ProcessLabel, Is.EqualTo("Drew Torgeson"));
            Assert.That(loginRequest.ProcessType, Is.EqualTo(ProcessInfo.ProcessType.Player));

            //prepare to send a login reply
            MessageNumber replyConv = new MessageNumber() { Pid = 0, Seq = 1 };
            MessageNumber replyMsg = new MessageNumber() { Pid = 1, Seq = 2 };

            LoginReply loginReply = new LoginReply()
            {
                ProcessInfo = new ProcessInfo()
                {
                    ProcessId = 4,
                    EndPoint = playerEp
                },
                Success = true,
                Note = "Test",
                ConvId = replyConv,
                MsgId = replyMsg
            };

            mockRegistry.Send(new Envelope()
            {
                Message = loginReply,
                Ep = playerEp
            });

            //the player will now send a game list request
            Envelope gameListEnvelope = mockRegistry.Receive(2000);

            expectedId = new MessageNumber() { Pid = 4, Seq = 1 };

            Assert.That(gameListEnvelope, Is.Not.Null);
            Assert.That(gameListEnvelope.Message, Is.TypeOf(typeof(GameListRequest)));

            GameListRequest gameListRequest = gameListEnvelope.Message as GameListRequest;

            Assert.That(gameListRequest.ConvId, Is.EqualTo(expectedId));
            Assert.That(gameListRequest.MsgId, Is.EqualTo(expectedId));
            Assert.That(gameListRequest.StatusFilter, Is.EqualTo((int)GameInfo.StatusCode.Available));

            //prepare a game list reply
            replyConv = new MessageNumber() { Pid = 4, Seq = 1 };
            replyMsg = new MessageNumber() { Pid = 1, Seq = 3 };

            GameInfo[] games = new GameInfo[]
            {
                new GameInfo()
                {
                    GameId = 1,
                    GameManager = new ProcessInfo()
                    {
                        EndPoint = mockRegistry.Ep,
                        ProcessId = 5
                    },
                    Status = GameInfo.StatusCode.Available,
                }
            };

            GameListReply gameListReply = new GameListReply()
            {
                Success = true,
                Note = "Test",
                ConvId = replyConv,
                MsgId = replyMsg,
                GameInfo = games
            };

            mockRegistry.Send(new Envelope()
            {
                Message = gameListReply,
                Ep = playerEp
            });

            //the player will now send a join game request
            Envelope joinGameEnvelope = mockRegistry.Receive(2000);

            expectedId = new MessageNumber() { Pid = 4, Seq = 2 };

            Assert.That(joinGameEnvelope, Is.Not.Null);
            Assert.That(joinGameEnvelope.Message, Is.TypeOf(typeof(JoinGameRequest)));

            JoinGameRequest joinGameRequest = joinGameEnvelope.Message as JoinGameRequest;

            Assert.That(joinGameRequest.ConvId, Is.EqualTo(expectedId));
            Assert.That(joinGameRequest.MsgId, Is.EqualTo(expectedId));
            Assert.That(joinGameRequest.GameId, Is.EqualTo(1));

            //prepare a join game reply
            replyConv = new MessageNumber() { Pid = 4, Seq = 2 };
            replyMsg = new MessageNumber() { Pid = 2, Seq = 4 };

            JoinGameReply joinGameReply = new JoinGameReply()
            {
                Success = true,
                Note = "Test",
                ConvId = replyConv,
                MsgId = replyMsg,
                GameId = 1,
                InitialLifePoints = 20
            };

            mockRegistry.Send(new Envelope()
            {
                Message = joinGameReply,
                Ep = playerEp
            });

            //the player will now wait a few seconds, let's send an alive request
            MessageNumber aliveConvId = new MessageNumber() { Pid = 1, Seq = 4 };
            Envelope aliveRequest = new Envelope()
            {
                Ep = playerEp,
                Message = new AliveRequest()
                {
                    ConvId = aliveConvId,
                    MsgId = aliveConvId
                }
            };

            mockRegistry.Send(aliveRequest);

            //now we wait for the player to respond
            Envelope aliveEnvelope = mockRegistry.Receive(2000);

            expectedId = new MessageNumber() { Pid = 4, Seq = 3 };

            Assert.That(aliveEnvelope, Is.Not.Null);
            Assert.That(aliveEnvelope.Message, Is.TypeOf(typeof(Reply)));

            Reply aliveReply = aliveEnvelope.Message as Reply;

            Assert.That(aliveReply.ConvId, Is.EqualTo(aliveConvId));
            Assert.That(aliveReply.MsgId, Is.EqualTo(expectedId));
            Assert.That(aliveReply.Success, Is.True);


            //the player will now send a log out request
            Envelope logoutEnvelope = mockRegistry.Receive(17000);

            expectedId = new MessageNumber() { Pid = 4, Seq = 4 };

            Assert.That(logoutEnvelope, Is.Not.Null);
            Assert.That(logoutEnvelope.Message, Is.TypeOf(typeof(LogoutRequest)));

            LogoutRequest logoutRequest = logoutEnvelope.Message as LogoutRequest;

            Assert.That(logoutRequest.ConvId, Is.EqualTo(expectedId));
            Assert.That(logoutRequest.MsgId, Is.EqualTo(expectedId));


            //prepare a log out reply
            replyConv = new MessageNumber() { Pid = 4, Seq = 4 };
            replyMsg = new MessageNumber() { Pid = 1, Seq = 5 };

            Reply logout = new Reply()
            {
                Success = true,
                ConvId = replyConv,
                MsgId = replyMsg
            };

            mockRegistry.Send(new Envelope()
            {
                Message = logout,
                Ep = playerEp
            });
        }
    }
}
