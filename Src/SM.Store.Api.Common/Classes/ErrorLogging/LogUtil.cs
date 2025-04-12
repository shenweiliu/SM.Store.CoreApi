using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SM.Store.Api.Common
{
    public static class LogUtil
    {
        private static string logDateTimeFormat = StaticConfigs.GetConfig("LogDateTimeFormat") ?? "MM/dd/yyyy HH:mm:ss:fff";

        public static async Task WriteToLogFile(string filePath, string logMsg, int callCnt = 0)
        {
            int iCnt = callCnt;
            try
            {
                await Task.Run(() =>
                {
                    File.AppendAllText(filePath, string.Format("\r\n{0}", logMsg));
                });
            }
            catch (Exception ex)
            {
                //Set 6 time recursive calls if the file is being used by another process.
                if (iCnt > 5)
                {
                    //Log the error to event Log.
                    await WriteToEventLog(string.Format("WriteToLogFile Error: \r\n{0}", GetAllErrorMessages(ex)), EventLogEntryType.Error);
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                    iCnt += 1;
                    await WriteToLogFile(filePath, logMsg, callCnt: iCnt);
                }
            }
        }

        public static async Task WriteToEventLog(string logMsg, EventLogEntryType msgType, string eventSourceName = "", string eventLogName = "")
        {
            if (string.IsNullOrEmpty(eventSourceName))
                eventSourceName = StaticConfigs.GetConfig("EventSourceName");
            //if (string.IsNullOrEmpty(eventSourceName))
            //    eventSourceName = "Application";
            if (string.IsNullOrEmpty(eventLogName))
                eventLogName = StaticConfigs.GetConfig("EventLogName");
            //if (string.IsNullOrEmpty(eventLogName))
            //    eventLogName = "Application";

            if (string.IsNullOrEmpty(eventSourceName) || string.IsNullOrEmpty(eventLogName))
            {
                return;
            }

            if (!EventLog.SourceExists(eventSourceName))
                EventLog.CreateEventSource(eventSourceName, eventLogName);

            using (EventLog log = new EventLog())
            {
                log.Source = eventSourceName;
                log.Log = eventLogName;

                await Task.Run(() =>
                {
                    log.WriteEntry(logMsg, msgType);
                });
            }
        }

        public static async Task ArchiveLogFile(string filePath, int maxLogSizeInKB)
        {

            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
            if (!fi.Exists)
                return;
            try
            {
                if (fi.Length > (maxLogSizeInKB * 1024))
                {
                    var dt = DateTime.Now.ToString(logDateTimeFormat);
                    var folderPath = Path.GetDirectoryName(filePath);
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    await Task.Run(() =>
                    {
                        File.Move(filePath, folderPath + "\\" + fileName + "_" + dt.Replace("/", "").Replace(":", "").Replace(" ", "_") + ".log");
                    });
                }
            }
            catch (Exception ex)
            {
                //Log the error to both app and event logs.
                var errMsg = string.Format("\r\n{0}, BackupLog Error: {1}", DateTime.Now.ToString(logDateTimeFormat), GetAllErrorMessages(ex));
                await Task.Run(() =>
                {
                    File.AppendAllText(filePath, errMsg);
                });
                await WriteToEventLog(errMsg, EventLogEntryType.Error);
            }
        }

        public static async Task DeleteLogFile(string filePath, int callCnt = 0)
        {
            int iCnt = callCnt;
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    //Set 6 time recursive calls if the file is being used by another process.
                    if (iCnt > 5)
                    {
                        //Log the error to event Log.
                        await WriteToEventLog(string.Format("DeleteLogFile Error: \r\n{0}", GetAllErrorMessages(ex)), EventLogEntryType.Error);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                        iCnt += 1;
                        await DeleteLogFile(filePath, callCnt: iCnt);
                    }
                }
            }
        }

        public static string ResolveFilePath(string filePath, PathType pathType = PathType.ExeRoot)
        {
            var fullFilePath = string.Empty;
            if (filePath.Contains(@":\") || filePath.StartsWith(@"\\"))
            {
                //It's absolute path.
                fullFilePath = filePath;
            }
            else
            {
                //It's relative path.
                if (pathType == PathType.WebRoot)
                    //Web applications.
                    fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
                else if (pathType == PathType.ExeRoot)
                    //Non-web applications, or web area.
                    fullFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filePath);
            }
            return fullFilePath;
        }

        public static string GetAllErrorMessages(Exception ex)
        {
            var msg = string.Empty;
            Exception currentEx = ex;
            while (currentEx != null)
            {
                if (currentEx.InnerException == null || (currentEx.InnerException != null && currentEx.Data.Count > 0))
                {
                    if (string.IsNullOrEmpty(msg))
                        msg = currentEx.Message;
                    else
                        msg += System.Environment.NewLine + currentEx.Message;
                }
                currentEx = currentEx.InnerException;
            }
            return msg;
        }
    }

    public enum PathType
    {
        WebRoot,
        ExeRoot
    }
}
