using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Articles
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        public int? ParentId { get; set; }

        public string Username { get; set; }

        public string UserImageURL { get; set; }

        public string Text { get; set; }

        public DateTime  AddedDate { get; set; }

        public List<CommentViewModel> Replies { get; set; }

    }
}
