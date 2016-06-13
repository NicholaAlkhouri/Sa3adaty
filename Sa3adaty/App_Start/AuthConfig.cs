using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using GooglePlusOAuthLogin;
using Microsoft.Web.WebPages.OAuth;

namespace Sa3adaty
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            string FacebookAppId = ConfigurationManager.AppSettings["FacebookAppId"];
            string FacebookAppSecret = ConfigurationManager.AppSettings["FacebookAppSecret"];
            string GPlusClientID = ConfigurationManager.AppSettings["GPlusClientID"];
            string GPlustClientSecret = ConfigurationManager.AppSettings["GPlustClientSecret"];

            OAuthWebSecurity.RegisterFacebookClient(
                appId: FacebookAppId,
                appSecret: FacebookAppSecret);

            OAuthWebSecurity.RegisterClient(new GooglePlusClient(GPlusClientID, GPlustClientSecret), "Google+", null);
            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
