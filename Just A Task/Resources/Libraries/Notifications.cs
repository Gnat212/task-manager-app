using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerRemakeApp.Data;
using System.IO;
using Telegram.Bot;
using System.Runtime.Remoting.Messaging;
using Telegram.Bot.Types.InputFiles;

namespace TaskManagerRemakeApp.Resources.Libraries
{
    public enum NotificationType
    {
        TaskAdd, TaskInfoUpdate, TaskStatusUpdate
    }
    
    public static class Notifications 
    {
        private static string StatusToString(Status status)
        {
            switch (status)
            {
                case Status.InWork:
                    return "В работе";
                case Status.OnCheck:
                    return "На проверке";
                case Status.Fired:
                    return "Просрочено";
                case Status.Completed:
                    return "Завершёно";
                case Status.Rejected:
                    return "Отклонено";
                case Status.Cancelled:
                    return "Отменено";
                default:
                    return "Неизвестно";
            }
        }

        private static void DivideMessageForSmtp(string message, out string header, out string body)
        {
            string tempHeader;

            tempHeader = message.Substring(0, message.IndexOf("<br><br>"));
            tempHeader = tempHeader.Replace("<b>", "");
            tempHeader = tempHeader.Replace("</b>", "");
            tempHeader = tempHeader.Replace("<i>", "");
            tempHeader = tempHeader.Replace("</i>", "");

            header = tempHeader;
            body = message.Substring(message.IndexOf("<br><br>") + 8);
        }

        private static string FormatMessgaeForTelegram(string message)
        {
            return message.Replace("<br>", Environment.NewLine);
        }

        private static string CreateAddMessage(Data.Task task)
        {
            var users = task.UsersList.ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append($"<b>Добавлена новая задача</b> <i>\"{task.Header}\"</i><br><br>");
            sb.Append($"<b>Сроки выполнения:</b> с <i>{task.GetTime:dd.MM.yyyy}</i> по <i>{task.EndTime:dd.MM.yyyy}</i><br><br>");
            sb.Append("<b>Ответственный:</b><br>");
            sb.Append($"<i>{task.ResponsibleName}</i><br><br>");
            sb.Append("<b>Участники:</b><br>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.Append($"<i>{i + 1}. {users[i].Username}</i><br>");
            }
            sb.Append("<br><b>Подробности в приложении Just a Work</b>");

            return sb.ToString();
        }

        private static string CreateInfoUpdateMessage(Data.Task task) 
        {
            var users = task.UsersList.ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append($"<b>Задача</b> <i>\"{task.Header}\"</i> <b>изменена</b><br><br>");
            sb.Append($"<b>Сроки выполнения:</b> с <i>{task.GetTime:dd.MM.yyyy}</i> по <i>{task.EndTime:dd.MM.yyyy}</i><br><br>");
            sb.Append("<b>Ответственный:</b><br>");
            sb.Append($"<i>{task.ResponsibleName}</i><br><br>");
            sb.Append("<b>Участники:</b><br>");
            for (int i = 0; i < users.Count; i++)
            {
                sb.Append($"<i>{i + 1}. {users[i].Username}</i><br>");
            }
            sb.Append("<br><b>Подробности в приложении Just a Work</b>");

            return sb.ToString();
        }

        private static string CreateStatusUpdateMessage(Data.Task task)
        {
            var status = task.CurrentStatus;
            bool hasFile = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id) != null;

            StringBuilder sb = new StringBuilder();
            sb.Append($"<b>Cтатус задачи</b> <i>\"{task.Header}\"</i> <b>изменён на</b> <i>\"{StatusToString(status)}\"</i><br><br>");

            if (status == Status.InWork && task.IsReturned)
            {
                sb.Append($"<b>Задача отправлена на доработку</b><br>");
                sb.Append($"<b>Причина:</b><br>");
                sb.Append($"<i>{task.AdminReport}</i><br><br>");
                sb.Append("<b>Подробности в приложении Just a Work</b>");
            }
            else if (status == Status.OnCheck && hasFile)
            {
                sb.Append("<i>Файл отчёта прикреплён к сообщению</i><br><br>");
                sb.Append("<b>Подробности в приложении Just a Work</b>");
            }
            else if (status == Status.OnCheck && hasFile == false)
            {
                sb.Append("<i>Файл отчёта отсутствует</i><br><br>");
                sb.Append("<b>Подробности в приложении Just a Work</b>");
            }
            else
                sb.Append("<b>Подробности в приложении Just a Work</b>");


