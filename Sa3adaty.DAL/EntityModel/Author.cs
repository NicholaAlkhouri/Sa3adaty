//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sa3adaty.DAL.EntityModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Author
    {
        public Author()
        {
            this.Articles = new HashSet<Article>();
            this.AuthorImages = new HashSet<AuthorImage>();
            this.Videos = new HashSet<Video>();
        }
    
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public System.DateTime AddDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Description { get; set; }
        public string FacebookPage { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string DisplayName { get; set; }
        public Nullable<bool> IsProfileEnabled { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string FacebookId { get; set; }
    
        public virtual ICollection<Article> Articles { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<AuthorImage> AuthorImages { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}
