using GalaSoft.MvvmLight.Command;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;

namespace KonigLabs.FantaEmotion.ViewModel.ViewModels
{
    /// <summary>
    /// VM полученного видео
    /// </summary>
    public class TakeVideoResultViewModel: BaseViewModel
    {
        private IViewModelNavigator _navigator;

        public TakeVideoResultViewModel(IViewModelNavigator navigator, string videoResultPath)
        {
            _videoResultPath = videoResultPath;
            _navigator = navigator;
        }

        #region Команды

        private RelayCommand _nextCommnad;
        private RelayCommand _repeatCommand;

        /// <summary>
        /// Продолжить
        /// </summary>
        public RelayCommand NextCommnad
        {
            get { return _nextCommnad ?? (_nextCommnad = new RelayCommand(OnNext)); }
        }

        private void OnNext() {

        }
        
        /// <summary>
        /// Повторить запись видео
        /// </summary>
        public RelayCommand RepeatCommand
        {
            get { return _repeatCommand ?? (_repeatCommand = new RelayCommand(OnRepeat)); }
        }

        private void OnRepeat()
        {
            _navigator.NavigateBack(this);
        }

        #endregion

        /// <summary>
        /// Видео
        /// </summary>
        public string VideoPath
        {
            get
            {
                return _videoResultPath;
            }
        }

        private string _videoResultPath;
    }
}
