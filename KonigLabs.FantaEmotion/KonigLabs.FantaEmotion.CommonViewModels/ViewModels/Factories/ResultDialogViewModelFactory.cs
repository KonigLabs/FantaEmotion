using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Dialogs;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories
{
    public class ResultDialogViewModelFactory : ViewModelBaseFactory<ResultDialogViewModel>
    {
        protected override ResultDialogViewModel GetViewModel(object param)
        {
            return new ResultDialogViewModel((ResultBaseViewModel) param);
        }
    }
}
