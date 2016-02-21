using System;
using System.Runtime.InteropServices;
using KonigLabs.FantaEmotion.SDKData.Miscellaneous;

namespace KonigLabs.FantaEmotion.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInformation
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GlobalConstants.MAX_NAME)]
        public string PortName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GlobalConstants.MAX_NAME)]
        public string DeviceDescription;

        public UInt32 DeviceSubType;

        public UInt32 Reserved;
    }
}
