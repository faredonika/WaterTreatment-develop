using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Properties;

namespace WaterTreatment.Web.Services
{

    public interface IEmailService
    {

        void SendRegisteredEmails();
        void Send(MailMessage Message);
        void Send(String To, String Subject, String Body);
        void Send(String To, String CC, String Bcc, String Subject, String Body);
        void Send(IEnumerable<String> To, IEnumerable<String> CC, IEnumerable<String> Bcc, String Subject, String Body);
        void Send(IEnumerable<String> To, IEnumerable<String> CC, IEnumerable<String> Bcc, String Subject, String Body, IEnumerable<Attachment> Attachments);

    }

    public class AdminSiteData
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
        public string siteName { get; set; }
    }

    public class EmailService : IEmailService
    {

        private readonly WTContext Context;

        public EmailService(WTContext context)
        {

            Context = context;

        }

        public void SendRegisteredEmails()
        {

            //foreach (var site in Context.Sites.Where(x => !x.NextDigest.HasValue || DateTime.UtcNow > x.NextDigest).ToList())
            //{
            //    try
            //    {
            //        SendSiteDigest(site);
            //        NotifyAdminsBadDataSiteDigest();
            //        SetNextDigest(site);
            //    }
            //    catch
            //    {
            //        throw;
            //    }

            //}
            Context.SaveChanges();

        }

        private void SendSiteDigest(Entities.Site S)
        {
            const string nl = "<br />";

            if (S.NextDigest.HasValue && DateTime.UtcNow < S.NextDigest.Value)
            {
                return;
            }

            var subject = string.Format("Monthly Report Digest for {0}", S.Name);

            int month = S.NextDigest.HasValue ? S.NextDigest.Value.AddMonths(-1).Month : DateTime.UtcNow.AddMonths(-1).Month;
            int year = S.NextDigest.HasValue ? S.NextDigest.Value.AddMonths(-1).Year : DateTime.UtcNow.AddMonths(-1).Year;

            var reportUrl = string.Format("<a href=\"{0}/Reports?from={1}&to={2}&site={3}\">All Reports for {4}</a>",
                Settings.Default.HostUrl,
                new DateTime(year, month, 1).ToShortDateString(),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToShortDateString(),
                S.Id,
                S.Name);

            var oobUrl = string.Format("<a href=\"{0}/Reports/OutOfBounds?site={1}\">Out of Acceptable Range Report for {2}</a>", Settings.Default.HostUrl, S.Id, S.Name);

            var body = new StringBuilder();
            body.AppendLine(Context.Settings.First().Value);
            body.AppendLine(nl);
            body.AppendLine(nl);
            body.AppendLine(reportUrl);
            body.AppendLine(nl);
            body.AppendLine(oobUrl);
            body.AppendLine(nl);
            body.AppendLine(nl);
            body.AppendLine("{0}");

            var subscribers = new List<Tuple<string, string>>(); 
            foreach (var subscription in Context.ReportSubscriptions.Where(rs => rs.Site.Id == S.Id).ToList())
            {
                subscription.UnsubscribeAuthToken = KeyGenerator.GetUniqueKey(128);

                var unsubscribeUrl = string.Format("<a href=\"{0}/Reports/Unsubscribe?id={1}&token={2}\">Unsubscribe to this email</a>", Settings.Default.HostUrl, subscription.Id, subscription.UnsubscribeAuthToken);

                subscribers.Add(new Tuple<string, string>(subscription.User.Email, unsubscribeUrl));
            }

            Context.SaveChanges();

            //foreach (var s in subscribers)
            //    Send(s.Item1, string.Empty, string.Empty, subject, string.Format(body.ToString(), s.Item2));

        }

        private void SetNextDigest(Entities.Site S)
        {
            if (!S.NextDigest.HasValue)
                S.NextDigest = NextMonth(DateTime.UtcNow);
            else
                S.NextDigest = NextMonth(S.NextDigest.Value);
        }

        private DateTime NextMonth(DateTime Start)
        {
            return new DateTime(Start.AddMonths(1).Year, Start.AddMonths(1).Month, 1);
        }

        public void Send(MailMessage Message)
        {

#if DEBUG
            //StringBuilder StrippedMessage = new StringBuilder();
            //var nl = "<br />";

            //StrippedMessage.AppendLine("These are the addresses removed for testing purposes");
            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine("To:");

            //foreach (var To in Message.To.Select(x => x.Address)) {
            //    StrippedMessage.AppendLine(To);
            //    StrippedMessage.AppendLine(nl);
            //}
           

            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine("CC:");

            //foreach (var CC in Message.CC.Select(x => x.Address)) {
            //    StrippedMessage.AppendLine(CC);
            //    StrippedMessage.AppendLine(nl);
            //}

            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine("BCC:");

            //foreach (var Bcc in Message.Bcc.Select(x => x.Address)) {
            //    StrippedMessage.AppendLine(Bcc);
            //    StrippedMessage.AppendLine(nl);
            //}

            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine(nl);

            //var From = Settings.Default.FromEmailAddress;

            //StrippedMessage.AppendLine("This is the configured Support Email");
            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine(From);
            //StrippedMessage.AppendLine(nl);
            //StrippedMessage.AppendLine(nl);

            //Message.Body = StrippedMessage.ToString() + Message.Body;

            //Message.To.Clear();
            //Message.CC.Clear();
            //Message.Bcc.Clear();

            //Message.To.Add(Context.Settings.Single(s => s.Id == Context.Ref.Settings.DebugQAEmail.Id).Value);
#endif

            Message.IsBodyHtml = true;

            using (var Client = new SmtpClient())
            {
                Client.Timeout = 30000;
                Client.Send(Message);
            }

        }

        public void Send(String To, String Subject, String Body)
        {
            Send(EmailAddressesAttribute.GetAddresses(To), new List<string>(), new List<string>(), Subject, Body);
        }

        public void Send(String To, String CC, String Bcc, String Subject, String Body)
        {

            Send(EmailAddressesAttribute.GetAddresses(To),
                EmailAddressesAttribute.GetAddresses(CC),
                EmailAddressesAttribute.GetAddresses(Bcc),
                Subject,
                Body);

        }

        public void Send(IEnumerable<String> To, IEnumerable<String> CC, IEnumerable<String> Bcc, String Subject, String Body)
        {
            Send(To, CC, Bcc, Subject, Body, new List<Attachment>());
        }

        public void Send(IEnumerable<String> To, IEnumerable<String> CC, IEnumerable<String> Bcc, String Subject, String Body, IEnumerable<Attachment> Attachments)
        {

            using (MailMessage Message = new MailMessage())
            {
                foreach (var address in To)
                    Message.To.Add(address);

                foreach (var address in CC)
                    Message.CC.Add(address);

                foreach (var address in Bcc)
                    Message.Bcc.Add(address);

                foreach (var item in Attachments)
                    Message.Attachments.Add(item);

                Message.Subject = Subject;
                Message.Body = Body;

                var From = Settings.Default.FromEmailAddress;

                Message.From = new MailAddress(From);
                Message.Priority = MailPriority.Normal;

                Send(Message);
            }

        }

        private void NotifyAdminsBadDataSiteDigest()
        {
            IEnumerable<AdminSiteData> resultantRows = null;
            try
            {
                resultantRows = Context.Database.SqlQuery<AdminSiteData>("exec [dbo].[spGetAdminsForSitesWithBadData]");
            }
            catch (Exception ex)
            {
                var Errormsg = ex.Message;
            }

            if (resultantRows == null)
            {
                return;
            }

            var distinctValues = resultantRows.GroupBy(c => c.Email).Select(c => c.First()).ToList();

            foreach (AdminSiteData adminNames in distinctValues)
            {
                var adminSites = resultantRows.Where(e => e.Email == adminNames.Email).ToList();
                NotifyAdminsBadDataSiteDigest(adminSites);
            }
        }


        private void NotifyAdminsBadDataSiteDigest(List<AdminSiteData> sites)
        {
            const string nl = "<br />";
            string email = sites[0].Email;
            string first = sites[0].FirstName;
            string last = sites[0].LastName;

            var subject = "Monthly Data Complience Report for Sites";

            string month = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture); ;
            int year = DateTime.Now.Year;
            string addressLine = "Dear " + first + '\n';

            var body = new StringBuilder();
            body.AppendLine(addressLine);
            body.AppendLine(nl);
            body.AppendLine("The Following sites have reported data not in compliance with the specified ranges.");
            body.AppendLine(nl);
            body.AppendLine("Please, follow the table below for more information.");
            body.AppendLine(nl);
            body.AppendLine(nl);
            body.AppendLine("<table align ='center'>");

            //var request = HttpContext.Current.Request;
            //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
            var url2 = Settings.Default.HostUrl;


            var subscribers = new List<Tuple<string, string>>();
            foreach (var site in sites)
            {
                var siteReports = Context.Reports.Where(r => r.Site.Id == site.Id).ToList();
                string reportUrl = "";
                string reportSubmitted ="";


                body.AppendLine("<tr>");
                body.AppendLine("<td align = 'right' > Site Name :  </td>");
                body.AppendLine("<td >" + site.siteName + "</td>");
                body.AppendLine("</tr>");
                foreach (var rpt in siteReports)
                {
                    reportUrl = url2 + "/Reports/View/" + rpt.Id.ToString();
                    reportSubmitted = "Report submitted on: " + ((DateTime)rpt.SubmissionDate).ToString("MM dd, yyyy");
                    body.AppendLine("<tr>");
                    body.AppendLine("<td align = 'right' > " + reportSubmitted + "</td>");
                    body.AppendLine("<td  >" + reportUrl + "</td>");
                    body.AppendLine("</tr>");
                }

            }
            body.AppendLine("</table><br>");

            //Context.SaveChanges();
            Send(email, string.Empty, string.Empty, subject, body.ToString());

        }

    }

}