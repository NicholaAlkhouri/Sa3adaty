using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.User
{
    public class SubscriptionViewModel
    {
        public int? UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string UnsubscriptionToken { get; set; }

    }
}
