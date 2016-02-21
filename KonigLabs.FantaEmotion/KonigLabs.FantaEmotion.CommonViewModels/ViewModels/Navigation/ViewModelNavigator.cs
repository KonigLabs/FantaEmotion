using KonigLabs.FantaEmotion.CommonViewModels.Messenger;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation
{
    public class ViewModelNavigator : IViewModelNavigator
    {
        private readonly IMessenger _messenger;
        private readonly IChildrenViewModelsFactory _childrenViewModelsFactory;
        private readonly ViewModelStorage _storage;

        public ViewModelNavigator(
            IMessenger messenger, 
            IChildrenViewModelsFactory childrenViewModelsFactory,
            ViewModelStorage storage
            )
        {
            _messenger = messenger;
            _childrenViewModelsFactory = childrenViewModelsFactory;
            _storage = storage;
        }

        public void NavigateBack(BaseViewModel viewModel)
        {
            var previous = _storage.Previous(viewModel);

            if (previous == null)
                return;

            RaiseContentChanged(previous);
        }

        private void RaiseContentChanged(BaseViewModel content)
        {
            var message = _messenger.CreateMessage<ContentChangedMessage>();
            message.Content = content;
            _messenger.Send(message);
        }

        public void NavigateForward(BaseViewModel from, BaseViewModel to)
        {
            var next = _storage.Next(from, to);
            RaiseContentChanged(next);
        }

        public void NavigateForward(BaseViewModel to)
        {
            var firstNode = _storage.Next(to);
            RaiseContentChanged(firstNode);
        }

        public void NavigateForward<TViewModelTo>(BaseViewModel from, object param)
            where TViewModelTo : BaseViewModel
        {
            BaseViewModel next = null;
            var existing = _storage.TryRemoveExisting<TViewModelTo>(from);
            if (existing != null)
            {
                next = existing;
            }
            else
            {
                var to = _childrenViewModelsFactory.GetChild<TViewModelTo>(param);
                next = _storage.Next(from, to);
            }
            
            RaiseContentChanged(next);
        }

        public void NavigateForward<TViewModelTo>(object param) where TViewModelTo : BaseViewModel
        {
            var to = _childrenViewModelsFactory.GetChild<TViewModelTo>(param);
            var firstNode = _storage.Next(to);
            RaiseContentChanged(firstNode);
        }
    }
}
