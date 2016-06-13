using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.Videos
{
    public class VideoViewModel
    {
        [Display(Name = "ID")]
        public int VideoId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Meta Title")]
        public string MetaTitle { get; set; }

        [Display(Name = "Tags")]
        public List<string> Tags { get; set; }

        [Required]
        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [Required]
        [UIHint("ForeignKey")]
        [Display(Name = "Author")]
        public Nullable<int> AuthorId { get; set; }

        [Required]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        [Required]
        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Views")]
        public int CountOfViews { get; set; }

        [Display(Name = "Youtube ID")]
        public string YoutubeId { get; set; }

        [Display(Name = "Duration Minutes")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Duration Seconds")]
        public int DurationSeconds { get; set; }

        [Required]
        [UIHint("ForeignKey")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
