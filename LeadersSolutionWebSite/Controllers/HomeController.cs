using System.Globalization;
using LeadersSolutionWebSite.Models;
using LeadersSolutionWebSite.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Threading;
namespace LeadersSolutionWebSite.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult ChangeLanguage(string url, string languageChoice)
        {
            if (!string.IsNullOrEmpty(languageChoice))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageChoice);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageChoice);
            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = languageChoice;
            Response.Cookies.Add(cookie);
            return Redirect(url);
        }

        /// <summary>
        /// Download LS Profile
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult DownloadCompanyProfile()
        {
            string userIp = Request.UserHostAddress;
            string key = "companyprofile_" + userIp;

            // Limit: 1 request every 10 seconds
            if (RateLimiter.IsLimitedLSProfile(key, 300))
            {
                return RedirectToAction("About", new { msg = Resources.ABoutUSPage.RateLimitMessag });
            }
            // Build the full physical path to the file
            string path = Server.MapPath("~/Files/" + "LSProfile.pdf");
            // You can also use Path.Combine with IWebHostEnvironment in ASP.NET Core
            // Read the file data into a byte array
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            // Specify the MIME type (e.g., "application/pdf", "image/png", or "application/octet-stream" for general files)
            string mimeType = "application/octet-stream";

            // Return the file for download with a specified download name
            return File(fileBytes, mimeType, "LSProfile.pdf");
        }
        /// <summary>
        /// Validate its not a robot and the Model data before Saving to DB
        /// </summary>
        /// <param name="contactUsVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(ContactUsVM contactUsVM)
        {
            string userIp = Request.UserHostAddress;
            string key = "contactus_" + userIp;

            // Limit: 1 request every 5 min
            if (RateLimiter.IsLimitedContactUs(key, 300))
            {
                return RedirectToAction("About", new { msg = Resources.ABoutUSPage.RateLimitMessag });
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("About", new { msg = Resources.ABoutUSPage.RobotValidateMessag });
            }
            var secret = "6LesxrwtAAAAAPFI5T_sWXmwaEYKZVpHpTOtUrp8";
            var token = contactUsVM.RecaptchaToken;

            using (var client = new WebClient())
            {
                var result = client.DownloadString(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}"
                );

                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                bool success = json.success;
                var score = json.score;

                if (!success || score < 0.5m)
                {
                   
                    return RedirectToAction("About", new { msg = Resources.ABoutUSPage.RobotValidateMessag });
                    
                }
            }
            try
            {
                contactUsVM.Name = HttpUtility.HtmlEncode(contactUsVM.Name);
                try
                {
                    var addr = new System.Net.Mail.MailAddress(contactUsVM.Email);
                    if (addr.Address != contactUsVM.Email)
                    {
                        return RedirectToAction("Index", new { msg = Resources.ABoutUSPage.EnterValidEmail });
                    }
                }
                catch
                {
                    return RedirectToAction("Index", new { msg = Resources.ABoutUSPage.EnterValidEmail });
                }
                contactUsVM.Email = HttpUtility.HtmlEncode(contactUsVM.Email);
                contactUsVM.Mobile = HttpUtility.HtmlEncode(contactUsVM.Mobile);
                contactUsVM.Service = HttpUtility.HtmlEncode(contactUsVM.Service);
                contactUsVM.Comments = HttpUtility.HtmlEncode(contactUsVM.Comments ?? "");

                var msg = ContactUs.AddNewContactUs(contactUsVM);

                return RedirectToAction("About", new { msg = Resources.ABoutUSPage.ThanksAndProcesContactMessage });

            }
            catch (Exception)
            {
                return RedirectToAction("About", new { msg = Resources.ContactUsPage.PleaseRetryAgain });
            }
            
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SubscribeToNewsLetter(string newsLetterEmail)
        {
            string userIp = Request.UserHostAddress;
            string key = "newsletter_" + userIp;

            // Limit: 1 request every 10 seconds
            if (RateLimiter.IsLimitedNewsLetterSubscrip(key, 300))
            {
                return RedirectToAction("Index", new { msg = Resources.ContactUsPage.RateLimitMessag });
            }
            if (String.IsNullOrEmpty(newsLetterEmail))
            {
                return RedirectToAction("Index", new { msg = Resources.ContactUsPage.PleaseRetryAgain });
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(newsLetterEmail);
                if (addr.Address != newsLetterEmail)
                {
                    return RedirectToAction("Index", new { msg = Resources.ContactUsPage.PleaseRetryAgain });
                }
            }
            catch
            {
                return RedirectToAction("Index", new { msg = Resources.ContactUsPage.PleaseRetryAgain });
            }
            newsLetterEmail = HttpUtility.HtmlEncode(newsLetterEmail);
            var result = ContactUs.AddNewNewsLetter(newsLetterEmail);
            switch (result)
            {
                case 0: // Success
                    return RedirectToAction("Index", new { msg = Resources.ContactUsPage.PleaseRetryAgain });

                case 1: // Already subscribed
                    return RedirectToAction("Index", new { msg = Resources.ContactUsPage.ThankYouForSubscrition });

                case 2: // Technical error
                    return RedirectToAction("Index", new { msg = Resources.ContactUsPage.AlreadySubscribed });

                default:
                    return RedirectToAction("Index", new { msg = Resources.ContactUsPage.ThankYouForSubscrition });
            }   
        }
        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }

        public ActionResult About(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }

        public ActionResult CEOStatementPage()
        {            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = Resources.ContactUsPage.AppreciateReachingOut;

            return View();
        }
        public ActionResult LSServises()
        {
            
            return View();
        }
        public ActionResult LSSolutions()
        {
           
            return View();
        }
        public ActionResult LSCyberSecurity()
        {       

            return View();
        }
        public ActionResult LSServisesDetails()
        {

            return View();
        }

        public ActionResult LSBlogs()
        {

            return View();
        }
        public ActionResult LSBlogDetails()
        {

            return View();
        }
    }
}