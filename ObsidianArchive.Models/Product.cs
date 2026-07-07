using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ObsidianArchive.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name= "List Price")]
        public string ListPrice { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name="Price for 1-50")]
        public double Price { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name="Price for 50+")]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 1000)]
        [Display(Name = "Price for 100+")]
        public double Price100 { get; set; }
        [Display(Name="Product Image")]
        public string? ImageUrl { get; set; }
    }
}