            return sb.ToString();
        } 

        
        private static async void SmtpSendMessage(string recipientEmail, string header, string body, byte[] attachment = null, string attachmentName = null)
        {
            var smtp = TaskDbEntities.NewContext.SmtpServer.First();

            string smtpServer = smtp.SmtpServerIp;
            int smtpPort = Convert.ToInt32(smtp.SmtpServerPort);
            string senderEmail = smtp.SmtpLogin;
            string senderPassword = smtp.SmtpPassword;
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(smtpServer))
                {
                    smtpClient.Port = smtpPort;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    using (MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail))
                    {
                        mailMessage.Subject = header;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = body;

                        if (attachment != null && attachmentName != null)
                        {
                            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), attachmentName));
                        }

                        await System.Threading.Tasks.Task.Run(() =>
                        {
                            smtpClient.Send(mailMessage);
                        }
                        );
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

        }

        private static async void TelegramSendMessage(string telegramUserId, string message, byte[] attachment = null, string attachmentName = null)
        {
            var client = new TelegramBotClient(TaskDbEntities.NewContext.TelegramBot.First().TelegramBotToken);
            
            if (attachment == null && attachmentName == null)
            {
                _ = await client.SendTextMessageAsync(telegramUserId, FormatMessgaeForTelegram(message), Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            else
            {
                var file = new InputOnlineFile(new MemoryStream(attachment));
                file.FileName = attachmentName;
                _ = await client.SendDocumentAsync(telegramUserId, file, caption: FormatMessgaeForTelegram(message), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }

        public static async void SendNotifications(Data.Task task, NotificationType type)
        {
            if (type == NotificationType.TaskAdd)
            {
                var userList = task.UsersList.ToList();
                string notificationString = CreateAddMessage(task);
                string header, body;
                DivideMessageForSmtp(notificationString, out header, out body);

                for (int i = 0; i < userList.Count; i++)
                {
                    if (userList[i].TelegramUserId != null)
                    {
                        try 
                        {
                            await System.Threading.Tasks.Task.Run(() =>
                            {
                                TelegramSendMessage(userList[i].TelegramUserId, notificationString);
                            }
                        );   
                        }
                        catch { }
                    }
                    if (userList[i].Email != null)
                    {
                        try 
                        {
                            await System.Threading.Tasks.Task.Run(() =>
                            {
                                SmtpSendMessage(userList[i].Email, header, body);
                            }
                        ); 
                        }
                        catch { }
                    }
                }
                return;
            }
            if (type == NotificationType.TaskInfoUpdate)
            {
                var userList = task.UsersList.ToList();
                string notificationString = CreateInfoUpdateMessage(task);
                string header, body;
                DivideMessageForSmtp(notificationString, out header, out body);

                for (int i = 0; i < userList.Count; i++)
                {
                    if (userList[i].TelegramUserId != null)
                    {
                        try { TelegramSendMessage(userList[i].TelegramUserId, notificationString); }
                        catch { }
                    }
                    if (userList[i].Email != null)
                    {
                        try { SmtpSendMessage(userList[i].Email, header, body); }
                        catch { }
                    }
                }
                return;
            }
            if (type == NotificationType.TaskStatusUpdate)
            {
                var dbContext = TaskDbEntities.NewContext;

                var userList = task.UsersList.ToList();
                var adminList = dbContext.User.Where(x => x.IsAdmin).ToList();
                string notificationString = CreateStatusUpdateMessage(task);
                string header, body;
                DivideMessageForSmtp(notificationString, out header, out body);

                var taskFile = TaskDbEntities.NewContext.TaskFile.FirstOrDefault(x => x.TaskId == task.Id);
                for (int i = 0; i < userList.Count; i++)
                {
                    if (userList[i].TelegramUserId != null)
                    {
                        try 
                        {
                            if (task.CurrentStatus == Status.OnCheck)
                            {
                                if (taskFile != null)
                                    TelegramSendMessage(userList[i].TelegramUserId, notificationString, taskFile.AttachedFile, taskFile.AttachedFileName);
                                else
                                    TelegramSendMessage(userList[i].TelegramUserId, notificationString);
                            }
                            else
                                TelegramSendMessage(userList[i].TelegramUserId, notificationString); 
                        }
                        catch { }
                    }
                    if (userList[i].Email != null)
                    {
                        try 
                        {
                            if (task.CurrentStatus == Status.OnCheck)
                            {
                                if (taskFile != null)
                                    SmtpSendMessage(userList[i].Email, header, body, taskFile.AttachedFile, taskFile.AttachedFileName);
                                else
                                    SmtpSendMessage(userList[i].Email, header, body);
                            }
                            else
                                SmtpSendMessage(userList[i].Email, header, body);
                        }
                        catch { }
                    }
                }

                if (task.CurrentStatus == Status.OnCheck)
                {
                    for (int i = 0; i < adminList.Count; i++)
                    {
                        if (adminList[i].TelegramUserId != null)
                        {
                            try
                            {
                                if (taskFile != null)
                                    TelegramSendMessage(adminList[i].TelegramUserId, notificationString, taskFile.AttachedFile, taskFile.AttachedFileName);
                                else
                                    TelegramSendMessage(adminList[i].TelegramUserId, notificationString);
                            }
                            catch { }
                        }
                        if (adminList[i].Email != null)
                        {
                            try
                            {
                                if (taskFile != null)
                                    SmtpSendMessage(adminList[i].Email, header, body, taskFile.AttachedFile, taskFile.AttachedFileName);
                                else
                                    SmtpSendMessage(adminList[i].Email, header, body);
                            }
                            catch { }
                        }
                    }
                }     
                
                return;
            }

        }

    }
}
