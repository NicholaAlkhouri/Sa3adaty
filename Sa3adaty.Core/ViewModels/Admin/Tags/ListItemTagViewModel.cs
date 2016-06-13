using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Tags
{
    public class ListItemTagViewModel
    {
        public int TagId { get; set; }

        public string Name { get; set; }

        public string FrontTitle { get; set; }

        public int ArticlesCount { get; set; }
    }
}
