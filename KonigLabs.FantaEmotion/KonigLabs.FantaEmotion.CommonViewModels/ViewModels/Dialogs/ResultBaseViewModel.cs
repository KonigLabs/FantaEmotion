using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Validation;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Dialogs
{
    public abstract class ResultBaseViewModel : ValidateableViewModel
    {
        public abstract bool CanConfirm { get; }

        public abstract string Title { get; }
    }
}
