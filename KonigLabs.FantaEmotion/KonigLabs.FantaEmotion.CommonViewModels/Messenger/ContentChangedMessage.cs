using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;

namespace KonigLabs.FantaEmotion.CommonViewModels.Messenger
{
    public class ContentChangedMessage
    {
        public object Parameter { get; set; }

        public BaseViewModel Content { get; set; }
    }
}
