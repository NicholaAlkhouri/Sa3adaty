using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Sa3adaty.Helpers
{
    public static class FBHelper
    {
        private static string access_token = null;
        private static string app_id = null;
        private static  string app_secret = null;

        public static string AccessToken
        {
            get {
                if (access_token == null)
                    GetAccessToken();
                
                return access_token;
            }
        }

        public static string AppId
        {
            get
            {
                if (app_id == null)
                {
                    app_id = System.Configuration.ConfigurationManager.AppSettings["FacebookAppId"];
                }

                return app_id;
            }
        }

        public static string AppSecret
        {
            get
            {
                if (app_secret == null)
                {
                    app_secret = System.Configuration.ConfigurationManager.AppSettings["FacebookAppSecret"];
                }

                return app_secret;
            }
        }

        public static  void GetAccessToken()
        {
            Uri targetUri = new Uri("https://graph.facebook.com/v2.2/oauth/access_token?client_id=" + AppId + "&client_secret=" + AppSecret + "&grant_type=client_credentials");
            
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(targetUri);
            request.ContentType = "application/json; charset=utf-8";
           

            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string objText = reader.ReadToEnd();
                   access_token = objText.Split('=')[1]; 
                }
            }
            catch(Exception ex)
            {
            }
        }
    }
}