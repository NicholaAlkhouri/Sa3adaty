using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.DAL.EntityModel;
using Sa3adaty.DAL.Infrastructure;

namespace Sa3adaty.Core.Services
{
    public class LogService
    {
        public enum LogLevels { ERROR, WARNING, INFO };
        #region Privates
        private DataAccessManager DAManager;
        #endregion

        #region Constructor
        public LogService(DataAccessManager unit_of_work)
        {
            DAManager = unit_of_work;
        }
        public LogService()
        {
            DAManager = new DataAccessManager();
        }
        #endregion

        #region Methods
        public void WriteInfo(string message, string exception = "", string stack = "", string source = "")
        {
            if (Convert.ToInt32(ConfigurationManager.AppSettings["LogLevel"]) >= 3)
            {
                WriteLog(LogLevels.INFO, message, exception, stack, source);
            }
        }

        public void WriteWarning(string message, string exception = "", string stack = "", string source = "")
        {
            if (Convert.ToInt32(ConfigurationManager.AppSettings["LogLevel"]) >= 2)
            {
                WriteLog(LogLevels.WARNING, message, exception, stack, source);
            }
        }

        public void WriteError(string message, string exception = "", string stack = "", string source = "")
        {
            if (Convert.ToInt32(ConfigurationManager.AppSettings["LogLevel"]) >= 1)
            {
                WriteLog(LogLevels.ERROR, message, exception, stack, source);
            }
        }

        private void WriteLog(LogLevels level, string message, string exception = "", string stack = "", string source = "")
        {
            Log db_log = new Log() {Exception = exception,Level =level.ToString() ,LogDate = DateTime.Now,Message  = message,Source = source, Stack = stack,  };
            try
            {
                DAManager.LogsRepository.Insert(db_log);
                DAManager.Save();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion 
    }
}
