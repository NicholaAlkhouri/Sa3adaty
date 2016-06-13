using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Articles
{
    public class ListArticleViewModel
    {
        public string Title { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }

        public string URL { get; set; }

        public DateTime PublishDate { get; set; }

        public int ArticleId {get;set;}

    }
}
