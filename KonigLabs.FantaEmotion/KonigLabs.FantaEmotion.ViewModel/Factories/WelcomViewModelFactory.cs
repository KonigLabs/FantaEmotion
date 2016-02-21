using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using KonigLabs.FantaEmotion.ViewModel.ViewModels;

namespace KonigLabs.FantaEmotion.ViewModel.Factories
{
    public class WelcomViewModelFactory : ViewModelBaseFactory<WelcomViewModel>
    {
        private IViewModelNavigator _navigator;
        public WelcomViewModelFactory(IViewModelNavigator navigator)
        {
            _navigator = navigator;
        }
        protected override WelcomViewModel GetViewModel(object param)
        {
            return new WelcomViewModel(_navigator);
        }
    }
}
