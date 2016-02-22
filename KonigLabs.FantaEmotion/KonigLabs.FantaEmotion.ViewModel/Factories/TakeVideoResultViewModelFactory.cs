using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using KonigLabs.FantaEmotion.ViewModel.ViewModels;

namespace KonigLabs.FantaEmotion.ViewModel.Factories
{
    public class TakeVideoResultViewModelFactory : ViewModelBaseFactory<TakeVideoResultViewModel>
    {
        private IViewModelNavigator _navigator;
        public TakeVideoResultViewModelFactory(IViewModelNavigator navigator)
        {
            _navigator = navigator;
        }
        protected override TakeVideoResultViewModel GetViewModel(object param)
        {
            return new TakeVideoResultViewModel(_navigator, (string)param);
        }
    }
}
