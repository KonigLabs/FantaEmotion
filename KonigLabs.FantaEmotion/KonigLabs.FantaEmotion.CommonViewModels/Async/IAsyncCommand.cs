using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KonigLabs.FantaEmotion.CommonViewModels.Async
{
    public interface IAsyncCommand : ICommand
    {
        AggregateException Exceptions {  get; }
        Task ExecuteAsync(object parameter);
    }
}
