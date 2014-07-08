using System.Net.Mail; 

namespace WeatherAggregator.Core.Logic
{
    public static class EmailSender
    {
        private static  string fromEmailAddress = "\"Weather Aggregator\" <WeatherAggregator@mail.com>";
        private static string hostAddress = "smtp.gmail.com";

        public static void SendEmail(string toEmailAddress, string mailBody)
        {
            //MailMessage mail = new MailMessage();

            //mail.From = new MailAddress(fromEmailAddress);
            //SmtpClient smtp = new SmtpClient();
            //smtp.Port = 465;
            //smtp.UseDefaultCredentials = true;
            //smtp.Host = hostAddress;
            //smtp.EnableSsl = true;
            //mail.To.Add(new MailAddress(toEmailAddress));
            //mail.IsBodyHtml = true;
            //mail.Body = mailBody;
            //smtp.Send(mail);
        }
    }
}
