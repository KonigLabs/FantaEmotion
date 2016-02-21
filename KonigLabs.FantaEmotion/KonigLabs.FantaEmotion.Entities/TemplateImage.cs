using System.ComponentModel.DataAnnotations.Schema;

namespace KonigLabs.FantaEmotion.Entities
{
    public class TemplateImage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public int TemplateId { get; set; }
    }
}
