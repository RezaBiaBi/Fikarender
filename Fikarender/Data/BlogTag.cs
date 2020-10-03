using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class BlogTag
    {
        [Key]
        public int Id { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
