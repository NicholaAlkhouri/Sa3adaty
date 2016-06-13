using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Account
{
    public class ManageAccountViewModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        [Required]
        public string Name { get; set; }


        public string Image { get; set; }

        public int Country { get; set; }

        public int Gender { get; set; }

        public bool Subscribe { get; set; }

        public LocalPasswordModel LocalPasswordModel { get; set; }
    }
}
