using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;

namespace KonigLabs.FantaEmotion.CommonViewModels.Messenger
{
    public class ShowChildWindowMessage
    {
        public bool IsDialog { get; set; }

        public BaseViewModel Content { get; set; }
    }
}
