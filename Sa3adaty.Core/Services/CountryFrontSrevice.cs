using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class CountryFrontSrevice
    {
         #region Privates
            private DataAccessManager DAManager;
        #endregion

        #region Constructor
            public CountryFrontSrevice(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
            }
        #endregion

        #region Methods
            public List<CountryViewModel> GetOnlineCountries()
            {
                List<CountryViewModel> result = new List<CountryViewModel>();
                result = DAManager.CountriesRepository.Get(c => c.IsPublished == true).Select(c => new CountryViewModel()
                {ArabicName = c.ArabicName,
                 CountryId = c.CountryId,
                 Code = c.Code,
                 IsPublished = c.IsPublished 
                }).ToList();

                return result;
            }
        #endregion 
    }
}
