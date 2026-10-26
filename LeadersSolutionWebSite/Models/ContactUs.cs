using LeadersSolutionWebSite.Utilities;
using LeadersSolutionWebSite.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace LeadersSolutionWebSite.Models
{
    public partial class ContactUs
    {
        public static int AddNewNewsLetter(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return 0;
            }
            var db = new LSDataModelContainer();
            var hash = CryptoHelper.Hash(email);
            var newsLetter = db.NewsLetters.Where(e => e.EmailHash == hash).FirstOrDefault();
            if (newsLetter == null)
            {
                newsLetter = new NewsLetter();
                newsLetter.Email = CryptoHelper.Encrypt(email);
                newsLetter.EmailHash = hash;
                newsLetter.SubscriptionDate = DateTime.Now;
                db.NewsLetters.Add(newsLetter);
                db.SaveChanges();
                return 1;
            }
            return 2;
        }
        public static ContactUs AddNewContactUs(ContactUsVM contactUsVM) 
        {
            if (contactUsVM == null)
            {
                return null;
            }
            var db = new LSDataModelContainer();

            var contactUs = new ContactUs();
            contactUs.Name = CryptoHelper.Encrypt(contactUsVM.Name);
            contactUs.Email = CryptoHelper.Encrypt(contactUsVM.Email);
            contactUs.Mobile = CryptoHelper.Encrypt(contactUsVM.Mobile);
            contactUs.Service = CryptoHelper.Encrypt(contactUsVM.Service);
            contactUs.Comment = contactUsVM.Comments;
            contactUs.ContactedDate = DateTime.Now;
            db.ContactUs.Add(contactUs);
            db.SaveChanges();
            return contactUs;
        }

       

    }
}