using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SanayeeIdentity_5.Models
{
    public class WorkerViewCardModelcs
    {
        public string Address { get; set; }
        public int WorkerId { get; set; }
        public string UserID { get; set; }
        public string SBIN { get; set; }
        public Nullable<bool> Onwork { get; set; }
        public Nullable<bool> Available { get; set; }
        public Nullable<int> TotalRate { get; set; }
        public int MaxRateofWorkers { get;set; }
        public byte[] PhotoBin { get; set; }
        public string Photo { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public string Id { get; set; }
        public int AddressId { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        //last worker schedual
        public int ScedualId { get; set; }
    }
}
