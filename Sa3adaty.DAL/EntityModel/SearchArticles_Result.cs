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
    
    public partial class SearchArticles_Result
    {
        public Nullable<long> RowNum { get; set; }
        public int KEY { get; set; }
        public Nullable<int> RANK { get; set; }
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public Nullable<int> AuthorId { get; set; }
        public bool IsPublished { get; set; }
        public System.DateTime AddDate { get; set; }
        public System.DateTime PublishDate { get; set; }
        public int NumberOfViews { get; set; }
        public string URL { get; set; }
        public string ImageURL { get; set; }
    }
}
