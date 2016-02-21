using KonigLabs.FantaEmotion.CommonViewModels.Messenger;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Dialogs;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;

namespace KonigLabs.FantaEmotion.CommonViewModels.Services
{
    public interface IDialogService
    {
        bool ShowConfirmationDialog(string message);

        void SetWindowCloseState(bool state);

        bool ShowResultDialog(ResultBaseViewModel viewModel);

        void ShowInfo(string info);
    }

    public class DialogService : IDialogService
    {
        private readonly IMessenger _messenger;
        private readonly IChildrenViewModelsFactory _viewModelFactory;

        public DialogService(IMessenger messenger, IChildrenViewModelsFactory viewModelFactory)
        {
            _messenger = messenger;
            _viewModelFactory = viewModelFactory;
        }

        public bool ShowConfirmationDialog(string messageText)
        {
            var message = _messenger.CreateMessage<ShowChildWindowMessage>();
            var viewModel = (ConfirmDialogViewModel) _viewModelFactory.GetChild<ConfirmDialogViewModel>(messageText);
            message.Content = viewModel;
            message.IsDialog = true;
            _messenger.Send(message);
            return viewModel.Status;
        }

        public void SetWindowCloseState(bool state)
        {
            var message = _messenger.CreateMessage<WindowStateMessage>();
            message.State = state;
            _messenger.Send(message);
        }

        public bool ShowResultDialog(ResultBaseViewModel contentViewModel)
        {
            var message = _messenger.CreateMessage<ShowChildWindowMessage>();
            var viewModel = (ResultDialogViewModel)_viewModelFactory.GetChild<ResultDialogViewModel>(contentViewModel);
            message.Content = viewModel;
            message.IsDialog = true;
            _messenger.Send(message);
            return viewModel.Status;
        }

        public void ShowInfo(string info)
        {
            var message = _messenger.CreateMessage<ShowChildWindowMessage>();
            var viewModel = (InfoDialogViewModel)_viewModelFactory.GetChild<InfoDialogViewModel>(info);
            message.Content = viewModel;
            message.IsDialog = false;
            _messenger.Send(message);
        }
    }
}
