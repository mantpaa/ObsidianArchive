
using System.ComponentModel.DataAnnotations;

namespace ObsidianArchive.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
        [Range(0,100, ErrorMessage="Display order must be between 0 and 100.")]
        [Display(Name = "Display Order")]
        [validatenever]
        public int? DisplayOrder { get; set; }
    }
}
