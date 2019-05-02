using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Winnovative;

namespace WaterTreatment.Web.Services
{
    public interface IPDFService
    {
        byte[] Create(string html);

        string ContentType { get; }
        string Extension { get; }
    }

    public class PDFService : IPDFService
    {
        private const float Margin = .3f * 45f;

        public byte[] Create(string html)
        {
            var converter = new PdfConverter
            {
                LicenseKey = "JKq6q7qrubm7vqu6uaW7q7i6pbq5pbKysrKruw==",
                MediaType = "print"
            };

            converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            converter.PdfDocumentOptions.BottomMargin = Margin;
            converter.PdfDocumentOptions.TopMargin = Margin;
            converter.PdfDocumentOptions.RightMargin = Margin;
            converter.PdfDocumentOptions.LeftMargin = Margin;

            html = LocalizeHtml(html);

            return converter.ConvertHtml(html, HttpContext.Current.Request.Url.ToString());
        }

        public string ContentType
        {
            get
            {
                return "application/force-download";
            }
        }

        public string Extension
        {
            get
            {
                return ".pdf";
            }
        }

        private static string LocalizeHtml(string html)
        {
            var localContent = "file:///" + HostingEnvironment.MapPath("~/Content/");
            var localScripts = "file:///" + HostingEnvironment.MapPath("~/Scripts/");

            return html.Replace("/Content/", localContent).Replace("/Scripts/", localScripts);
        }
    }
}