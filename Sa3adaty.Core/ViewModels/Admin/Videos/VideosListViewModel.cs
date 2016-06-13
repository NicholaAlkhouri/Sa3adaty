using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Videos
{
    public class VideosListViewModel
    {
        [UIHint("ItemsList")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
