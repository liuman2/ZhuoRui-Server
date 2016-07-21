using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Common
{
    public class LogHelper
    {
       static  ILog mylog = log4net.LogManager.GetLogger("MyLoger");
       static ILog sqlLog = log4net.LogManager.GetLogger("SqlLogger");
       
        public static void LogError(string message,Exception ex=null)
        {
            if (ex != null)
            {
                mylog.Error(message, ex);
            }
            else
            {
                mylog.Error(message);
            } 
        }
        public static void LogSql(string message)
        {

            sqlLog.Info(message); 
        }

    }
}
