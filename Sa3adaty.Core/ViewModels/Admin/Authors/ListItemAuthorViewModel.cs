using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Authors
{
    public class ListItemAuthorViewModel
    {
        [Display(Name = "ID")]
        public int AothorId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
