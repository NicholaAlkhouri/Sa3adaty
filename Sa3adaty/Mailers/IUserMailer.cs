using System.Collections.Generic;
using Mvc.Mailer;
using Sa3adaty.Core.ViewModels.Articles;

namespace Sa3adaty.Mailers
{ 
    public interface IUserMailer
    {
			MvcMailMessage Welcome();
            MvcMailMessage PasswordReset(string to_email, string reset_link, string username);
	}
}