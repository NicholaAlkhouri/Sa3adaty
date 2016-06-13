using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels.Admin.Tags;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class TagService
    {
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public TagService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

            #region Methods
            public List<ListItemTagViewModel> GetTags(out int total_count, int page = 0, int page_size = 10, string search_key = "", string order_by = "Name", string order_dir = "desc")
            {
                IQueryable<Tag> result;
                
                    result = DAManager.TagsRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.TagName) : a.OrderByDescending(c => c.TagName)));


                if (search_key != "")
                    result = result.Where(tag => tag.TagName.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<Tag> final_res = result.ToList();
                List<ListItemTagViewModel> final_result = new List<ListItemTagViewModel>();
                foreach (Tag tag in final_res.ToList())
                    final_result.Add(new ListItemTagViewModel() {Name =tag.TagName, TagId = tag.TagId,FrontTitle = tag.FrontTitle  });

                return final_result;
            }
           
            public bool IsTagExist(string tag, int tag_id = 0)
            {
                Tag db_tag = DAManager.TagsRepository.Get(t => t.TagName == tag).FirstOrDefault();

                if (db_tag == null || (tag_id != 0 && tag_id == db_tag.TagId))
                    return false;

                return true;
            }

            public TagViewModel  GetTagById(int tag_id)
            {
                var tag = DAManager.TagsRepository.Get(t => t.TagId == tag_id).FirstOrDefault();
                if (tag != null)
                {
                    TagViewModel tag_viewmodel = new TagViewModel() {TagId = tag_id,MetaDescription = tag.MetaDescription,MetaTitle= tag.MetaTitle , TagName = tag.TagName,FrontTitle = tag.FrontTitle, FrontDescription = tag.FrontDescription };


                    return tag_viewmodel;
                }
                else
                    return null;
            }

            public int AddNewTag(TagViewModel tag)
            {
                Tag db_tag = new Tag() {MetaDescription = tag.MetaDescription, TagName = tag.TagName,MetaTitle = tag.MetaTitle,FrontTitle = tag.FrontTitle, FrontDescription = tag.FrontDescription };

                DAManager.TagsRepository.Insert(db_tag);

                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }

                return db_tag.TagId;
            }
            

            public int UpdateTag(TagViewModel tag)
            {
                Tag old_tag = DAManager.TagsRepository.Get(t => t.TagId == tag.TagId).FirstOrDefault();

                if (old_tag != null)
                {
                    old_tag.MetaDescription = tag.MetaDescription;
                    old_tag.MetaTitle = tag.MetaTitle;
                    old_tag.TagId = tag.TagId;
                    old_tag.TagName = tag.TagName;
                    old_tag.FrontTitle = tag.FrontTitle;
                    old_tag.FrontDescription = tag.FrontDescription;
                }
                else
                    return -1;
                try
                {
                    DAManager.Save();
                    return old_tag.TagId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }

            public List<string> GetAvailableTags()
            {
               return  DAManager.TagsRepository.Get().Select(t => t.TagName).ToList();
            }
            #endregion
    }
}
