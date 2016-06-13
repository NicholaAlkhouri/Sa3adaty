using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Sa3adaty.Helpers
{
    public static class FrontHelpers
    {
        #region Privates
        private static string host;
        private static string images_path;
        private static string[] months = { "كانون الثاني", "شباط", "آذار", "نيسان", "أيار", "حزيران", "تموز", "آب", "أيلول", "تشرين الأول", "تشرين الثاني", "كانون الأول" };
        private static string[] week_days = { "أحد", "اثنين", "ثلاثاء", "أربعاء", "خميس", "جمعة", "سبت" };
        #endregion 
        
        #region Properties
        private static string Host
        {
            get
            {
                if (host == null || host == "")
                    host = ConfigurationManager.AppSettings["host"];
                return host;
            }
            set { host = value; }
        }
        private static string ImagesPath
        {
            get
            {
                if (images_path == null || host == "")
                    images_path = ConfigurationManager.AppSettings["images_path"];
                return images_path;
            }
            set { images_path = value; }
        }
        #endregion

        #region HTML Helpers
        public static string AbsoluteAction(this UrlHelper url,string actionName, string controllerName, object routeValues = null)
        {
            string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;

            return url.Action(actionName, controllerName, routeValues, scheme);
        }

        public static string AbsoluteArabicURL(this HtmlHelper  helper, string custom_url)
        {
            return HttpContext.Current.Server.UrlDecode(Host + custom_url);
        }
        public static string AbsoluteURL(this HtmlHelper helper, string custom_url)
        {
            return Host + custom_url;
        }

        public static string AbsoluteEncodedImageURL(this HtmlHelper helper, string custom_url)
        {
            string c = HttpContext.Current.Server.UrlDecode(Host + custom_url);

            string result = Path.GetDirectoryName(c) + "/" + HttpContext.Current.Server.UrlEncode(Path.GetFileName(c));
            return result;
        }

        public static string AbsoluteImagePath(this HtmlHelper helper, string custom_url)
        {
            return HttpContext.Current.Server.UrlDecode(ImagesPath + custom_url);
        }

        public static string EnglishDate(this HtmlHelper helper, DateTime date)
        {
            return date.ToString();
        }

        public static string ArabicDateTime(this HtmlHelper helper, DateTime date)
        {
            return ChangeToArabic(date.Day.ToString() + " " +  ArabicMonth(date.Month) + " " + date.Year.ToString() + " " + date.Hour.ToString() + ":" +date.Minute.ToString() ); 
        }

        public static string ArabicDate(this HtmlHelper helper, DateTime date)
        {
            return ChangeToArabic(date.Day.ToString() + " " + ArabicMonth(date.Month) + " " + date.Year.ToString() );
        }

        public static string FormatDateTime(this HtmlHelper helper, DateTime date)
        {
            return date.ToString("MMMM dd,yyyy hh:mm tt");
        }

        public static string FormatDate(this HtmlHelper helper, DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string ArabicNumbers(this HtmlHelper helper, string  text)
        {
            return ChangeToArabic(text);
        }

        public static string FormatNumbers(this HtmlHelper helper, string text)
        {
            //maybe later we can change all numbers to arabic from here
            return text;
        }

        //Replace HashTags with link to the category page
        public static string ReplaceHashTags(this HtmlHelper helper, string text)
        {
            if (text == null)
                return "";
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.Action("Home", "Index");
           MatchCollection hashtag_matches =  Regex.Matches(text, "#(\\w)+");
           foreach (Match hash_match in hashtag_matches)
           {
               string clean_tag = hash_match.Value.Replace("#","").Replace("_"," ");

               //text = text.Replace(hash_match.Value, clean_tag);
               //to be changed when go with tags on front end
               text = text.Replace(hash_match.Value, "<a href='" + AbsoluteArabicURL(helper,urlHelper.Action("ListByTag", "Category", new { id = clean_tag.Replace(" ","-") })) + "'>" + clean_tag + "</a>");
           }
            //Regex.Matches(text,"#+(\w)+"); 
            //maybe later we can change all numbers to arabic from here
            return text;
        }

        public static string ArabicMonth(int month_number)
        {
            if (month_number < 1 || month_number > 12)
                return months[0];

            return months[month_number - 1];
        }

        public static string ArabicWeekDay(int day_number)
        {
            if (day_number < 1 || day_number > 12)
                return week_days[0];

            return week_days[day_number - 1];
        }

        public static string ChangeToArabic(this string input)
        {
            return input.Replace('0', '\u0660')
            .Replace('1', '\u0661')
            .Replace('2', '\u0662')
            .Replace('3', '\u0663')
            .Replace('4', '\u0664')
            .Replace('5', '\u0665')
            .Replace('6', '\u0666')
            .Replace('7', '\u0667')
            .Replace('8', '\u0668')
            .Replace('9', '\u0669');
           
        }

        #endregion 

        #region page title
        public static string PageTitle(this HtmlHelper helper, string title)
        {
            return title == "" ? "سعادتي، موقع مختص بالمرأة والعائلة والمجتمع" : title + " - سعادتي";
        }
        #endregion 

    }
}