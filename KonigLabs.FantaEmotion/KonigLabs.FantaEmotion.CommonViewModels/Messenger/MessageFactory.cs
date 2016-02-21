using System;

namespace KonigLabs.FantaEmotion.CommonViewModels.Messenger
{
    public class MessageFactory
    {
        public TMessage CreateMessage<TMessage>()
        {
            return Activator.CreateInstance<TMessage>();
        }
    }
}
