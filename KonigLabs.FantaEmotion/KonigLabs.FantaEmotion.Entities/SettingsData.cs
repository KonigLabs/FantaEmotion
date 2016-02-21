using System.ComponentModel.DataAnnotations.Schema;

namespace KonigLabs.FantaEmotion.Entities
{
    public class SettingsData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public byte[] Data { get; set; }
    }
}
