using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KonigLabs.FantaEmotion.PhotoBlot.Annotations;

namespace KonigLabs.FantaEmotion.PhotoBlot.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SpotSizeX { get; set; }
        public int SpotSizeY { get; set; }

        public List<byte[,]> Pixs { get; set; }
        public List<Blot> PixSpots { get; set; }
        public List<Color> SpotColors { get; set; }

        private WriteableBitmap _overlay;

        public WriteableBitmap Overlay
        {
            get { return _overlay; }
            set
            {
                _overlay = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            SizeX = 768;
            SizeY = 512;
            SpotSizeX = 200;
            SpotSizeY = 233;

            SpotColors = new List<Color>
            {
                Color.FromArgb(150, 164, 232, 0),
                Color.FromArgb(150, 255, 185, 17),
                Color.FromArgb(150, 0, 149, 59),
                Color.FromArgb(150, 222, 184, 99),
                Color.FromArgb(150, 234, 0, 53)
            };

            var colors = new List<Color>
            {
                Colors.Red,
                Colors.Blue,
                Colors.Green
            };
            Overlay = new WriteableBitmap(SizeX, SizeY, 96, 96, PixelFormats.Pbgra32, new BitmapPalette(colors));
            Overlay.Clear(Colors.Black);

            PixSpots = new List<Blot>();
            Pixs = new List<byte[,]>();

            var spots =
                Directory.GetFiles(Directory.GetCurrentDirectory() + @"\img\").Where(p => p.Contains(".png")).ToArray();
            foreach (var spot in spots)
            {
                Pixs.Add(new byte[SpotSizeX, SpotSizeY]);
                var img = new WriteableBitmap(new BitmapImage(new Uri(spot)));
                for (var i = 0; i < img.PixelWidth; i++)
                    for (var j = 0; j < img.PixelHeight; j++)
                        if (img.GetPixel(i, j).A > 0)
                            Pixs[Pixs.Count - 1][i, j] = 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
