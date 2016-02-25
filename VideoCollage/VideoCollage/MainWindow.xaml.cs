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
using VideoCollage.ViewModels;

namespace VideoCollage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();

        }

        private void MyMediaElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            ToFull(sender as MediaElement);
        }
        private Thickness _previewPosition;
        private void ToFull(MediaElement elem)
        {
             
            var p = elem.TransformToVisual(this).Transform(new Point(0, 0));
            _previewPosition = new Thickness(p.X, p.Y, 0, 0);
            FullBorder.Visibility = Visibility.Collapsed;
            FullBorder.Width = 400;
            FullBorder.Height = 225;
            FullBorder.Margin = new Thickness(Root.ActualWidth / 2 - 200, Root.ActualHeight / 2 - 112.5, 0, 2);
            FullMedia.Source = elem.Source;

            FullBorder.Visibility = Visibility.Visible;


            FullBorder.BorderBrush = new SolidColorBrush(Colors.Orange);
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromSeconds(2));
          
            //da.RepeatBehavior=new RepeatBehavior(3);
            //da.FillBehavior == FillBehavior.Stop;
            FullBorder.BeginAnimation(OpacityProperty, da);


            var t = new DispatcherTimer();
                t.Interval = TimeSpan.FromSeconds(5);
                t.Tick += (tt, t1) =>
                { //анимация отъезда назад
                    
                    var widthAnimation = new DoubleAnimation
                    {
                        To = elem.ActualWidth,
                        Duration = TimeSpan.FromSeconds(1)
                    };
                    var heightAnimation = new DoubleAnimation
                    {
                        To = elem.ActualHeight,
                        Duration = TimeSpan.FromSeconds(1)
                    };

                    var marginAnimation = new ThicknessAnimation(_previewPosition, TimeSpan.FromSeconds(1));

                    var storyBoard = new Storyboard()
                    {
                        Children = new TimelineCollection { widthAnimation, heightAnimation, marginAnimation }
                    };

                    storyBoard.FillBehavior=FillBehavior.Stop;

                    FullBorder.VerticalAlignment = VerticalAlignment.Top;
                    FullBorder.HorizontalAlignment = HorizontalAlignment.Left;
                    Storyboard.SetTarget(widthAnimation, FullBorder);
                    Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));
                    Storyboard.SetTarget(heightAnimation, FullBorder);
                    Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));
                    Storyboard.SetTarget(marginAnimation, FullBorder);
                    Storyboard.SetTargetProperty(marginAnimation, new PropertyPath(MarginProperty));
                    storyBoard.Completed += (c, a) =>
                    {
                        FullBorder.Visibility = Visibility.Collapsed; ;
                    };
                    FullBorder.BeginStoryboard(storyBoard);

                    t.Stop();
                };
                
                t.Start();

        }
    }

}
