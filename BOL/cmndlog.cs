using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class cmndlog
    {   
        public int fileid { get; set; }
        public string query { get; set; }
        public string query_status { get; set; }
        public string query_message { get; set; }
        public string failed_query_line_no { get; set; }
        public string query_datetime { get; set; }
      
    }
}
