﻿using Messages;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Utils;

namespace CommSub.Conversation
{
    public abstract class ConversationFactory
    {
        public int DefaultMaxRetries { get; set; }
        public int DefaultTimeOut { get; set; }

        private Dictionary<Type, Type> typeMapping;

        protected void Add(Type messageType, Type conversationType)
        {
            typeMapping.Add(messageType, conversationType);
            typeMapping.Add(conversationType, messageType);
        }

        public void Initialize()
        {
            typeMapping = new Dictionary<Type, Type>();
            DefaultMaxRetries = 3;
            DefaultTimeOut = 3000;
        }

        public ConversationFactory()
        {
            Initialize();
        }


        public Conversation CreateFromMessageType(Type type)
        {
            return CreateFromType(type);
        }

        private Conversation CreateFromType(Type type)
        {
            Type convType = null;
            if (typeMapping.TryGetValue(type, out convType))
            {
                Func<Conversation> ConversationCreator = Expression.Lambda<Func<Conversation>>(
                   Expression.New(convType.GetConstructor(Type.EmptyTypes))
                 ).Compile();

                Conversation conv = ConversationCreator();
                conv.Timeout = DefaultTimeOut;
                conv.Tries = DefaultMaxRetries;
                conv.Id = new MessageNumber()
                {
                    Pid = ObjectIdGenerator.Instance.GetNextIdNumber(),
                    Seq = 0
                };
                return conv;
            }
            return null;
        }

        public Conversation CreateFromConversationType(Type type)
        {
            return CreateFromType(type);
        }

    }
}