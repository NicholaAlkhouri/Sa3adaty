using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.DAL.Repositories
{
    public class ArticleTagsRepository : GenericRepository<ArticleTag>
    {
        public ArticleTagsRepository(Sa3adatyEntities context)
            : base(context)
        {
        }
    }

}
