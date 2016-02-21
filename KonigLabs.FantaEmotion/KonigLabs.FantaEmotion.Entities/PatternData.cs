using System.ComponentModel.DataAnnotations.Schema;

namespace KonigLabs.FantaEmotion.Entities
{
    public class PatternData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public PatternType PatternType { get; set; }

        public string Name  { get; set; }

        public byte[] Data { get; set; }
    }
}
