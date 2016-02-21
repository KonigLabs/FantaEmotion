using System.Threading.Tasks;
using System.Windows.Input;

namespace KonigLabs.FantaEmotion.CommonViewModels.Async
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
