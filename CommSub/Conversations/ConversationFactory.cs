using Messages;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Utils;

namespace CommSub.Conversations
{
    public abstract class ConversationFactory
    {
        public int DefaultMaxRetries { get; set; }
        public int DefaultTimeOut { get; set; }

        private Dictionary<Type, Type> typeMapping;

        protected void AddTypeMapping(Type messageType, Type conversationType)
        {
            typeMapping.Add(messageType, conversationType);
        }

        public virtual void Initialize()
        {
            typeMapping = new Dictionary<Type, Type>();
            DefaultMaxRetries = 3;
            DefaultTimeOut = 3000;

            InitTypeMappings();
        }

        protected abstract void InitTypeMappings();

        public ConversationFactory()
        {
            Initialize();
        }

        public bool MessageCanStartConversation(Type messageType)
        {
            return typeMapping.ContainsKey(messageType);
        }

        public virtual Conversation CreateFromMessageType(Type messageType)
        {
            return CreateFromConversationType(typeMapping[messageType]);
        }

        public virtual Conversation CreateFromConversationType(Type convType)
        {
            Conversation conv = null;

            if (typeMapping.ContainsValue(convType))
            {
                Func<Conversation> ConversationCreator = Expression.Lambda<Func<Conversation>>(
                   Expression.New(convType.GetConstructor(Type.EmptyTypes))
                 ).Compile();

                conv = ConversationCreator();
                conv.Timeout = DefaultTimeOut;
                conv.Tries = DefaultMaxRetries;

                //TODO: Figure out what ID to assigna new conversation
                conv.Id = new MessageNumber()
                {
                    Pid = ObjectIdGenerator.Instance.GetNextIdNumber(),
                    Seq = 0
                };
            }
            return conv;
        }

    }
}
