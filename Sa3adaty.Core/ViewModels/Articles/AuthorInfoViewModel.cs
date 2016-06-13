using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Articles
{
    public class AuthorInfoViewModel
    {
        public string FullName { get; set; }

        public string ImageURL { get; set; }

        public int AuthorId { get; set; }

        public bool IsProfileEnabled { get; set; }

        public string URL { get; set; }
    }
}
