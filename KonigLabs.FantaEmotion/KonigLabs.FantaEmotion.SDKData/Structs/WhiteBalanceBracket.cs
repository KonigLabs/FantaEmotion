using System.Runtime.InteropServices;
using KonigLabs.FantaEmotion.SDKData.Enums;

namespace KonigLabs.FantaEmotion.SDKData.Structs
{
    /// <summary>
    /// Indicates the white balance bracket amount.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WhiteBalanceBracket
    {
        BracketMode BracketMode;
        WhiteBalanceShift WhiteBalanceShift;
    }
}
