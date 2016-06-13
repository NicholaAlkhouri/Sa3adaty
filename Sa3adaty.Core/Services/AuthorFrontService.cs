using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels.Author;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class AuthorFrontService
    {
        
        #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public AuthorFrontService(DataAccessManager unit_of_work)
            {
                DAManager = unit_of_work;
                logService = new LogService(unit_of_work);
            }
        #endregion

        #region Methods
            public AuthorViewModel GetAuthorByURL(string url)
            {
                Author DB_author;

                DB_author = DAManager.AuthorsRepository.Get(a => a.URL.Trim() == url.Trim() , null, "AuthorImages.Image").FirstOrDefault();

                if (DB_author == null)
                    return null;



                AuthorViewModel author_viewModel = new AuthorViewModel()
                {
                    AuthorId = DB_author.AuthorId,
                    Description = DB_author.Description,
                    Email = DB_author.Email,
                    FacebookPage = DB_author.FacebookPage,
                    Name = DB_author.Name,
                    Title = DB_author.Title,
                    URL = DB_author.URL ,
                    IsProfileEnabled = DB_author.IsProfileEnabled??false ,
                    MetaDescription = DB_author.MetaDescription,
                    MetaTitle = DB_author.MetaTitle,
                    FacebookId = DB_author.FacebookId
                };

                if (DB_author.AuthorImages.Count() > 0)
                    author_viewModel.ImageURL = ImageService.GenerateImageFullPath(DB_author.AuthorImages.First().Image.URL, AuthorService.AuthorThumbWidth1.ToString(), AuthorService.AuthorThumbHeight1.ToString());



                return author_viewModel;
            }
        #endregion
    }
}
