using System;
using System.Diagnostics;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TheBot.Model;

namespace TheBot
{

    public class Program
    {

        private static readonly TelegramBotClient Bot = new TelegramBotClient("368582572:AAGmFGS1Jdv8ZFMzlbvOnb2G8CNxMJTW764");

        static void Main(string[] args)
        {
            Console.WriteLine($"Staring at: {DateTime.Now.ToString()}");
            Console.WriteLine("Feedback Telegram bot v1.0.0.0");
            Console.WriteLine("Loading components...");

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Console.WriteLine("Connecting to Telegram servers...");
            var me = Bot.GetMeAsync().Result;
            Console.WriteLine("Robot veryfied as: " + me.Username);
            Console.WriteLine("Getting Messages...");
            Bot.StartReceiving();
            // Confirm close
            while (true)
            {
                string confirm = Guid.NewGuid().ToString();

                Console.WriteLine("Confirm exit with entering the following:\r\n" + confirm);

                if (Console.ReadLine() == confirm)
                    break;
            }
            Console.WriteLine("Stopping...");
            Bot.StopReceiving();
            Console.WriteLine("Goodbye...");
            Console.ReadKey();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            // Save the error to db
            using (var db = new MainModel())
            {
                db.Errors.Add(new Errors { DateTime = DateTime.Now, Message = receiveErrorEventArgs.ApiRequestException.Message, Id = Guid.NewGuid() });
                db.SaveChanges();
            }
        }

        public enum RequestTypes
        {
            Offer = 0,
            Entrepreneurship = 1,
            Message = 2,
        }

        const string Offer = "ثبت";
        const string Entrepreneurship = "ثبت طرح کارآفرینی";

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            string mes = "درخواستی جهت تکمیل وجود ندارد";
            var message = messageEventArgs.Message;
            try
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                Console.WriteLine(
                    "----------------------------\r\n" +
                    "Message received" + "\r\n" +
                    "Username: " + (message.Chat.Username ?? "[Username not defined]") + "\r\n" +
                    $"Chat ID: {message.Chat.Id} \r\n" +
                    $"Send DateTime: {message.Date.ToString()} \r\n" +
                    $"Recieved DateTime {DateTime.Now.ToString()} \r\n"
                    );
                using (var db = new MainModel())
                {
                    db.Messages.Add(
                        new Messages
                        {
                            Id = message.MessageId.ToString(),
                            ChatId = message.Chat.Id.ToString(),
                            DateTime = DateTime.Now,
                            Value = message.Text ?? string.Empty,
                            Content = null
                        }
                        );
                    db.SaveChanges();
                }


                using (var db = new TheBot.Model.MainModel())
                {
                    var Requests = db.Requests.Where(x => x.ChatId == message.Chat.Id.ToString()).Where(x => x.Status > 0 && x.Status != 6).ToList();
                    Requests lastRequest = null;

                    int type = -1;
                    if (Requests.Count() != 0)
                        lastRequest = Requests.OrderBy(x => x.SubmitDate).FirstOrDefault();

                    if (message != null && message.Type == MessageType.TextMessage)
                    {
                        if (message.Text.StartsWith("/start"))
                        {
                            mes = @"دستورات:
/start - مشاهده ی دستورات
/new - ثبت پیغام
/cont - تکمیل پیغام
/del - حذف پیغام های ناتمام
/fol - دریافت کد پیگیری
";
                        }
                        else if (message.Text.StartsWith("/del") && Requests.Count() > 0)
                        {
                            foreach (var item in Requests)
                            {
                                item.Status = item.Status * -1;
                            }
                            db.SaveChanges();
                            mes = "حذف پیغام های نا تمام با موفقیت انجام شد";
                        }
                        else if (message.Text.StartsWith("/cont") && lastRequest != null)
                        {
                            switch (lastRequest.Status)
                            {
                                case 1:
                                    mes = "نام و نام خانوادگی خود را وارد نمائید";
                                    break;
                                case 2:
                                    mes = "سن خود را وارد نمائید";
                                    break;
                                case 3:
                                    mes = "جنسیت خود را وارد نمائید";
                                    break;
                                case 4:
                                    mes = "شماره ی تماس خود را وارد نمائید";
                                    break;
                                case 5:
                                    mes = "از حجت الاسلام والمسلمین دکتر رییسی چه میخواهید؟";
                                    break;
                            }
                        }
                        else if (message.Text.StartsWith("/fol"))
                        {
                            mes = "کد پیگیری شما:\r\n" + message.Chat.Id;
                        }

                        else if (message.Text.StartsWith("/new"))
                        {
                            type = (int)RequestTypes.Message;
                            db.Requests.Add(
                            new Requests
                            {
                                Id = Guid.NewGuid(),
                                SubmitDate = message.Date,
                                ChatId = message.Chat.Id.ToString(),
                                Status = 1,
                                Type = type
                            }
                            );
                            db.SaveChanges();
                            mes = "درخواست با موفقیت ثبت شد. ادامه:\r\n/cont";
                        }

                        else
                        {

                            if (lastRequest != null)
                            {
                                switch (lastRequest.Status)
                                {
                                    case 1:
                                        lastRequest.Fullname = message.Text;
                                        lastRequest.Status = 2;
                                        mes = "نام و نام خانوادگی ثبت شد، سن خود را وارد نمائید";
                                        break;
                                    case 2:
                                        lastRequest.Age = message.Text;
                                        lastRequest.Status = 3;
                                        mes = "سن شما ثبت شد، جنسیت خود را وارد نمائید";
                                        break;
                                    case 3:
                                        lastRequest.Gender = message.Text;
                                        lastRequest.Status = 4;
                                        mes = "جسنیت شما ثبت شد، شماره ی تماس خود را وارد نمائید";
                                        break;
                                    case 4:
                                        lastRequest.Phone = message.Text;
                                        lastRequest.Status = 5;
                                        mes = "تلفن تماس شما ثبت شد. از حجت الاسلام والمسلمین دکتر رییسی چه میخواهید";
                                        break;
                                    case 5:
                                        lastRequest.Question1 = message.Text;
                                        lastRequest.Status= 6;
                                        mes = $"ضمن قدردانی از ثبت پیغام شما به اطلاع میرساند، پیغام شما در سامانه ثبت گردید و مورد پیگیری قرار خواهد گرفت. شما میتوانید با شماره پیگیری {message.Chat.Id}، موارد را پیگیری نمائید.";
                                        break;
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                mes = "خطا در پردازش پیغام شما";
            }
            await Bot.SendTextMessageAsync(message.Chat.Id, mes, replyToMessageId: message.MessageId, replyMarkup: new ReplyKeyboardHide());
        }
    }
}
