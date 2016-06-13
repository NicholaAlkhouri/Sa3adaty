using Sa3adaty.DAL.EntityModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Threading;
using System.Text;

namespace Sa3adaty.DAL.Infrastructure
{
    /// <summary>
    /// Manages the lifecycle of the EF's object context
    /// </summary>
    /// <remarks>Uses a context per http request approach or one per thread in non web applications</remarks>
    public static class ContextManager
    {
        #region Private Members

        // accessed via lock(_threadObjectContexts), only required for multi threaded non web applications
        private static readonly Hashtable _threadObjectContexts = new Hashtable();

        #endregion

        public static DbSet<T> GetDBSet<T>(T entity, string contextKey)
            where T : class
        {
            return GetDBContext(contextKey).Set<T>();
        }

        /// <summary>
        /// Returns the active object context
        /// </summary>
        public static DbContext GetDBContext(string contextKey)
        {
            DbContext dbContext = GetCurrentObjectContext(contextKey);
            if (dbContext == null) // create and store the object context
            {
                dbContext = new Sa3adatyEntities();
                StoreCurrentObjectContext(dbContext, contextKey);
            }
            return dbContext;
        }

        ///// <summary>
        ///// Gets the repository context
        ///// </summary>
        ///// <returns>An object representing the repository context</returns>
        //public static object GetRepositoryContext(string contextKey)
        //{
        //    return GetDBContext(contextKey);
        //}

        ///// <summary>
        ///// Sets the repository context
        ///// </summary>
        ///// <param name="repositoryContext">An object representing the repository context</param>
        //public static void SetRepositoryContext(object repositoryContext, string contextKey)
        //{
        //    if (repositoryContext == null)
        //    {
        //        RemoveCurrentObjectContext(contextKey);
        //    }
        //    else if (repositoryContext is DbContext)
        //    {
        //        StoreCurrentObjectContext((DbContext)repositoryContext, contextKey);
        //    }
        //}


        #region Object Context Lifecycle Management

        /// <summary>
        /// gets the current object context 		
        /// </summary>
        private static DbContext  GetCurrentObjectContext(string contextKey)
        {
            DbContext objectContext = null;
            if (HttpContext.Current == null)
                objectContext = GetCurrentThreadObjectContext(contextKey);
            else
                objectContext = GetCurrentHttpContextObjectContext(contextKey);
            return objectContext;
        }

        /// <summary>
        /// sets the current session 		
        /// </summary>
        private static void StoreCurrentObjectContext(DbContext objectContext, string contextKey)
        {
            if (HttpContext.Current == null)
                StoreCurrentThreadObjectContext(objectContext, contextKey);
            else
                StoreCurrentHttpContextObjectContext(objectContext, contextKey);
        }

        /// <summary>
        /// remove current object context 		
        /// </summary>
        private static void RemoveCurrentObjectContext(string contextKey)
        {
            if (HttpContext.Current == null)
                RemoveCurrentThreadObjectContext(contextKey);
            else
                RemoveCurrentHttpContextObjectContext(contextKey);
        }

        #region private methods - HttpContext related

        /// <summary>
        /// gets the object context for the current thread
        /// </summary>
        private static DbContext GetCurrentHttpContextObjectContext(string contextKey)
        {
            DbContext objectContext = null;
            if (HttpContext.Current.Items.Contains(contextKey))
                objectContext = (DbContext)HttpContext.Current.Items[contextKey];
            return objectContext;
        }

        private static void StoreCurrentHttpContextObjectContext(DbContext objectContext, string contextKey)
        {
            if (HttpContext.Current.Items.Contains(contextKey))
                HttpContext.Current.Items[contextKey] = objectContext;
            else
                HttpContext.Current.Items.Add(contextKey, objectContext);
        }

        /// <summary>
        /// remove the session for the currennt HttpContext
        /// </summary>
        private static void RemoveCurrentHttpContextObjectContext(string contextKey)
        {
            DbContext objectContext = GetCurrentHttpContextObjectContext(contextKey);
            if (objectContext != null)
            {
                HttpContext.Current.Items.Remove(contextKey);
                objectContext.Dispose();
            }
        }

        #endregion

        #region private methods - ThreadContext related

        /// <summary>
        /// gets the session for the current thread
        /// </summary>
        private static DbContext GetCurrentThreadObjectContext(string contextKey)
        {
            DbContext objectContext = null;
            Thread threadCurrent = Thread.CurrentThread;
            if (threadCurrent.Name == null)
                threadCurrent.Name = contextKey;
            else
            {
                object threadObjectContext = null;
                lock (_threadObjectContexts.SyncRoot)
                {
                    threadObjectContext = _threadObjectContexts[contextKey];
                }
                if (threadObjectContext != null)
                    objectContext = (DbContext)threadObjectContext;
            }
            return objectContext;
        }

        private static void StoreCurrentThreadObjectContext(DbContext objectContext, string contextKey)
        {
            lock (_threadObjectContexts.SyncRoot)
            {
                if (_threadObjectContexts.Contains(contextKey))
                    _threadObjectContexts[contextKey] = objectContext;
                else
                    _threadObjectContexts.Add(contextKey, objectContext);
            }
        }

        private static void RemoveCurrentThreadObjectContext(string contextKey)
        {
            lock (_threadObjectContexts.SyncRoot)
            {
                if (_threadObjectContexts.Contains(contextKey))
                {
                    DbContext objectContext = (DbContext)_threadObjectContexts[contextKey];
                    if (objectContext != null)
                    {
                        objectContext.Dispose();
                    }
                    _threadObjectContexts.Remove(contextKey);
                }
            }
        }

        /*
        private static string BuildContextThreadName()
        {
            return Thread.CurrentThread.Name;
        }

        private static string BuildHttpContextName()
        {
            return OBJECT_CONTEXT_KEY;
        }*/

        #endregion

        #endregion


    }
}
