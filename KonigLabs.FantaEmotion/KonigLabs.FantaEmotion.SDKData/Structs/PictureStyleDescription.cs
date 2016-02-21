using System;
using System.Runtime.InteropServices;
using KonigLabs.FantaEmotion.SDKData.Enums;

namespace KonigLabs.FantaEmotion.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PictureStyleDescription
    {
        public int Contrast;
        public UInt32 Sharpness;
        public int Saturation;
        public int ColorTone;
        public MonochromeFilterEffect MonochromeFilterEffect;
        public MonochromeTone MonochromeTone;
    }
}
