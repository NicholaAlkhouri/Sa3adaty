using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sa3adaty.Areas.Admin.Models
{
    public class Navigator
    {
        public enum Items { NONE,ARTICLES, NEWARTICLE, LISTARTICLES,
            COMMENTSARTICLE,NEWCATEGORY, LISTCATEGORIES, CATEGORIES,
            USERS, NEWUSER,LISTUSERS,
            QUOTES,NEWQUOTE,LISTQUOTES,
            AUTHORS,NEWAUTHOR ,LISTAUTHOR,
            LISTTIPS,NEWTIP,TIPS,
            LISTTAG, NEWTAG, TAGS,
            LISTPOLL,NEWPOLL,POLLS,
            LISTVIDEOTAG,NEWVIDEOTAG,VIDEOTAGS,
            VIDEOS, NEWVIDEO, LISTVIDEOS, COMMENTSVIDEOS,
            NEWVIDEOCATEGORY, VIDEOCATEGORIES, LISTVIDEOCATEGORIES
        }
    }
}