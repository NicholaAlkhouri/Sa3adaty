using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin
{
    public class ImageViewModel
    {
        public int ImageId { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string ThumnailURL { get; set; }
    }
}
