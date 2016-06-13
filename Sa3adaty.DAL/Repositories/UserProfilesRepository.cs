
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
    public class UserProfilesRepository :GenericRepository<UserProfile>
    {  
        public UserProfilesRepository(Sa3adatyEntities context):base(context)
        {
        }

    }
}
