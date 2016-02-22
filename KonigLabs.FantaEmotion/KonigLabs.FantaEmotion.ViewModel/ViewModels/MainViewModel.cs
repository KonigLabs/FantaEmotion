using System;
using KonigLabs.FantaEmotion.CommonViewModels.Behaviors;
using KonigLabs.FantaEmotion.CommonViewModels.Messenger;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;

namespace KonigLabs.FantaEmotion.ViewModel.ViewModels
{
    public class MainViewModel : BaseViewModel, IWindowContainer
    {
        private IViewModelNavigator _navigator;
        private IMessenger _messenger;

        public MainViewModel(IViewModelNavigator navigator, IMessenger messenger)
        {
            messenger.Register<ContentChangedMessage>(this, OnContentChanged);
            _messenger = messenger;
            _navigator = navigator;
            //_navigator.NavigateForward<WelcomViewModel>(null);
            _navigator.NavigateForward<TakePhotoViewModel>(null);
        }

        private BaseViewModel _currentContent;

        public event EventHandler<ShowWindowEventArgs> ShowWindow;

        public BaseViewModel CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                RaisePropertyChanged();
            }
        }

        private void OnContentChanged(ContentChangedMessage message)
        {
            CurrentContent?.Dispose();

            CurrentContent = message.Content;
            CurrentContent?.Initialize();
        }
    }
}
