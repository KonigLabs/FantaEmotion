using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KonigLabs.FantaEmotion.View.Convertrs
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imgBytes = value as byte[];
            if (imgBytes == null || imgBytes.Length == 0)
                return null;
            var ms = new MemoryStream(imgBytes);
            var bm = new BitmapImage();
            bm.BeginInit();
            bm.StreamSource = ms;
            bm.EndInit();
            return bm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
