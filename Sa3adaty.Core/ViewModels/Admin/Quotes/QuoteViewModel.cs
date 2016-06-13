using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Quotes
{
    public class QuoteViewModel
    {
        [Display(Name = "ID")]
        public int QuoteId { get; set; }

        [Required]
        [Display(Name = "Quote")]
        public string Quote { get; set; }

        [Display(Name = "Author")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Display On")]
        public DateTime DayDate { get; set; }

        [Display(Name = "Display On")]
        public string  DayDateString { get; set; }
    }
}
