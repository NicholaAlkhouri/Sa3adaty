using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Common.Interfaces
{
    public interface ILinkPagenation :IPagination 
    {
        /// <summary>
        /// The link template to apply to each page
        /// </summary>
        string LinkTemplate { get; }
    }
}
