﻿using System;
using System.Collections.Generic;

namespace MVCForum.Website.ViewModels
{
    public class FavouritePostViewModel
    {
        public Guid PostId { get; set; }
    }

    public class MyFavouritesViewModel
    {
        public MyFavouritesViewModel()
        {
            Posts = new List<PostViewModel>();
        }
        public List<PostViewModel> Posts { get; set; }
    }
}