using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using KonigLabs.FantaEmotion.ViewModel.ViewModels;

namespace KonigLabs.FantaEmotion.ViewModel.Factories
{
    public class TakePhotoResultViewModelFactory : ViewModelBaseFactory<TakePhotoResultViewModel>
    {
        private IViewModelNavigator _navigator;
        public TakePhotoResultViewModelFactory(IViewModelNavigator navigator)
        {
            _navigator = navigator;
        }
        protected override TakePhotoResultViewModel GetViewModel(object param)
        {
            return new TakePhotoResultViewModel(_navigator, (byte[])param);
        }
    }
}
