using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Dialogs;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories
{
    public class InfoDialogViewModelFactory : ViewModelBaseFactory<InfoDialogViewModel>
    {
        protected override InfoDialogViewModel GetViewModel(object param)
        {
            return new InfoDialogViewModel(param.ToString());
        }
    }
}