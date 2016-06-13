using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.Core.ViewModels.Admin.Quotes;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class QuoteService
    {
         #region Privates
            private DataAccessManager DAManager;
            private LogService logService;
        #endregion

        #region Constructor
            public QuoteService(DataAccessManager unit_of_work)
        {
            DAManager = unit_of_work;
            logService = new LogService(unit_of_work);
        }
        #endregion

            public List<QuoteViewModel> GetQuotes(out int total_count, int page = 0, int page_size = 10, string search_key = "", string order_by = "DayDate", string order_dir = "desc")
            {
                IQueryable<DailyQuote> result;
                if (order_by == "Author")
                    result = DAManager.QuotesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Author) : a.OrderByDescending(c => c.Author)));
                else if(order_by == "Quote")
                    result = DAManager.QuotesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.Quote) : a.OrderByDescending(c => c.Quote)));
                else
                    result = DAManager.QuotesRepository.Get(null, a => (order_dir == "asc" ? a.OrderBy(c => c.DayDate) : a.OrderByDescending(c => c.DayDate)));


                if (search_key != "")
                    result = result.Where(cat => cat.Quote.Contains(search_key));

                total_count = result.Count();
                result = result.Skip(page).Take(page_size);
                List<DailyQuote> final_res = result.ToList();
                List<QuoteViewModel> final_result = new List<QuoteViewModel>();
                foreach (DailyQuote quote in final_res)
                    final_result.Add(new QuoteViewModel() { Author = quote.Author, DayDateString=quote.DayDate.ToString(), Quote= quote.Quote, QuoteId = quote.QuoteId });

                return final_result;
            }
            
            public int AddNewQuote(QuoteViewModel  quote)
            {
                DailyQuote DBQuote = new DailyQuote() { Author = quote.Author, DayDate = quote.DayDate ,Quote = quote.Quote};

                DAManager.QuotesRepository.Insert(DBQuote);

                try
                {
                    DAManager.Save();
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }


                return DBQuote.QuoteId ;
            }


            public QuoteViewModel  GetQuoteById(int quote_id)
            {
                var quote = DAManager.QuotesRepository.Get(q => q.QuoteId == quote_id).FirstOrDefault();
                if (quote != null)
                {
                    QuoteViewModel quote_viewmodel = new QuoteViewModel() {Author = quote.Author,DayDate = quote.DayDate,Quote = quote.Quote,QuoteId = quote.QuoteId  };

                    return quote_viewmodel;
                }
                else
                    return null;
            }

            public int UpdateQuote(QuoteViewModel quote)
            {
                DailyQuote  DBQuote = DAManager.QuotesRepository.Get(q => q.QuoteId == quote.QuoteId).FirstOrDefault();

                if (DBQuote != null)
                {
                    DBQuote.Quote = quote.Quote;
                    DBQuote.Author = quote.Author;
                    DBQuote.DayDate = quote.DayDate;
                }
                else
                    return -1;
                try
                {
                    DAManager.Save();
                    return DBQuote.QuoteId;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return -1;
                }
            }

            public bool DeleteQuote(int quote_id)
            {
                DAManager.QuotesRepository.Delete(quote_id);

                try
                {
                    DAManager.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    logService.WriteError(ex.Message, ex.Message, ex.StackTrace, ex.Source);
                    return false;
                }
            }

            public QuoteViewModel GetTodayQuote()
            {
                DateTime today = DateTime.Now;
                return DAManager.QuotesRepository.Get(c => (c.DayDate.Year == today.Year && c.DayDate.Month == today.Month && c.DayDate.Day <= today.Day) || (c.DayDate.Year == today.Year && c.DayDate.Month < today.Month) || c.DayDate.Year < today.Year).OrderByDescending(c => c.DayDate).Take(1).Select(c=> new QuoteViewModel (){Quote = c.Quote , Author= c.Author}).FirstOrDefault();

            }
    }
}
