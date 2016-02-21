using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KonigLabs.FantaEmotion.Common.Extensions
{
    public static class BitmapExtensions
    {
        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLenght)
        {
            var ratio = 1.0f;
            var maxSide = sourceBitmap.Width > sourceBitmap.Height
                ? sourceBitmap.Width
                : sourceBitmap.Height;

            ratio = maxSide / (float)canvasWidthLenght;

            var bitmapResult = (sourceBitmap.Width > sourceBitmap.Height
                ? new Bitmap(canvasWidthLenght, (int)(sourceBitmap.Height / ratio))
                : new Bitmap((int)(sourceBitmap.Width / ratio), canvasWidthLenght));

            using (var graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphicsResult.DrawImage(sourceBitmap,
                    new Rectangle(0, 0,
                        bitmapResult.Width, bitmapResult.Height),
                    new Rectangle(0, 0,
                        sourceBitmap.Width, sourceBitmap.Height),
                    GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            return bitmapResult;
        }

        public static Bitmap CartoonFilter(this Bitmap sourceBitmap,
            int levels,
            int filterSize,
            byte threshold)
        {
            var paintFilterImage =
                sourceBitmap.OilPaintFilter(levels, filterSize);

            var edgeDetectImage =
                sourceBitmap.GradientBasedEdgeDetectionFilter(threshold);

            var paintData =
                paintFilterImage.LockBits(new Rectangle(0, 0,
                    paintFilterImage.Width, paintFilterImage.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

            var paintPixelBuffer = new byte[paintData.Stride *
                                               paintData.Height];

            Marshal.Copy(paintData.Scan0, paintPixelBuffer, 0,
                paintPixelBuffer.Length);

            paintFilterImage.UnlockBits(paintData);

            var edgeData =
                edgeDetectImage.LockBits(new Rectangle(0, 0,
                    edgeDetectImage.Width, edgeDetectImage.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

            var edgePixelBuffer = new byte[edgeData.Stride *
                                              edgeData.Height];

            Marshal.Copy(edgeData.Scan0, edgePixelBuffer, 0,
                edgePixelBuffer.Length);

            edgeDetectImage.UnlockBits(edgeData);

            var resultBuffer = new byte[edgeData.Stride *
                                           edgeData.Height];

            for (var k = 0; k + 4 < paintPixelBuffer.Length; k += 4)
            {
                if (edgePixelBuffer[k] == 255 ||
                    edgePixelBuffer[k + 1] == 255 ||
                    edgePixelBuffer[k + 2] == 255)
                {
                    resultBuffer[k] = 0;
                    resultBuffer[k + 1] = 0;
                    resultBuffer[k + 2] = 0;
                    resultBuffer[k + 3] = 255;
                }
                else
                {
                    resultBuffer[k] = paintPixelBuffer[k];
                    resultBuffer[k + 1] = paintPixelBuffer[k + 1];
                    resultBuffer[k + 2] = paintPixelBuffer[k + 2];
                    resultBuffer[k + 3] = 255;
                }
            }

            var resultBitmap = new Bitmap(sourceBitmap.Width,
                sourceBitmap.Height);

            var resultData =
                resultBitmap.LockBits(new Rectangle(0, 0,
                    resultBitmap.Width, resultBitmap.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Bitmap OilPaintFilter(this Bitmap sourceBitmap,
            int levels,
            int filterSize)
        {
            var sourceData =
                sourceBitmap.LockBits(new Rectangle(0, 0,
                    sourceBitmap.Width, sourceBitmap.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            var resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            var intensityBin = new int[levels];
            var blueBin = new int[levels];
            var greenBin = new int[levels];
            var redBin = new int[levels];

            levels = levels - 1;

            var filterOffset = (filterSize - 1) / 2;
            var byteOffset = 0;
            var calcOffset = 0;
            var currentIntensity = 0;
            var maxIntensity = 0;
            var maxIndex = 0;

            double blue = 0;
            double green = 0;
            double red = 0;

            for (var offsetY = filterOffset;
                offsetY <
                sourceBitmap.Height - filterOffset;
                offsetY++)
            {
                for (var offsetX = filterOffset;
                    offsetX <
                    sourceBitmap.Width - filterOffset;
                    offsetX++)
                {
                    blue = green = red = 0;

                    currentIntensity = maxIntensity = maxIndex = 0;

                    intensityBin = new int[levels + 1];
                    blueBin = new int[levels + 1];
                    greenBin = new int[levels + 1];
                    redBin = new int[levels + 1];

                    byteOffset = offsetY *
                                 sourceData.Stride + offsetX * 4;

                    for (var filterY = -filterOffset;
                        filterY <= filterOffset;
                        filterY++)
                    {
                        for (var filterX = -filterOffset;
                            filterX <= filterOffset;
                            filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            currentIntensity = (int)Math.Round(((pixelBuffer[calcOffset] +
                                                                 pixelBuffer[calcOffset + 1] +
                                                                 pixelBuffer[calcOffset + 2]) / 3.0 *
                                                                 (levels)) / 255.0);

                            intensityBin[currentIntensity] += 1;
                            blueBin[currentIntensity] += pixelBuffer[calcOffset];
                            greenBin[currentIntensity] += pixelBuffer[calcOffset + 1];
                            redBin[currentIntensity] += pixelBuffer[calcOffset + 2];

                            if (intensityBin[currentIntensity] > maxIntensity)
                            {
                                maxIntensity = intensityBin[currentIntensity];
                                maxIndex = currentIntensity;
                            }
                        }
                    }

                    blue = blueBin[maxIndex] / maxIntensity;
                    green = greenBin[maxIndex] / maxIntensity;
                    red = redBin[maxIndex] / maxIntensity;

                    resultBuffer[byteOffset] = ClipByte(blue);
                    resultBuffer[byteOffset + 1] = ClipByte(green);
                    resultBuffer[byteOffset + 2] = ClipByte(red);
                    resultBuffer[byteOffset + 3] = 255;

                }
            }

            var resultBitmap = new Bitmap(sourceBitmap.Width,
                sourceBitmap.Height);

            var resultData =
                resultBitmap.LockBits(new Rectangle(0, 0,
                    resultBitmap.Width, resultBitmap.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Bitmap GradientBasedEdgeDetectionFilter(
            this Bitmap sourceBitmap,
            byte threshold = 0)
        {
            var sourceData =
                sourceBitmap.LockBits(new Rectangle(0, 0,
                    sourceBitmap.Width, sourceBitmap.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            var resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            int sourceOffset = 0, gradientValue = 0;
            var exceedsThreshold = false;

            for (var offsetY = 1; offsetY < sourceBitmap.Height - 1; offsetY++)
            {
                for (var offsetX = 1; offsetX < sourceBitmap.Width - 1; offsetX++)
                {
                    sourceOffset = offsetY * sourceData.Stride + offsetX * 4;
                    gradientValue = 0;
                    exceedsThreshold = true;

                    // Horizontal Gradient
                    CheckThreshold(pixelBuffer,
                        sourceOffset - 4,
                        sourceOffset + 4,
                        ref gradientValue, threshold, 2);
                    // Vertical Gradient
                    exceedsThreshold =
                        CheckThreshold(pixelBuffer,
                            sourceOffset - sourceData.Stride,
                            sourceOffset + sourceData.Stride,
                            ref gradientValue, threshold, 2);

                    if (exceedsThreshold == false)
                    {
                        gradientValue = 0;

                        // Horizontal Gradient
                        exceedsThreshold =
                            CheckThreshold(pixelBuffer,
                                sourceOffset - 4,
                                sourceOffset + 4,
                                ref gradientValue, threshold);

                        if (exceedsThreshold == false)
                        {
                            gradientValue = 0;

                            // Vertical Gradient
                            exceedsThreshold =
                                CheckThreshold(pixelBuffer,
                                    sourceOffset - sourceData.Stride,
                                    sourceOffset + sourceData.Stride,
                                    ref gradientValue, threshold);

                            if (exceedsThreshold == false)
                            {
                                gradientValue = 0;

                                // Diagonal Gradient : NW-SE
                                CheckThreshold(pixelBuffer,
                                    sourceOffset - 4 - sourceData.Stride,
                                    sourceOffset + 4 + sourceData.Stride,
                                    ref gradientValue, threshold, 2);
                                // Diagonal Gradient : NE-SW
                                exceedsThreshold =
                                    CheckThreshold(pixelBuffer,
                                        sourceOffset - sourceData.Stride + 4,
                                        sourceOffset - 4 + sourceData.Stride,
                                        ref gradientValue, threshold, 2);

                                if (exceedsThreshold == false)
                                {
                                    gradientValue = 0;

                                    // Diagonal Gradient : NW-SE
                                    exceedsThreshold =
                                        CheckThreshold(pixelBuffer,
                                            sourceOffset - 4 - sourceData.Stride,
                                            sourceOffset + 4 + sourceData.Stride,
                                            ref gradientValue, threshold);

                                    if (exceedsThreshold == false)
                                    {
                                        gradientValue = 0;

                                        // Diagonal Gradient : NE-SW
                                        exceedsThreshold =
                                            CheckThreshold(pixelBuffer,
                                                sourceOffset - sourceData.Stride + 4,
                                                sourceOffset + sourceData.Stride - 4,
                                                ref gradientValue, threshold);
                                    }
                                }
                            }
                        }
                    }

                    resultBuffer[sourceOffset] = (byte)(exceedsThreshold ? 255 : 0);
                    resultBuffer[sourceOffset + 1] = resultBuffer[sourceOffset];
                    resultBuffer[sourceOffset + 2] = resultBuffer[sourceOffset];
                    resultBuffer[sourceOffset + 3] = 255;
                }
            }

            var resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            var resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        private static bool CheckThreshold(byte[] pixelBuffer,
            int offset1, int offset2,
            ref int gradientValue,
            byte threshold,
            int divideBy = 1)
        {
            gradientValue +=
                Math.Abs(pixelBuffer[offset1] -
                         pixelBuffer[offset2]) / divideBy;

            gradientValue +=
                Math.Abs(pixelBuffer[offset1 + 1] -
                         pixelBuffer[offset2 + 1]) / divideBy;

            gradientValue +=
                Math.Abs(pixelBuffer[offset1 + 2] -
                         pixelBuffer[offset2 + 2]) / divideBy;

            return (gradientValue >= threshold);
        }

        private static byte ClipByte(double colour)
        {
            return (byte)(colour > 255
                ? 255
                : (colour < 0 ? 0 : colour));
        }
    }
}
