using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.User
{
    public class ListItemUserViewModel
    {
        [Display(Name = "ID")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Date of Registration")]
        public string RegistrationDate { get; set; }
    }
}
