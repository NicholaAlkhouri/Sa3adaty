using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sa3adaty.Core.ViewModels
{
    public class CountryViewModel
    {
        public int CountryId { get; set; }

        public string ArabicName { get; set; }

        public string EnglishName { get; set; }

        public string Code { get; set; }

        public bool IsPublished { get; set; }
    }
}
