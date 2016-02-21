using GalaSoft.MvvmLight.Command;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;

namespace KonigLabs.FantaEmotion.ViewModel.ViewModels
{
    public class WelcomViewModel:BaseViewModel
    {
        private IViewModelNavigator _navigator;
        public WelcomViewModel(IViewModelNavigator navigator)
        {
            _navigator = navigator;
        }
        private RelayCommand _startCommand;
        public RelayCommand StartCommand
        {
            get {
                return _startCommand ?? (_startCommand = new RelayCommand(OnStart));
                    }
        }
        private void OnStart()
        {
            _navigator.NavigateForward<TakePhotoViewModel>(this, null);
        }
    }
}
