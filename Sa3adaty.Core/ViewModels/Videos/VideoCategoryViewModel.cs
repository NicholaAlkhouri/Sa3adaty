﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Common.Interfaces;

namespace Sa3adaty.Core.ViewModels.Videos
{
    public class VideoCategoryViewModel : ILinkPagenation
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public List<ListVideoViewModel> Videos { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public string URL { get; set; }

        public string LinkTemplate { get; set; }

        public int PageNumber
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public int TotalItems
        {
            get;
            set;
        }

        public int TotalPages
        {
            get { return (int)(Math.Ceiling((decimal)TotalItems / (decimal)PageSize)); }
        }

        public int FirstItem
        {
            get { return ((PageNumber - 1) * PageSize) + 1; }
        }

        public int LastItem
        {
            get
            {
                if (PageNumber < TotalPages)
                {
                    return PageNumber * PageSize;
                }
                else
                {
                    return TotalItems;
                }
            }
        }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool HasNextPage
        {
            get { return PageNumber < TotalPages; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.Videos.GetEnumerator();
        }
    }

}
