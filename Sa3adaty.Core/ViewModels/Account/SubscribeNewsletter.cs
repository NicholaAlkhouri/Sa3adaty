using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Account
{
    public class SubscribeNewsletter
    {
        [EmailAddress]
        [Required]
        public string email { get; set; }

        public int? user_id { get; set; }
    }
}
