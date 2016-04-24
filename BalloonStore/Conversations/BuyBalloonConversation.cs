using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ResponderConversations;
using CommSub.Conversations;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;
using System.Security.Cryptography;

namespace BalloonStore.Conversations
{
    public class BuyBalloonConversation : RequestReply
    {
        public bool ValidatedByPennyBank = false;
        private BalloonReply reply = null;

        protected override Type[] AllowedTypes
        {
            get
            {
                return new Type[] { typeof(BuyBalloonRequest) };
            }
        }

        protected override Message CreateReply()
        {
            BuyBalloonRequest req = Request as BuyBalloonRequest;

            reply = new BalloonReply();

            Balloon balloon = ((BalloonStore)Process).Balloons.ReserveOne();

            if (balloon == null)
                reply.Note = "No balloons left in inventory";

            if (PlayerInGame(req.ConvId.Pid) && balloon != null && IsValidPenny(req.Penny))
            {
                reply.Balloon = balloon;
                reply.Note = "Nice doing business";
                reply.Success = true;
                ((BalloonStore)Process).CachedPennies.Add(req.Penny);
            }
            else
                reply.Success = false;

            return reply;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid()
                && Request is BuyBalloonRequest
                && ((BuyBalloonRequest)Request).Penny != null;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid()
                && Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }

        private bool IsValidPenny(Penny penny)
        {
            bool result = true;

            //start by testing the cache

            if (((BalloonStore)Process).CachedPennies.Contains(penny))
            {
                reply.Note = "Invalid Penny, I've seen this one before";
                result = false;
            }
            else
            {
                //now verify the signature is valid

                RSAParameters receiverRSAKeyInfo = new RSAParameters();
                receiverRSAKeyInfo.Modulus = ((BalloonStore)Process).PennyBankPublicKey.Modulus;
                receiverRSAKeyInfo.Exponent = ((BalloonStore)Process).PennyBankPublicKey.Exponent;

                RSACryptoServiceProvider receiverRSA = new RSACryptoServiceProvider();
                receiverRSA.ImportParameters(receiverRSAKeyInfo);

                SHA1Managed hasher = new SHA1Managed();
                byte[] bytes = penny.DataBytes();
                byte[] hash = hasher.ComputeHash(bytes);

                RSAPKCS1SignatureDeformatter rsaSignComparer = new RSAPKCS1SignatureDeformatter(receiverRSA);
                rsaSignComparer.SetHashAlgorithm("SHA1");

                bool verified = rsaSignComparer.VerifySignature(hash, penny.DigitalSignature);

                if (!verified)
                {
                    reply.Note = "Invalid Penny, the signature doesn't check out";
                    result = false;
                }
                else
                {
                    //finally ask the PennyBank to do the final validation

                    ValidatePennyConversation conv = ((BalloonStore)Process).CommSubsystem.ConversationFactory.CreateFromConversationType<ValidatePennyConversation>();
                    conv.TargetEndPoint = ((BalloonStore)Process).PennyBankEndPoint;
                    conv.Parent = this;
                    conv.Pennies = new Penny[] { penny };

                    conv.Execute();

                    if (!ValidatedByPennyBank)
                    {
                        reply.Note = "Invalid Penny, PennyBank said no";
                        result = false;
                    }
                }
            }

            return result;
        }

        private bool PlayerInGame(int pid)
        {
            if (((BalloonStore)Process).Players.Where(x => x.ProcessId == pid).Count() > 0)
            {
                return true;
            }
            else
            {
                reply.Note = "You are not in the Game";
                return false;
            }
        }
    }
}
