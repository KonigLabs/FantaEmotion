using System;

namespace KonigLabs.FantaEmotion.CommonViewModels.Behaviors
{
    public interface IWindowContainer
    {
        event EventHandler<ShowWindowEventArgs> ShowWindow; 
    }
}
