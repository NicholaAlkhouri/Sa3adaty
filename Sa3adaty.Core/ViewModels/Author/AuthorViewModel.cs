using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Author
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string URL { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string FacebookPage { get; set; }

        public string FacebookId { get; set; }

        public string ImageURL { get; set; }

        public bool IsProfileEnabled { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }
    }
}
