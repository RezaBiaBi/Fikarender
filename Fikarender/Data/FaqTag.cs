using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fikarender.Data
{
    public class FaqTag
    {
        public FaqTag()
        {
        }

        [Key]
        public int Id { get; set; }

        public int FaqId { get; set; }
        public virtual Faq Faq { get; set; }
        
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
