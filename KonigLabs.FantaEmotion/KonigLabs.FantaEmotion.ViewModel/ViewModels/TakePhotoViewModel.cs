﻿using System;
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
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.Drawing;
using System.IO;

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
        private RelayCommand _takeVideoCommand;
        private RelayCommand<uint> _setFocusCommand;

        private string _labelTakeVideo = "Take Video";

        public string LabelTakeVideo
        {
            get { return _labelTakeVideo; }
            set { _labelTakeVideo = value; RaisePropertyChanged(); }
        }

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
            _imageProcessor.CameraAddEvent += ImageProcessorCameraAddEvent;
            _imageProcessor.CameraRemoveEvent += ImageProcessorCameraRemoveEvent;
            _imageProcessor.InitializeProcessor();
            _cameraStreamSynchronize = new AutoResetEvent(false);
            OpenSession();
            if (!_sessionOpened)
                return;
            
            StartLiveView();
        }

        private void ImageProcessorCameraRemoveEvent(object sender, EventArgs e)
        {
            //Пытамся открыть ссесию если камера норм то показываем превью
            CloseSession();
            
        }

        private void ImageProcessorCameraAddEvent(object sender, EventArgs e)
        {
            //Пытамся открыть ссесию если камера норм то показываем превью
            OpenSession();
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
        List<byte[]> images = new List<byte[]>();
        private void ImageProcessorOnStreamChanged(object sender, ImageDto image)
        {
            Width = image.Width;
            Height = image.Height;
            LiveViewImageStream = image.ImageData;
            if (IsRecordingVideo)
            {
                images.Add(image.ImageData);
            }
            if (LiveViewImageStream.Length > 0)
                _cameraStreamSynchronize.Set();
        }
        DispatcherTimer _timerStop = new DispatcherTimer();
        DispatcherTimer _timerAwaitRecord = new DispatcherTimer();
        private void StopRecord(object s, EventArgs e)
        {
            _timerStop.Stop();
            _timerAwaitRecord.Tick += GetLastFile;
            _timerAwaitRecord.Interval = TimeSpan.FromSeconds(6);
            _timerAwaitRecord.Start();
            _imageProcessor.StopRecordVideo();
            /*Task.Run(() =>
            {
                Gif.Components.AnimatedGifEncoder encoder = new Gif.Components.AnimatedGifEncoder();
                encoder.SetDelay(100);
                encoder.SetRepeat(0);
                encoder.Start(Path.Combine(_imageProcessor.GetVideoDirectory().FullName, DateTime.Now.Millisecond + ".gif"));
                foreach (var img in images)
                {
                    using (var st = new MemoryStream(img))
                    {
                        encoder.AddFrame(Image.FromStream(st));
                    }
                }
                encoder.Finish();
            });*/
            
            
        }

        private void GetLastFile(object s, EventArgs e)
        {
            _timerAwaitRecord.Stop();
            var info = _imageProcessor.GetVideoDirectory();
            var lastVideo = info.EnumerateFiles("MVI*.mov").OrderByDescending(p => p.CreationTimeUtc).FirstOrDefault();
            _navigator.NavigateForward<TakeVideoResultViewModel>(this, lastVideo.FullName);
            IsRecordingVideo = false;
        }
        private async void RecordVideo()
        {
            IsRecordingVideo = true;
            for (var i = 3; i >= 0; --i)
            {
                Counter = i;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            try
            {
                _imageProcessor.StartRecordVideo();
                //записываем видео продолжительностью 5 секунд
                _timerStop.Interval = TimeSpan.FromSeconds(5);
                _timerStop.Tick += StopRecord;

                _timerStop.Start();
            }
            catch (Exception ex)
            {
                _imageProcessor.StopRecordVideo(true);
                CloseSession();
                OpenSession();
                StartLiveView();
                IsRecordingVideo = false;
                TakeVideoCommand.CanExecute(null);
                LabelTakeVideo = "Oops, an error please try again";
            }
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
            _settings = _settingsProvider.GetCameraSettings();
            if (_settings != null)
            {
                _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)_settings.SelectedWhiteBalance);
                _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)_settings.SelectedAvValue);
                _imageProcessor.SetSetting((uint)PropertyId.ExposureCompensation, (uint)_settings.SelectedCompensation);
                _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)_settings.SelectedIsoSensitivity);
                _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)_settings.SelectedShutterSpeed);
            }
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
                VisiblePreview = Visibility.Hidden;
                return;
            }
            else
            {
                VisiblePreview = Visibility.Visible;
            }
            _sessionOpened = true;
        }

        private void GoBack()
        {
            _cameraStreamSynchronize.Do(x => x.Set());
            _navigator.NavigateBack(this);
        }


        public int Counter
        {
            get { return _counter; }
            set { Set(() => Counter, ref _counter, value); }
        }

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
        public Visibility VisiblePreview { set; get; }
        

        public RelayCommand TakeVideoCommand
        {
            get
            {
                return _takeVideoCommand ??
                       (_takeVideoCommand =new 
                           RelayCommand(RecordVideo, () => !IsRecordingVideo));
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
