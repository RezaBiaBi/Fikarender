using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class FaqTag
    {
        [Key]
        public int Id { get; set; }

        public int FaqId { get; set; }
        public virtual Faq Faq { get; set; }
        
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
