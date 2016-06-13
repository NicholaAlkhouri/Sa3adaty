using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.DAL.Repositories
{
    public class AuthorImagesRepository : GenericRepository<AuthorImage>
    {

        public AuthorImagesRepository(Sa3adatyEntities context)
            : base(context)
        {
        }
    }
}
