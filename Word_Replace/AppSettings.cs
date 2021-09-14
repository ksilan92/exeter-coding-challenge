using System;
using System.Configuration;

namespace Word_Replace
{
    public static class AppSettings
    {
        #region public static T GetAppSetting<T>(string key)
        /// <summary>
        /// Get the appsetting values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string key)
        {
            return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }
        #endregion
    }
}
