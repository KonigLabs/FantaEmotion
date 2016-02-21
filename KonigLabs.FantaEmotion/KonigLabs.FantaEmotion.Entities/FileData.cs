using System.ComponentModel.DataAnnotations.Schema;

namespace KonigLabs.FantaEmotion.Entities
{
    public class FileData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public byte[] Data { get; set; }
    }
}
