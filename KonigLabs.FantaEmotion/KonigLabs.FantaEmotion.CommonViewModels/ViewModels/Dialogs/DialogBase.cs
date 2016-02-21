using System;
using KonigLabs.FantaEmotion.CommonViewModels.Behaviors;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Dialogs
{
    public abstract class DialogBase : BaseViewModel, ICloseable
    {
        public event EventHandler<bool> StateChanged;
        public event Action<WindowState> RequestWindowVisibilityChanged;
        public void OnClose()
        {
            
        }

        protected void Close()
        {
            var handler = RequestWindowVisibilityChanged;
            handler?.Invoke(WindowState.Closed);
        }

        public abstract string Title { get; }
    }
}