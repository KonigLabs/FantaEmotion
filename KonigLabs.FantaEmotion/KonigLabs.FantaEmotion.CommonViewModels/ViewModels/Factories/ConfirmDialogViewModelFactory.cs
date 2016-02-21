using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Dialogs;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories
{
    public class ConfirmDialogViewModelFactory : ViewModelBaseFactory<ConfirmDialogViewModel>
    {
        protected override ConfirmDialogViewModel GetViewModel(object param)
        {
            return new ConfirmDialogViewModel(param.ToString());
        }
    }
}