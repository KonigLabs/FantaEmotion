using System.ComponentModel.DataAnnotations.Schema;

namespace KonigLabs.FantaEmotion.Entities
{
    public class Composition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual Template Template { get; set; }

        //public virtual Image Background { get; set; }

        //public virtual Image Overlay { get; set; }

        public int TemplateId { get; set; }

        //public int? BackgroundId { get; set; }

        //public int? OverlayId { get; set; }
    }
}
