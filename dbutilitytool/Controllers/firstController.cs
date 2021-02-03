using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BLL;
using dbutilitytool.Models;

namespace dbutilitytool.Controllers
{
    public class firstController : Controller
    {
        public ActionResult home()
        {
            List<SelectListItem> filelist = new List<SelectListItem>();

            filelist.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            uidata uidata = new uidata();
            uidata.filename = filelist;
                    
            return View(uidata);


        }
        
        public ActionResult connectservercall(uidata sample)
        {
            List<SelectListItem> filelist = new List<SelectListItem>();

            filelist.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });


            uidata uidata = new uidata();
            uidata.filename = filelist;

            crud objj = new crud();
            
            int result = Int32.Parse(sample.at);

            if (sample.servername != null)
            {
                TempData["storeserver"] = sample.servername;
                if (result == 1)
                {
                    objj.setconn(sample.servername, "master");

                   
                }
                else
                {
                    objj.setsqlconn(sample.servername, "master", sample.username, sample.password);

                }

            }
            ViewBag.connmessage = "Conncetion established successfully!!!";
            objj.giveservernametobll(sample.servername);

            return View("home", uidata);
        }


        [HttpPost]
        public ActionResult getfiles(uidata sample)
        {

            int i = 1;
            crud objj2 = new crud();
            List<SelectListItem> filelist = new List<SelectListItem>();

            string path = sample.path; 
            
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo flInfo in dir.GetFiles())
            {

                filelist.Add(new SelectListItem
                {
                    Text = flInfo.Name,
                    Value = i.ToString()
                });
                i++;
            }
            uidata uidata = new uidata();
            uidata.filename = filelist;
            return View("home", uidata);
        }


        [HttpPost]
        public string getfilesresult(IEnumerable<string> selectedfileid) 
        {
            if (selectedfileid == null)
            {

                return "You did not select anything";

            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("You Selected:" + string.Join(",", selectedfileid));
                return sb.ToString();
            }
        }
        public ActionResult executefiles(uidata sample)
        {
            crud objj2 = new crud();
            string storeserver = TempData["storeserver"].ToString();
            string path = sample.path;
            objj2.setconn(storeserver, "master");
            ViewBag.hello = objj2.ExecuteSequentialFile(path,storeserver);
            return View("home");
        }
    }
}