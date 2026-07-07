
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ObsidianArchive.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Category Name")]
        public string Name { get; set; } = string.Empty;
        [Range(0,100, ErrorMessage="Display order must be between 0 and 100.")]
        [DisplayName("Display Order")]
        public int? DisplayOrder { get; set; }
    }
}
