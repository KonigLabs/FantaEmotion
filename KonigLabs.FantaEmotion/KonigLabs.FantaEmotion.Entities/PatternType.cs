using System.ComponentModel;

namespace KonigLabs.FantaEmotion.Entities
{
    public enum PatternType
    {
        [Description("Simple")]
        Simple = 0,

        [Description("3_in_1")]
        ThreeInOne = 1,

        [Description("Stripe")]
        Stripe = 2
    }
}
