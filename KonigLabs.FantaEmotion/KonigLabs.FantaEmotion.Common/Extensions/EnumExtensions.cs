using System.ComponentModel;

namespace KonigLabs.FantaEmotion.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this object value)
        {
            var attributes =
                (DescriptionAttribute[])value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
