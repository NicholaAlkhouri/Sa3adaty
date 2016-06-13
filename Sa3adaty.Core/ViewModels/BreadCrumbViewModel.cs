using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels
{
    public class BreadCrumbViewModel
    {
        public List<BreadCrumbItemViewModel> Items { get; set; }
    }

    public class BreadCrumbItemViewModel
    {
        public string Text { get; set; }

        public string URL { get; set; }
    }
}
