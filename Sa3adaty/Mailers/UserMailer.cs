using System.Collections.Generic;
using Mvc.Mailer;
using Sa3adaty.Core.ViewModels.Articles;

namespace Sa3adaty.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_EmailLayout";
		}
		
		public virtual MvcMailMessage Welcome()
		{
            MasterName = "_EmailLayout";
			//ViewBag.Data = someObject;
			return Populate(x =>
			{
				x.Subject = "Welcome";
				x.ViewName = "Welcome";
				x.To.Add("some-email@example.com");
			});
		}
 
		public virtual MvcMailMessage PasswordReset(string to_email,string reset_link,string username)
		{
            MasterName = "_EmailLayout";
            ViewBag.ResetLink = reset_link;
            ViewBag.Username = username;
			return Populate(x =>
			{
				x.Subject = "استعادة كلمة سرك على سعادتي";
				x.ViewName = "PasswordReset";
                x.To.Add(to_email);
                x.From = new System.Net.Mail.MailAddress("info@sa3adaty.com", "موقع سعادتي");
                
			});
		}

        public virtual MvcMailMessage SendEmail(string to_email, string view_name,List<ListArticleViewModel> articles,string unsubscription_token)
        {
            MasterName = "_NewsletterLayout";
            ViewBag.Articles = articles;
            ViewBag.UnsubscriptionToken = unsubscription_token;
            ViewBag.Email = to_email;
            string subject = articles[0].Title;
            return Populate(x =>
            {
                x.Subject = subject;
                x.ViewName = view_name;
                x.To.Add(to_email);
                x.From = new System.Net.Mail.MailAddress("info@sa3adaty.com", "موقع سعادتي");
            });
        }
       
 	}
}