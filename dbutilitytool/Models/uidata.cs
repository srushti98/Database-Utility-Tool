using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dbutilitytool.Models
{
    public class uidata
    {
                     
      
        public string servername { get; set; }
        public string path { get; set; }
        public string at { get; set; }
        public string username { get; set; }


        [DataType(DataType.Password)]
        public string password { get; set; }

       
        public HttpPostedFileBase fileup { get; set; }

        public IEnumerable<SelectListItem> filename { get; set; }
        public IEnumerable<SelectListItem> selectedfileid { get; set; }

    }



    
}