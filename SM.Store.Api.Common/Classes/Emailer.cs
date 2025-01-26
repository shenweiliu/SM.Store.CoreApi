using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace SM.Store.Api.Common
{
    public class Emailer
    {
        //private readonly IHttpContextAccessor httpContext;
        //public Emailer(IHttpContextAccessor httpContext)
        //{
        //    this.httpContext = httpContext;
        //}

        public static async Task SendEmail(EmailInput input, PathType pathType = PathType.ExeRoot)
        {
            await Task.Run(() =>
            {
                Attachment attachment = null;
                try
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        //Setting up general email parameters.
                        mail.From = new MailAddress(input.From);

                        if (!string.IsNullOrEmpty(input.To))
                            mail.To.Add(input.To.Replace(";", ","));

                        if (!string.IsNullOrEmpty(input.Cc))
                            mail.CC.Add(input.Cc.Replace(";", ","));

                        if (!string.IsNullOrEmpty(input.Bcc))
                            mail.Bcc.Add(input.Bcc.Replace(";", ","));

                        mail.Subject = input.Subject;
                        mail.Body = input.Message;
                        mail.IsBodyHtml = input.IsHtml;

                        if (!string.IsNullOrEmpty(input.attachFilePath))
                        {
                            input.attachFilePath = input.attachFilePath.Replace(";", ",");
                            var filePaths = input.attachFilePath.Split(',');
                            var fullFilePath = string.Empty;
                            foreach (var filePath in filePaths)
                            {
                                fullFilePath = LogUtil.ResolveFilePath(filePath, pathType);
                                if (File.Exists(fullFilePath))
                                {
                                    mail.Attachments.Add(new Attachment(fullFilePath));
                                }
                            }
                        }

                        if (input.AttachDynamicFileList != null && input.AttachDynamicFileList.Count > 0)
                        {
                            foreach (var item in input.AttachDynamicFileList)
                            {
                                //Add attachment from byte stream.
                                using (MemoryStream stream = new MemoryStream(item.attachFileBytes))
                                {
                                    if (stream.Length > 0)
                                    {
                                        attachment = new Attachment(stream, item.attachFileName);
                                        if (attachment != null)
                                            mail.Attachments.Add(attachment);
                                    }
                                    //Send out email before closing stream using SMTP server defined in config file.              
                                    using (SmtpClient smtp = new SmtpClient())
                                    {
                                        smtp.Host = input.SmtpHost;
                                        smtp.Send(mail);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Send out email using SMTP server defined in config file.              
                            using (SmtpClient smtp = new SmtpClient())
                            {
                                smtp.Host = input.SmtpHost;
                                smtp.Send(mail);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //To Windows Event Log.
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Emailer Error: " + ex.Message, EventLogEntryType.Error, 50001, 1);
                    }
                }
            });
        }

        //User LogUtil.ResolveFilePath
        //private static string ResolveFilePath(string filePath, HttpContext httpContext = null)
        //{
        //    var fullFilePath = string.Empty;
        //    if (filePath.Contains(@":\") || filePath.StartsWith(@"\\"))
        //    {
        //        //It's absolute path.
        //        fullFilePath = filePath;
        //    }
        //    else
        //    {
        //        //It's relative path.
        //        if (httpContext != null)
        //            //Web applications.
        //            fullFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        //        else
        //            //Non-web applications.
        //            fullFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filePath);
        //    }
        //    return fullFilePath;
        //}
    }

    public class EmailInput
    {
        public string SmtpHost { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsHtml { get; set; }
        //If attaching physical files (paths delimited by "," or ";").
        public string attachFilePath { get; set; }
        //If attaching file dynamically from byte array.
        public List<AttachDynamicFile> AttachDynamicFileList { get; set; }
    }

    public class AttachDynamicFile
    {
        public string attachFileName { get; set; }
        public byte[] attachFileBytes { get; set; }
    }
}

