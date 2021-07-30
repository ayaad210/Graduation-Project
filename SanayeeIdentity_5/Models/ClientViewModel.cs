using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SanayeeIdentity_5.Models
{
    public class ClientViewModel
    {

      public  string Uid;

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }


        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

    



        [Display(Name = "City")]

        public int CityId { get; set; }

        [Display(Name = "Area")]

        public int AreaId { get; set; }


    

 

    }

}