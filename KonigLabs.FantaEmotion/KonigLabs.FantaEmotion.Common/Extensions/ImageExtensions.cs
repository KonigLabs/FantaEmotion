using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KonigLabs.FantaEmotion.Common.Extensions
{

        public static class ImageExtensions
        {
            public static byte[] ToBytes(this Bitmap bmp)
            {
                byte[] result = { };
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        result = ms.ToArray();
                    }
                }
                catch (Exception)
                {
                }

                return result;
            }

            public static ImageSource ToImage(this byte[] buffer)
            {
                var biImg = new BitmapImage();
                var ms = new MemoryStream(buffer);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();

                return biImg;
            }
        }
    }
