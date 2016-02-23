using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using KonigLabs.FantaEmotion.CommonViewModels.Async;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using KonigLabs.FantaEmotion.PatternProcessing.Dto;
using KonigLabs.FantaEmotion.PatternProcessing.ImageProcessors;
using KonigLabs.FantaEmotion.SDKData.Enums;
using KonigLabs.FantaEmotion.SDKData.Events;
using KonigLabs.FantaEmotion.ViewModel.Providers;
using KonigLabs.FantaEmotion.ViewModel.Settings;

namespace KonigLabs.FantaEmotion.ViewModel.ViewModels
{
    public class TakePhotoViewModel : BaseViewModel
    {

        private const int CDefWidth = 2048;
        private const int CDefHeight = 1536;

        private CameraSettingsDto _settings;
        private readonly SettingsProvider _settingsProvider;
        //private readonly IDialogService _dialogService;
        private readonly IViewModelNavigator _navigator;
        private readonly CompositionModelProcessor _imageProcessor;
        private int _width;
        private int _height;
        private int _imageNumber;

        private RelayCommand _goBackCommand;
        private RelayCommand _openSessionCommand;
        private RelayCommand _closeSessionCommand;
        private RelayCommand _refreshCameraCommand;
        private RelayCommand _startLiveViewCommand;
        private IAsyncCommand _takeVideoCommand;
        private RelayCommand<uint> _setFocusCommand;

        private byte[] _liveViewImageStream;

        private bool _sessionOpened;
        private bool _isLiveViewOn;
        private int _counter;

        static AutoResetEvent _cameraStreamSynchronize;

        public TakePhotoViewModel(
            IViewModelNavigator navigator,
            CompositionModelProcessor imageProcessor,
            SettingsProvider settingsProvider
            )
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _imageProcessor = imageProcessor;

            _width = CDefWidth;
            _height = CDefHeight;
        }

