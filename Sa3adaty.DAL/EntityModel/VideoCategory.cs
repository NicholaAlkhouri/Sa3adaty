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
    
    public partial class VideoCategory
    {
        public VideoCategory()
        {
            this.Videos = new HashSet<Video>();
            this.VideoCategory1 = new HashSet<VideoCategory>();
        }
    
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentCategoryId { get; set; }
        public bool IsPublished { get; set; }
        public System.DateTime AddDate { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string URL { get; set; }
        public string ImageURL { get; set; }
        public Nullable<int> ImageId { get; set; }
    
        public virtual Image Image { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<VideoCategory> VideoCategory1 { get; set; }
        public virtual VideoCategory VideoCategory2 { get; set; }
    }
}
