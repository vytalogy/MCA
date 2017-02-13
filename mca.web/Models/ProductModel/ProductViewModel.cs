using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mca.web.Models
{
    public class ProductViewModel
    {
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string ModelNumber { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CurrentForeCasting { get; set; }
    }
}