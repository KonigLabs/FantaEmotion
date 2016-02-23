using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace VideoCollage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _videoPath = "Videos";
        private List<string> _videos = new List<string>();
        private List<MediaElement> _mediaElements = new List<MediaElement>();
        private int _countVideos = 12;
        private int _currentCountVideos = 0;
        private int _lastSelectIndex = 0;
        private int _currentFull = 0;
        private Thickness _previewPosition;
        /// <summary>
        /// Генерация списка видео
        /// </summary>
        private void CollageGenerate()
        {
            var files = Directory.GetFiles(_videoPath).ToList();
            var random = new Random();
            if (files.Count() <= _countVideos)
            {
                _videos = files;
            }
            else {
                int stop = _lastSelectIndex + _countVideos;
                _videos.Clear();
                for (int i = _lastSelectIndex; i < stop; i++)
                {
                    if (i >= files.Count)
                    {
                        for (var j = _lastSelectIndex - _videos.Count; _videos.Count != _countVideos; j--)
                        {
                            _videos.Add(files[j]);
                        }
                        break;
                    }
                    _videos.Add(files[i]);
                    _lastSelectIndex = i;
                }
            }
            for (int i = 0; i < _videos.Count; i++)
            {
                _mediaElements[i].Source = new Uri(_videos[i], UriKind.Relative);
            }
            _currentCountVideos = _videos.Count;
            _currentFull = 0;
        }

        private void ToFull()
        {
            //анимация отъезда назад
            var widthAnimation = new DoubleAnimation
            {
                To = _mediaElements[_currentFull].ActualWidth,
                Duration = TimeSpan.FromSeconds(1)
            };
            var heightAnimation = new DoubleAnimation
            {
                To = _mediaElements[_currentFull].ActualHeight,
                Duration = TimeSpan.FromSeconds(1)
            };

            var marginAnimation = new ThicknessAnimation(_previewPosition, TimeSpan.FromSeconds(1));

            var storyBoard = new Storyboard()
            {
                Children = new TimelineCollection { widthAnimation, heightAnimation, marginAnimation }
            };
            FullBorder.VerticalAlignment = VerticalAlignment.Top;
            FullBorder.HorizontalAlignment = HorizontalAlignment.Left;            
            Storyboard.SetTarget(widthAnimation, FullBorder);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTarget(heightAnimation, FullBorder);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));
            Storyboard.SetTarget(marginAnimation, FullBorder);
            Storyboard.SetTargetProperty(marginAnimation, new PropertyPath(MarginProperty));
            storyBoard.Completed += (s,e)=> {
                //задержка перед показом нового ролика 2 сек
                FullBorder.Visibility = Visibility.Collapsed;
                var t = new DispatcherTimer();
                t.Interval = TimeSpan.FromSeconds(2);
                t.Tick += (tt, t1) =>
                {
                    //выезд нового ролика
                    var p = _mediaElements[_currentFull].TransformToVisual(this).Transform(new Point(0, 0));
                    _previewPosition = new Thickness(p.X, p.Y, 0, 0);
                    FullBorder.Visibility = Visibility.Visible;
                    var newWidthAnimation = new DoubleAnimation
                    {
                        To = 400,
                        Duration = TimeSpan.FromSeconds(2)
                    };
                    var newHeightAnimation = new DoubleAnimation
                    {
                        To = 225,
                        Duration = TimeSpan.FromSeconds(2)
                    };
                    var newMarginAnimation = new ThicknessAnimation(new Thickness(Root.ActualWidth / 2 - 200, Root.ActualHeight / 2 - 112.5, 0, 0), TimeSpan.FromSeconds(2)) { From = _previewPosition };
                    Storyboard.SetTarget(newWidthAnimation, FullBorder);
                    Storyboard.SetTargetProperty(newWidthAnimation, new PropertyPath(WidthProperty));
                    Storyboard.SetTarget(newHeightAnimation, FullBorder);
                    Storyboard.SetTargetProperty(newHeightAnimation, new PropertyPath(HeightProperty));
                    Storyboard.SetTarget(newMarginAnimation, FullBorder);
                    Storyboard.SetTargetProperty(newMarginAnimation, new PropertyPath(MarginProperty));
                    var newStory = new Storyboard()
                    {
                        Children = new TimelineCollection { newHeightAnimation, newWidthAnimation, newMarginAnimation }
                    };
                    FullMedia.Source = _mediaElements[_currentFull].Source;
                    FullBorder.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _currentFull++;
                    if (_currentFull == _currentCountVideos)
                        _currentFull = 0;
                    FullBorder.BeginStoryboard(newStory);
                    t.Stop();
                };
                t.Start();
               
            };          
            FullBorder.BeginStoryboard(storyBoard);
            
        }
        

        public MainWindow()
        {
            
            InitializeComponent();
            //список элементов видое
            _mediaElements = new List<MediaElement> {
                                                        Media1,
                                                        Media2,
                                                        Media3,
                                                        Media4,
                                                        Media5,
                                                        Media6,
                                                        Media7,
                                                        Media8,
                                                        Media9,
                                                        Media10,
                                                        Media11,
                                                        Media12,
                                                    };
            //перегенерация елементов
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += (s, e) => CollageGenerate();
            timer.Start();
            //показать большой элемент
            
            var fullTimer = new DispatcherTimer();
            fullTimer.Interval = TimeSpan.FromSeconds(7);
            fullTimer.Tick += (s, e) => ToFull();
            fullTimer.Start();
            CollageGenerate();
        }

        private void OnEndedPlay(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = TimeSpan.FromSeconds(0);
            ((MediaElement)sender).Play();
        }
    }
}
