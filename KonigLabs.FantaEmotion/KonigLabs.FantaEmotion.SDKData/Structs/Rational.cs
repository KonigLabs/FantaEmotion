using System;
using System.Runtime.InteropServices;

namespace KonigLabs.FantaEmotion.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rational
    {
        public int Numerator;
        public UInt32 Denominator;
    }
}
