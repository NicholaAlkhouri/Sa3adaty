﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Website.Areas.Admin.ViewModels
{
    public class ListLogViewModel
    {
        public IList<LogEntry> LogFiles { get; set; }
    }
}