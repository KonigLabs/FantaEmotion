using EDSDKLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewFanta.Controls
{
    /// <summary>
    /// Interaction logic for TakeVideo.xaml
    /// </summary>
    public partial class TakeVideo : INotifyPropertyChanged, IDisposable
    {

        /// <summary>
        /// SDK
        /// </summary>
        SDKHandler _handler;
        /// <summary>
        /// Оповещение о том что видео сохранено
        /// </summary>
        public event EventHandler VideoSave;
        /// <summary>
        /// Обновляляка LV
        /// </summary>
        Action<BitmapImage> _setLiveView;
        /// <summary>
        /// Таймер отсчета времени до старта
        /// </summary>
        System.Windows.Threading.DispatcherTimer _timerStart = new System.Windows.Threading.DispatcherTimer();
        /// <summary>
        /// Таймер ожидания видео
        /// </summary>
        System.Windows.Threading.DispatcherTimer _timerAwaitVideo = new System.Windows.Threading.DispatcherTimer();
        /// <summary>
        /// Таймер остановки записи
        /// </summary>
        System.Windows.Threading.DispatcherTimer _timerStopRecord = new System.Windows.Threading.DispatcherTimer();
        private string _errorText = "упс! попробуй еще раз";
        private string _normalText = "запись";
        private int _counter = 0;
        private string _labelTakeVideo = "запись";
        public string LabelTakeVideo
        {
            set { _labelTakeVideo = value; OnPropertyChanged("LabelTakeVideo"); }
            get { return _labelTakeVideo; }
        }
        public int Counter
        {
            set { _counter = value; OnPropertyChanged("Counter"); }
            get { return _counter; }
        }
        public TakeVideo()
        {
            InitializeComponent();
            LiveView.Background = new ImageBrush();
            _setLiveView = bmp => ((ImageBrush)LiveView.Background).ImageSource = bmp;
            BtnRun.IsEnabled = false;
            Init();
        }
        /// <summary>
        /// Инициализируем SDK
        /// </summary>
        public void Init()
        {

            try
            {
                _handler = new SDKHandler();
                _handler.LiveViewUpdated += Handler_LiveViewUpdated;
                _handler.CameraHasShutdown += CameraHasShutdown;
                _handler.CameraAdded += CameraAdded;
                RefreshCam();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex);
            }
        }
        /// <summary>
        /// Запуск LV
        /// </summary>
        public void StartLiveView()
        {
            if (_handler != null)
            {
                try
                {
                    _handler.StartLiveView();
                }
                catch (Exception ex)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error(ex);
                }

            }
        }
        
        /// <summary>
        /// Обновим камеры 
        /// </summary>
        private void RefreshCam()
        {
            try
            {
                if (_handler.CameraSessionOpen)
                {
                    _handler.CloseSession();
                }
                var camers = _handler.GetCameraList();
                var cam = camers.FirstOrDefault();
                if (cam != null)
                {
                    _handler.OpenSession(cam);
                    BtnRun.IsEnabled = true;
                    BtnRun.Visibility = Visibility.Visible;
                    StartLiveView();
                }
            }
            catch (Exception ex){
                NLog.LogManager.GetCurrentClassLogger().Error(ex);
            }
        }
        /// <summary>
        /// Камера пришла
        /// </summary>
        private void CameraAdded()
        {
            RefreshCam();
        }
        /// <summary>
        /// Камера ушла
        /// </summary>
        private void CameraHasShutdown(object sender, EventArgs e)
        {
            ((ImageBrush)LiveView.Background).ImageSource = null;
            RefreshCam();
            
        }
        /// <summary>
        /// Обновление  LV
        /// </summary>
        /// <param name="img"></param>
        private void Handler_LiveViewUpdated(Stream img)
        {
            using (var wr = new WrappingStream(img))
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = wr;
                bmp.EndInit();
                bmp.Freeze();
                Application.Current.Dispatcher.Invoke(_setLiveView, bmp);
            }
        }
        
        /// <summary>
        /// ининциализации записи видео
        /// </summary>
        public void InitRecord()
        {
            BtnRun.Visibility = Visibility.Collapsed;
            Counter = 4;
            _timerStart.Interval = TimeSpan.FromSeconds(1);
            _timerStart.Tick += TimerTick;
            _timerStart.Start();
        }
        /// <summary>
        /// Жмяк по кнопке
        /// </summary>
        private void OnRun(object sender, RoutedEventArgs e)
        {
            InitRecord();
        }
        /// <summary>
        /// Сработка таймера
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            Counter--;
            if (Counter == 0)
            {
                _timerStart.Stop();
                _timerStart.Tick -= TimerTick;
                try
                {
                    _handler.StartFilming(MainWindow.GetLocalVideoDirectory().FullName);
                    LabelTakeVideo = _normalText;
                }
                catch (Exception ex)
                {
                    LabelTakeVideo = _errorText;
                    Terminate();
                    Init();
                    NLog.LogManager.GetCurrentClassLogger().Error(ex);
                    return;
                }
                _timerStopRecord.Interval = TimeSpan.FromSeconds(5.5);
                _timerStopRecord.Tick += TimerStopRecordTick;
                _timerStopRecord.Start();

            }
        }
        /// <summary>
        /// остановка записи
        /// </summary>
        private void TimerStopRecordTick(object sender, EventArgs e)
        {
            _timerStopRecord.Tick -= TimerStopRecordTick;
            _timerStopRecord.Stop();
            try
            {
                _handler.StartLiveView();
                _handler.StopFilming();
                LabelTakeVideo = _normalText;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex);
                LabelTakeVideo = _errorText;
                Terminate();
                Init();
                return;
            }

            _timerAwaitVideo.Interval = TimeSpan.FromSeconds(3);
            _timerAwaitVideo.Tick += TimerAwaitVideo; ;
            _timerAwaitVideo.Start();

        }
        /// <summary>
        /// ждем пока видео запишется на диск
        /// </summary>
        private void TimerAwaitVideo(object sender, EventArgs e)
        {
            _timerAwaitVideo.Tick -= TimerAwaitVideo;
            _timerAwaitVideo.Stop();
            ((ImageBrush)LiveView.Background).ImageSource = null;
            BtnRun.Visibility = Visibility.Visible;
            if (VideoSave != null)
                VideoSave(this, new EventArgs());
        }

        public void Dispose()
        {
            if (_handler != null)
            {
                Terminate();
            }
            _timerAwaitVideo = null;
            _timerStart = null;
            _timerStopRecord = null;
        }
        /// <summary>
        /// рубильник
        /// </summary>
        public void Terminate()
        {
            if (_handler == null)
                return;
            _handler.LiveViewUpdated -= Handler_LiveViewUpdated;
            _handler.CloseSession();
            _handler.Dispose();
            _handler = null;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }

}