        public override void Initialize()
        {
            _imageProcessor.TimerElapsed += ImageProcessorOnTimerElapsed;
            _imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged += ImageProcessorOnStreamChanged;
            _imageProcessor.ImageNumberChanged += ImageProcessorOnImageNumberChanged;


            _imageProcessor.InitializeProcessor();
            OpenSession();
            if (!_sessionOpened)
                return;

            _settings = _settingsProvider.GetCameraSettings();

            if (_settings != null)
            {
                _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)_settings.SelectedWhiteBalance);
                _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)_settings.SelectedAvValue);
                _imageProcessor.SetSetting((uint)PropertyId.ExposureCompensation, (uint)_settings.SelectedCompensation);
                _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)_settings.SelectedIsoSensitivity);
                _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)_settings.SelectedShutterSpeed);
            }
            _cameraStreamSynchronize = new AutoResetEvent(false);
            StartLiveView();
        }

        private void ImageProcessorOnImageNumberChanged(object sender, int newValue)
        {
            ImageNumber = newValue;
        }

        public override void Dispose()
        {
            //_imageProcessor.TimerElapsed -= ImageProcessorOnTimerElapsed;
            //_imageProcessor.CameraErrorEvent -= ImageProcessorOnCameraErrorEvent;
            //_imageProcessor.ImageChanged -= ImageProcessorOnStreamChanged;
            //_imageProcessor.ImageNumberChanged -= ImageProcessorOnImageNumberChanged;

            //_sessionOpened = false;
            //// TakingPicture = false;
            //_isLiveViewOn = false;
            //_imageProcessor.Dispose();
        }

        private void ImageProcessorOnTimerElapsed(object sender, int tick)
        {
            Counter = tick;
        }

        private void ImageProcessorOnCameraErrorEvent(object sender, CameraEventBase cameraErrorInfo)
        {
            switch (cameraErrorInfo.EventType)
            {
                case CameraEventType.Shutdown:
                    //TakingPicture = false;
                    //_dialogService.ShowInfo(cameraErrorInfo.Message);
                    Dispose();
                    break;
                case CameraEventType.Error:
                    //TakingPicture = false;

                    var ev = cameraErrorInfo as ErrorEvent;
                    if (ev != null && ev.ErrorCode == ReturnValue.TakePictureAutoFocusNG)
                    {
                        //_dialogService.ShowInfo("Не удалось сфокусироваться. Пожалуйста, повторите попытку.");
                        Dispose();
                        Initialize();
                    }
                    if (ev != null && ev.ErrorCode == ReturnValue.NotSupported)
                        return;
                    break;
            }
        }

        private void ImageProcessorOnStreamChanged(object sender, ImageDto image)
        {
            Width = image.Width;
            Height = image.Height;
            LiveViewImageStream = image.ImageData;
            if (LiveViewImageStream.Length > 0)
                _cameraStreamSynchronize.Set();
        }

        private async Task<string> RecordVideo(CancellationToken cancellationToken)
        {
            IsRecordingVideo = true;
            for (var i = 3; i >= 0; --i)
            {
                Counter = i;
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

            return await Task.Run(async () =>
            {
                _imageProcessor.StartRecordVideo();

                //записываем видео продолжительностью 5 секунд
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                var newVideoPath = await _imageProcessor.StopRecordVideo();

                //todo временно. сделать по event'у (TransferCompleteEvent лежит в SDKData)
                await Task.Delay(TimeSpan.FromSeconds(6), cancellationToken);
                //todo убрать. есть в _imageProcessor.StopRecordVideo()
                var info = _imageProcessor.GetVideoDirectory();
                var lastVideo = info.EnumerateFiles("MVI*.mov").OrderByDescending(p => p.CreationTimeUtc).FirstOrDefault();

                _navigator.NavigateForward<TakeVideoResultViewModel>(this, /*newVideoPath*/lastVideo.FullName);
                IsRecordingVideo = false;
                 return newVideoPath;
            }, cancellationToken);
        }

        private bool _isRecordingVideo;

        public bool IsRecordingVideo
        {
            get
            {
                return _isRecordingVideo;
            }
            set
            {
                _isRecordingVideo = value;
                Set(() => IsRecordingVideo, ref _isRecordingVideo, value);
            }
        }

        private void StartLiveView()
        {
            _imageProcessor.StartLiveView();
            _isLiveViewOn = true;
        }

        private void RefreshCamera()
        {
            _imageProcessor.RefreshCamera();
        }

        private void CloseSession()
        {
            _imageProcessor.CloseSession();
            _sessionOpened = false;
        }

        private void OpenSession()
        {
            var result = _imageProcessor.OpenSession();
            if (!result)
            {
                //_dialogService.ShowInfo("Камера отсутствует или не готова");
                GoBack();
                return;
            }

            _sessionOpened = true;
        }

        private void GoBack()
        {
            _cameraStreamSynchronize.Do(x => x.Set());

            //var takePictireCmd = ((AsyncCommand<Task<CompositionProcessingResult>>) TakePictureCommand);
            //if (takePictireCmd.CancelCommand.CanExecute(null))
            //    takePictireCmd.CancelCommand.Execute(null);
            _navigator.NavigateBack(this);
        }


        public int Counter
        {
            get { return _counter; }
            set { Set(() => Counter, ref _counter, value); }
        }


        //public bool TakingPicture
        //{
        //    get { return _takingPicture; }
        //    set
        //    {
        //        if (_takingPicture == value)
        //            return;

        //        _takingPicture = value;
        //        RaisePropertyChanged();
        //        RaisePropertyChanged(() => NotTakingPicture);
        //    }
        //}

        //public bool NotTakingPicture
        //{
        //    get { return !TakingPicture; }
        //}

        public int Width
        {
            get { return _width; }
            set { Set(() => Width, ref _width, value); }
        }

        public int Height
        {
            get { return _height; }
            set { Set(() => Height, ref _height, value); }
        }

        public int ImageNumber
        {
            get { return _imageNumber; }
            set { Set(() => ImageNumber, ref _imageNumber, value); }
        }

        public byte[] LiveViewImageStream
        {
            get { return _liveViewImageStream; }
            set
            {
                _liveViewImageStream = value;
                RaisePropertyChanged();
            }
        }

        //public IAsyncCommand TakePictureCommand
        //{
        //    get
        //    {
        //        return _takePictureCommand ??
        //               (_takePictureCommand =
        //                   AsyncCommand.Create<Task<CompositionProcessingResult>>(t => TakePicture(t),
        //                       () => _sessionOpened && !TakingPicture && !IsRecordingVideo));
        //    }
        //}

        public IAsyncCommand TakeVideoCommand
        {
            get
            {
                return _takeVideoCommand ??
                       (_takeVideoCommand =
                           AsyncCommand.Create(RecordVideo, () => !IsRecordingVideo));
            }
        }


        private void SetFocus(uint focus)
        {
            _imageProcessor.SetFocus(focus);
        }

        private IList<uint> _focusPoints;


        public IList<uint> FocusPoints => _focusPoints ??
                                          (_focusPoints = Enum.GetValues(typeof(LiveViewDriveLens)).OfType<uint>().ToList());

        public RelayCommand<uint> SetFocusCommand
        {
            get
            {
                return _setFocusCommand ?? (_setFocusCommand = new RelayCommand<uint>(SetFocus,
                    x => _sessionOpened && _isLiveViewOn));
            }
        }

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public RelayCommand OpenSessionCommand
        {
            get { return _openSessionCommand ?? (_openSessionCommand = new RelayCommand(OpenSession, () => true)); }
            //todo
        }

        public RelayCommand CloseSessionCommand
        {
            get
            {
                return _closeSessionCommand ??
                       (_closeSessionCommand = new RelayCommand(CloseSession, () => _sessionOpened));
            }
        }

        public RelayCommand RefreshCameraCommand
        {
            get
            {
                return _refreshCameraCommand ??
                       (_refreshCameraCommand = new RelayCommand(RefreshCamera, () => _sessionOpened));
            }
        }

        public RelayCommand StartLiveViewCommand
        {
            get
            {
                return _startLiveViewCommand ??
                       (_startLiveViewCommand = new RelayCommand(StartLiveView, () => _sessionOpened));
            }
        }

    }
}
