using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KonigLabs.FantaEmotion.PhotoBlot.ViewModels;

namespace KonigLabs.FantaEmotion.PhotoBlot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var backRect = (Rectangle) FindName("BackRect");
            if (backRect != null)
                backRect.Fill =
                    new ImageBrush(new BitmapImage(new Uri(@"app\photo.jpeg", UriKind.Relative)));

            MainViewModel = new MainViewModel();

            DataContext = MainViewModel;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            var posCur = new Point(pos.X - MainViewModel.SpotSizeX/2, pos.Y - MainViewModel.SpotSizeY/2);

            var rand = new Random();
            var color = rand.Next(MainViewModel.SpotColors.Count);

            Dispatcher.Invoke(
                () => MainViewModel.PixSpots.Add(new Blot {Pos = posCur, Ind = 0, Color = color}));
            if (!Run)
                DrawSpotByPixsAsync();
        }

        private bool Run { get; set; }

        private async void DrawSpotByPixsAsync()
        {
            Run = true;
            var timer = new Stopwatch();
            var times = new List<long>();

            var repeat = false;

            while (MainViewModel.PixSpots.Any())
            {
                timer.Reset();
                timer.Start();
                var resImg = new byte[MainViewModel.SizeX, MainViewModel.SizeY];
                foreach (var pixSpot in MainViewModel.PixSpots.Where(pixSpot => pixSpot.Ind < MainViewModel.Pixs.Count))
                {
                    repeat = true;
                    for (var i = 0; i < MainViewModel.SpotSizeX; i++)
                        for (var j = 0; j < MainViewModel.SpotSizeY; j++)
                        {
                            var x = (int) pixSpot.Pos.X + i;
                            var y = (int) pixSpot.Pos.Y + j;
                            if (x < 0 || x >= MainViewModel.SizeX || y < 0 || y >= MainViewModel.SizeY)
                                continue;
                            resImg[x, y] = (byte) (pixSpot.Color*MainViewModel.Pixs[pixSpot.Ind][i, j]);
                        }
                    pixSpot.Ind++;
                }
                if (repeat)
                    for (var i = 0; i < MainViewModel.SizeX; i++)
                    {
                        for (var j = 0; j < MainViewModel.SizeY; j++)
                        {
                            if (resImg[i, j] == 0)
                                continue;
                            MainViewModel.Overlay.SetPixel(i, j, MainViewModel.SpotColors[resImg[i, j]]);
                        }
                    }

                var copy = MainViewModel.Overlay;
                MainViewModel.Overlay = null;
                MainViewModel.Overlay = copy;

                timer.Stop();
                times.Add(timer.ElapsedMilliseconds);
                await Task.Delay(1);
                MainViewModel.PixSpots.RemoveAll(p => p.Ind >= MainViewModel.Pixs.Count);
            }
            Run = false;
        }
    }
}
