using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeadersSolutionWebSite.VM
{
    public class ContactUsVM
    {
        [Required]
        [StringLength(300)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(300)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; }

        [Required]
        public string Service { get; set; }

        [Required]
        [StringLength(500)]
        public string Comments { get; set; }

        public string RecaptchaToken { get; set; }
    }

}