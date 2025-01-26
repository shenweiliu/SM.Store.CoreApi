using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SM.Store.Api.Common
{ 
    public class ProcessError
    {
        private static string logDateTimeFormat = StaticConfigs.GetConfig("LogDateTimeFormat") ?? "MM/dd/yyyy HH:mm:ss:fff";
        private static string eventSourceName = StaticConfigs.GetConfig("EventSourceName");
        private static string eventLogName = StaticConfigs.GetConfig("EventLogName");
        private static string errMsg = string.Empty;

        //private readonly IWebHostEnvironment hostEnv;
        //public ProcessError(IWebHostEnvironment hostEnv)
        //{
        //    this.hostEnv = hostEnv;
        //}

        public static async Task LogAndReport(HttpContext context, Exception ex)
        {
            try
            {
                //Get all level error messages.
                errMsg = LogUtil.GetAllErrorMessages(ex);

                //Build detailed error message.
                var errorModel = await BuildErrorMessage(context, ex);

                //Logging and emailing notification.
                await LoggingAndEmailing(errorModel, ex, context);

                ////Build response to be sent to client.
                //var respMsg = StaticConfigs.GetConfig("StaticMessageToClient");
                //if (string.IsNullOrEmpty(respMsg))
                //{
                //    //If no static message defined in the config file.
                //    respMsg = errorModel.Description;
                //}
                //Return response of error.
                //context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                //{
                //    Content = new StringContent(errorModel.StatusCode.ToString() + ": " + respMsg),
                //};
                await context.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = errMsg
                }.ToString());
            }
            catch (Exception ex2)
            {
                if (!string.IsNullOrEmpty(eventSourceName) && !string.IsNullOrEmpty(eventLogName))
                {
                    await LogUtil.WriteToEventLog(ex.Message + "\r\n" + ex2.StackTrace, EventLogEntryType.Error, eventSourceName, eventLogName);
                }
            }
        }

        public static async Task LocalErrorAndContinue(HttpContext context, Exception ex, string custErrMsg, string localInfo)
        {
            //Get all level error messages.
            errMsg = LogUtil.GetAllErrorMessages(ex);

            //Build detailed error message.
            var errorModel = await BuildErrorMessage(context, ex, custErrMsg, localInfo);

            //Logging and emailing notification.
            await LoggingAndEmailing(errorModel, ex, context);
        }

        public static async Task StartupError(Exception ex, string custErrMsg)
        {
            //Get all level error messages.
            errMsg = LogUtil.GetAllErrorMessages(ex);

            //Build detailed error message.
            var errorModel = await BuildErrorMessage(null, ex, custErrMsg);

            //Logging and emailing notification.
            await LoggingAndEmailing(errorModel, ex);
        }

        private static async Task<ErrorModel> BuildErrorMessage(HttpContext context, Exception ex, string custErrMsg = "", string localInfo = "")
        {
            StringBuilder sbMsg = new StringBuilder(string.Format("{0} - Data Service Exception \r\n", DateTime.Now.ToString(logDateTimeFormat)));

            if (!string.IsNullOrEmpty(localInfo))
            {
                sbMsg.AppendLine("-- A local error occurred but the process is allowed to continue --");
            }

            //Environment.
            var env = StaticConfigs.GetConfig("Environment");
            if (!string.IsNullOrEmpty(env))
            {
                sbMsg.AppendLine(string.Format("Environment:  {0}", env));
            }

            //Server or machine name.
            //sbMsg.AppendLine(string.Format("Server Name:  {0}", context.Current.Server.MachineName));
            var machineName = Environment.MachineName ?? "";
            if (machineName != "") sbMsg.AppendLine(string.Format("Server Name:  {0}", machineName));

            var platform = System.Runtime.InteropServices.RuntimeInformation.OSDescription ?? "";
            if (platform != "") sbMsg.AppendLine(string.Format("Platform:  {0}", platform));

            //Application name.
            var appName = StaticConfigs.GetConfig("ApplicationName   ");
            if (!string.IsNullOrEmpty(appName))
            {
                sbMsg.AppendLine(string.Format("Application:  {0}   ", appName));
            }

            //Request data.
            var request = context != null ? context.Request : null;
            if (request != null)
            {
                sbMsg.AppendLine(string.Format("Host Name:  {0}   ", request.Host.Value));

                //Web API route.
                sbMsg.AppendLine(string.Format("API Route:  {0}   ", request.Path));

                //HTTP method.
                sbMsg.AppendLine(string.Format("HTTP Method:  {0}   ", request.Method));

                //Request URL.
                var requestUrl = request.GetDisplayUrl();
                sbMsg.AppendLine(string.Format("Request URL:  {0}   ", requestUrl));

                //Request parameters.
                try
                {
                    var parameters = request.Query;
                    if (parameters != null && parameters.Count > 0)
                    {
                        sbMsg.AppendLine("Request Parameters: ");
                        foreach (var item in parameters)
                        {
                            sbMsg.AppendLine(string.Format("   " + item.Key + ": " + item.Value));
                        }
                    }
                }
                catch
                {
                    //Bypass "Key not found". Key only for URL query string.
                }

                //Request body content.
                var strResult = string.Empty;
                try
                {
                    //ASP.NET 5.
                    //using (var ctStream = context.Request.Body)
                    //{
                    //    ctStream.Position = 0;
                    //    using (StreamReader readStream = new StreamReader(ctStream, Encoding.UTF8))
                    //    {
                    //        strResult = readStream.ReadToEnd();
                    //    }
                    //}
                    //ASP.NET Core.
                    request.EnableBuffering();
                    request.Body.Position = 0;
                    //Not working...
                    //var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                    //await request.Body.ReadAsync(buffer, 0, buffer.Length);
                    //strResult = Encoding.UTF8.GetString(buffer);

                    //Pipeline reader - ASP.NET Core 3+.
                    var result = await request.BodyReader.ReadAsync();
                    var buffer = result.Buffer;
                    strResult = Encoding.Default.GetString(buffer.FirstSpan);
                    request.BodyReader.AdvanceTo(buffer.End);

                }
                catch
                {
                    //Bypass null content. Only POST has request body content.    
                }

                if (strResult != string.Empty)
                {
                    sbMsg.AppendLine(string.Format("Request Content Type:  {0}", request.ContentType.ToString()));

                    //If possible password text, wrap strResult with MaskJson(strResult).
                    sbMsg.AppendLine(string.Format("Request Content:  {0}", strResult));
                }

                //DB instance:
                var dbInstanceName = StaticConfigs.GetConfig("DBInstanceName");
                if (!string.IsNullOrEmpty(dbInstanceName))
                {
                    sbMsg.AppendLine(string.Format("Database Instance:  {0}", dbInstanceName));
                }
            }

            var errStatus = ErrorStatusCode.GENERAL_ERROR;
            var errDescription = "";
            if (string.IsNullOrEmpty(custErrMsg))
            {
                //Sub-category of error status and descriptions.
                ErrorModel thisError = CategoryErrorMessage(ex);
                errStatus = thisError.StatusCode;
                errDescription = thisError.Description;

                //Discription Line.
                //Remove possible line break before close parenthesis.
                errDescription = errDescription.Replace("\r\n)", ")").Replace("\n)", ")").Replace("\r)", ")");
                sbMsg.AppendLine(string.Format("\r\nDescriptions: {0} - {1}", errStatus, errDescription));
            }
            else
            {
                errDescription = custErrMsg;
                sbMsg.AppendLine(string.Format("\r\nDescriptions: {0} ", errDescription));
            }

            //Populate final ApiError object.
            ErrorModel rtnMsg = new ErrorModel()
            {
                StatusCode = errStatus,
                Description = errDescription,
                Details = sbMsg.ToString(),
                StackTraces = string.Format("\r\nStack Trace: \r\n{0}\r\n", ex.StackTrace),
                AppName = appName
            };
            return rtnMsg;
        }

        private static ErrorModel CategoryErrorMessage(Exception ex, string errorType = "")
        {
            //Set default apiErrorStatus.
            var errorStatus = ErrorStatusCode.GENERAL_ERROR;

            //Categorize error with SQL Server data.
            if (!string.IsNullOrEmpty(errorType))
            {
                if (errorType.Contains("System.Data.SqlClient") || errorType.Contains("SqlException"))
                {
                    errorStatus = ErrorStatusCode.DATABASE_ERROR;
                }
            }
            else if (ex is System.Data.DataException || ex is Microsoft.Data.SqlClient.SqlException)
            {
                var exMsgCheck = errMsg.ToLower();
                if (exMsgCheck.Contains("violation of unique key constraint"))
                {
                    errorStatus = ErrorStatusCode.DATA_DUPLICATE_ERROR;
                }
                else
                {
                    errorStatus = ErrorStatusCode.DATABASE_ERROR;
                }
            }
            else if (ex.Message.ToLower().Contains("unauthorized") ||
         ex.Message.ToLower().Contains("access denied"))
            {
                errorStatus = ErrorStatusCode.AUTHENTICATION_ERROR;
            }
            else if (ex.Message.ToLower().Contains("invalid data") ||
         ex.Message.ToLower().Contains("data validation failed"))
            {
                errorStatus = ErrorStatusCode.DATA_VALIDATION_ERROR;
            }
            //Add any your own error code, sources, and status into the ApiStatusCode enum and here.
            //else if (...)
            //{
            //    ...
            //}

            var errorModel = new ErrorModel()
            {
                StatusCode = errorStatus,
                Description = errMsg
            };
            return errorModel;
        }

        private static async Task LoggingAndEmailing(ErrorModel errorModel, Exception originalEx, HttpContext context = null)
        {
            var logFilePath = StaticConfigs.GetConfig("ErrorLogFilePath");
            var logHistoryFilePath = StaticConfigs.GetConfig("ErrorLogHistoryFilePath");

            //Variable for merged details and stack traces.
            var logMessage = errorModel.Details + errorModel.StackTraces;

            //Logging to files. If error, do not affect emailing.
            try
            {
                if (!string.IsNullOrEmpty(logFilePath))
                {
                    logFilePath = LogUtil.ResolveFilePath(logFilePath, PathType.WebRoot);

                    if (File.Exists(logFilePath))
                        await LogUtil.DeleteLogFile(logFilePath);
                    if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                    await LogUtil.WriteToLogFile(logFilePath, logMessage);
                }

                if (!string.IsNullOrEmpty(logHistoryFilePath))
                {
                    logHistoryFilePath = LogUtil.ResolveFilePath(logHistoryFilePath, PathType.WebRoot);

                    //Check and archive log file if reaching the maximun size.
                    var maxLogSizeInKB = StaticConfigs.GetConfig("MaxLogSizeInKB");
                    if (string.IsNullOrEmpty(maxLogSizeInKB)) maxLogSizeInKB = "1024";
                    await LogUtil.ArchiveLogFile(logHistoryFilePath, int.Parse(maxLogSizeInKB));

                    if (!Directory.Exists(Path.GetDirectoryName(logHistoryFilePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(logHistoryFilePath));

                    await LogUtil.WriteToLogFile(logHistoryFilePath, logMessage);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(eventSourceName) && !string.IsNullOrEmpty(eventLogName))
                {
                    await LogUtil.WriteToEventLog(ex.Message + "\r\n" + ex.StackTrace, EventLogEntryType.Error, eventSourceName, eventLogName);
                }
            }

            //Begin to send notification email.
            var emailMessage = StaticConfigs.GetConfig("ErrorEmailMessage"); //static.
            var emailPlaceHolderText = errorModel.Details;
            var emailAttachment = StaticConfigs.GetConfig("ErrorEmailAttachment");

            //Build dynamic attachment in plain text format.
            var attachmentList = new List<AttachDynamicFile>();
            if (!string.IsNullOrEmpty(emailAttachment))
            {
                var attachment = new AttachDynamicFile();
                var logFileName = Path.GetFileNameWithoutExtension(logFilePath);
                var attachMessage = string.Empty;
                if (emailAttachment.ToLower() == "full")
                {
                    attachMessage = logMessage;
                }
                else if (emailAttachment.ToLower() == "trace")
                {
                    logFileName = logFileName + ".Trace";
                    attachMessage = string.Format("Additional {0}", errorModel.StackTraces);
                }
                attachment.attachFileName = logFileName + "_" + DateTime.Now.ToString(logDateTimeFormat).Replace("/", "").Replace(":", "").Replace(" ", "_") + ".txt";
                attachment.attachFileBytes = Encoding.UTF8.GetBytes(attachMessage);
                attachmentList.Add(attachment);
            }
            else
            {
                //Place all in the email body.
                emailPlaceHolderText = logMessage;
            }

            var emailInput = new EmailInput()
            {
                SmtpHost = StaticConfigs.GetConfig("ErrorSmtpHost"),
                From = StaticConfigs.GetConfig("ErrorEmailFrom"),
                To = StaticConfigs.GetConfig("ErrorEmailTo"),
                Subject = StaticConfigs.GetConfig("ErrorEmailSubject"),
                Message = !string.IsNullOrEmpty(emailMessage) ? emailMessage.Replace("{#message#}", emailPlaceHolderText).Replace("{#newline#}", "\r\n") : emailPlaceHolderText,
                AttachDynamicFileList = attachmentList
            };
            await Emailer.SendEmail(emailInput);
        }

        private static string MaskJson(string arg)
        {
            if (arg != null)
            {
                var maskParams = new List<string> { "password", "answer" };

                foreach (var maskParam in maskParams)
                {
                    string argLowerCase = null;
                    int index = 0;
                    int backupindex = 0;
                    while (index > -1)
                    {
                        argLowerCase = arg.ToLower();
                        backupindex = index;
                        index = argLowerCase.IndexOf(maskParam, index);
                        if (index > -1)
                        {
                            int maskStart = argLowerCase.IndexOf(":", index);
                            int maskEnd = argLowerCase.IndexOf("\",", maskStart);
                            if (maskEnd == -1)
                            {
                                maskEnd = argLowerCase.IndexOf("\"}", maskStart);
                            }

                            if (maskStart >= 0 && maskStart < maskEnd)
                            {
                                arg = arg.Substring(0, maskStart + 1) + "******" + arg.Substring(maskEnd);
                                index = arg.IndexOf("\"", maskStart + 2);
                            }
                        }
                        // check for indefinite loop
                        if (index == backupindex)
                            break;
                    }
                }
            }

            return arg;
        }

        private static string MaskXml(string arg)
        {
            if (arg != null)
            {
                var maskParams = new List<string> { "password", "answer" };

                foreach (var maskParam in maskParams)
                {
                    string argLowerCase = null;
                    int index = 0;
                    int backupindex = 0;
                    while (index > -1)
                    {
                        argLowerCase = arg.ToLower();
                        backupindex = index;
                        index = argLowerCase.IndexOf(maskParam, index);
                        if (index > -1)
                        {
                            int maskStart = argLowerCase.IndexOf(">", index);
                            int maskEnd = argLowerCase.IndexOf("</", maskStart);
                            if (maskStart >= 0 && maskStart < maskEnd)
                            {
                                arg = arg.Substring(0, maskStart + 1) + "******" + arg.Substring(maskEnd);
                                index = arg.IndexOf(">", maskStart + 2);
                            }
                        }
                        // check for indefinite loop
                        if (index == backupindex)
                            break;
                    }
                }
            }
            return arg;
        }

        //Request pipeline reader for List/Array. May not be used here but for references.
        private static void AddStringToList(ref List<string> results, in ReadOnlySequence<byte> readOnlySequence)
        {
            //Separate method because Span/ReadOnlySpan cannot be used in async methods
            ReadOnlySpan<byte> span = readOnlySequence.IsSingleSegment ? readOnlySequence.First.Span : readOnlySequence.ToArray().AsSpan();
            results.Add(Encoding.UTF8.GetString(span));
        }
    }

}
