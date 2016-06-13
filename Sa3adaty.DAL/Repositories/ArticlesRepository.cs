using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.DAL.Repositories
{
    public class ArticlesRepository  :GenericRepository<Article>
    {
        public ArticlesRepository(Sa3adatyEntities context)
            : base(context)
        {
        }

        public List<SearchArticles_Result> SearchArticles(string query, int page, int page_size,string except_ids_list= "")
        {
            return context.SearchArticles(query, page, page_size,  except_ids_list).ToList();
        }
    }
}
