using System;

namespace KonigLabs.FantaEmotion.CommonViewModels.Messenger
{
    public interface IMessenger
    {
        TMessage CreateMessage<TMessage>();

        void Send<TMessage>(TMessage message);

        void Register<TMessage>(object recepient, Action<TMessage> callback);

        void Unregister<TMessage>(object recepient);
    }
}
