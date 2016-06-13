using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels.Admin.User
{
    public  class UserViewModel
    {
        [Display(Name = "UserID")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [CompareAttribute("Password", ErrorMessage = "Password and confirmation mismatch")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Date of Birth")]
        public Nullable<DateTime> DateOfBirth { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public  string  LastName { get; set; }

        [Display(Name = "Gender")]
        public bool? Gender { get; set; }

        [Display(Name = "Date Of Registration")]
        public Nullable<DateTime> RegistrationDate { get; set; }

        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

    }
}
