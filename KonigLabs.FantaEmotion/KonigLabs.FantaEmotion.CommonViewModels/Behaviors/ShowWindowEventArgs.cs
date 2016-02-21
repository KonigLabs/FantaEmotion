using System;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;

namespace KonigLabs.FantaEmotion.CommonViewModels.Behaviors
{
    public class ShowWindowEventArgs : EventArgs
    {
        public BaseViewModel Context { get; private set; }

        public bool IsDialog { get; private set; }

        public ShowWindowEventArgs(BaseViewModel context, bool isDialog)
        {
            Context = context;
            IsDialog = isDialog;
        }
    }
}