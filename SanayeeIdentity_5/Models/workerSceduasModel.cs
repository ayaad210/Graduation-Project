using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SanayeeIdentity_5.Models
{
    public class workerSceduasModel
    {
        public string workerid { get; set; }
       public IEnumerable<SanayeeIdentity_5.Data.Schedual> List;
    }
}