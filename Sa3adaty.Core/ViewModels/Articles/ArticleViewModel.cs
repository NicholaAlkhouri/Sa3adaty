using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Articles
{
    public class ArticleViewModel
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }

        public string Description { get; set; }

        public int Views { get; set; }

        public DateTime PublishDate { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public List<CommentViewModel> Comments { get; set; }

        public string CategoryName { get; set; }

        public string CategoryURL { get; set; }

        public string URL { get; set; }

        public List<string> Tags { get; set; }

        public AuthorInfoViewModel Author { get; set; }
    }
}
