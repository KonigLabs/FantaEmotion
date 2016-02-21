using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace KonigLabs.FantaEmotion.CommonViewModels.ViewModels
{
    public class BaseViewModel : ViewModelBase, IDisposable
    {
        static BaseViewModel()
        {
            DispatcherHelper.Initialize();
        }

        public virtual void Dispose()
        {
        }

        public virtual void Initialize()
        {
            
        }

        protected virtual void UiInvoke(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}
