﻿using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Data.Mapping
{
    public class CategoryNotificationMapping : EntityTypeConfiguration<CategoryNotification>
    {
        public CategoryNotificationMapping()
        {
            HasKey(x => x.Id);

            HasRequired(x => x.Category)
                .WithMany(x => x.CategoryNotifications)
                .Map(x => x.MapKey("Category_Id"));

            HasRequired(x => x.User).WithMany(x => x.CategoryNotifications).Map(x => x.MapKey("UserId"));

        }
    }
}
