using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SanayeeIdentity_5.Models
{

    public class WorkerRegisterViewModel
    {


        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Type Of work")]
        [Required]

        public int typeid { get; set; } 
         
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "ISBN")]
        
        public string ISBN { get; set; }

        [Display(Name = "City")]

        public int CityId { get; set; }

        [Display(Name = "Area")]

        public int AreaId { get; set; }

        public bool IsAvailable { get; set; }

        public string userid { get; set; }

        public byte[] PhotoBin { get; set; }

    }

}