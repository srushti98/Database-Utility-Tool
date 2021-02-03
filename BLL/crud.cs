using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BOL;
using DAL;

namespace BLL
{
    public class crud
    {
        string servernamefromcontroller;
        back obj = new back();
        List<filelog> dbfileinfo = new List<filelog>();
        List<cmndlog> dbcmdinfo = new List<cmndlog>();

        public void setconn(string _servername, string _dbname)
        {
            obj.DALsetupwinconnect(_servername, _dbname);

        }
        public void setsqlconn(string _servername, string _dbname, string user, string password)
        {
            dbfileinfo=obj.DALsetupsqlconnect(_servername, _dbname, user, password);

        }
        
        public string ExecuteSequentialFile(string path,string servername)
        {
            obj.DALopenconnection();
            dbfileinfo=obj.DALlogserverconnectfilelog(servername);
            int count1 = 0;int count2 = 0;int count3 = 0;
            int flagfailed = 0;
            Boolean checkfilefound = false;
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo flInfo in dir.GetFiles())
            {
                foreach (filelog c in dbfileinfo)
                {
                    if (flInfo.Name == c.filename)
                    {
                        checkfilefound = true;
                        if (c.filestatus.Trim() == "success")
                        {
                            count1++;break;
                        }

                        if (c.filestatus.Trim() == "failed")
                        {
                            dbcmdinfo=obj.DALgetsuccesscmdlogs(flInfo.Name);
                            flagfailed = 1;


                            count2++;break;
                        }


                    }
                    
                   

                }
                //no file found then execute
                if ((checkfilefound == false)||(flagfailed==1))
                {
                    string flag = "cancontinue";
                    string filestatus = "failed";
                    string dbname = fetchdbname(flInfo.Name);
                    String UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    int fileid = getlastfileid();
                    fileid++;
                    obj.DALenternewfilelog(fileid,flInfo.Name,filestatus,DateTime.Now,UserName,servername,dbname);
                                                              
                    IEnumerable<string> commandstrings= splitscript(path + "\\" + flInfo.Name);
                    flag = obj.executeandsave("use [" + dbname + "]", fileid, flInfo.Name);
                    if (checkfilefound==false)
                    {
                        
                        foreach (string commandstring in commandstrings)
                        {
                            if (commandstring != "")
                            {
                                if (flag == "cancontinue")
                                {
                                    flag = obj.executeandsave(commandstring, fileid, flInfo.Name);

                                }
                                else
                                {
                                    break;
                                }
                            }


                        }

                        if (flag == "cancontinue")
                        {
                            obj.DALupdatefilelog(fileid, "success");
                        }
                        count3++;
                    }
                    if(flagfailed==1)
                    {
                        int tempflag = 0;
                        foreach (string commandstring in commandstrings)
                        {
                            tempflag = 0;//for eah file checking
                            if (commandstring != "")
                            {
                                foreach (cmndlog commandlog in dbcmdinfo)
                                {
                                    if (commandlog.query.Equals(commandstring))
                                    {
                                        tempflag = 1;
                                        break;
                                    }
                                }

                                if (tempflag == 0)
                                {
                                    if (flag == "cancontinue")
                                    {
                                        flag = obj.executeandsave(commandstring, fileid, flInfo.Name);

                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag == "cancontinue")
                        {
                            obj.DALupdatefilelog(fileid, "success");
                        }


                    }


                }

                string path2 = @"\\SLB-JDM9XT2\Users\spawar10\Documents\test_final\" + flInfo.Name;
                flInfo.CopyTo(path2);



            }
            obj.DALcloseconnection();
            string hello = "filenotexecuted becoz already success:" + count1 + " file failed:" + count2+"new file executed"+count3;

            return (hello);
        }

        public IEnumerable<string> splitscript(string _path)
        {
            string script = System.IO.File.ReadAllText(_path);
            // split script on GO command
            System.Collections.Generic.IEnumerable<string> commandstrings = Regex.Split(script, @"^\s*GO\s*$",
                                     RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return commandstrings;
        }

        public string fetchdbname(string filename)
        {
            string dbname;
            string[] strArr = null;
            string[] strArrdot = null;
            
            char[] splitchar = { '_' };
            char[] splitdot = { '.' };
            strArr = filename.Split(splitchar);

            strArrdot = strArr[2].Split(splitdot);
            dbname = strArrdot[0];
            return dbname;

        }

        public int getlastfileid()
        {
            int id = obj.DALgetlastfileid();
            return id;
        }

        public void giveservernametobll(string servername)
        {
            servernamefromcontroller = servername;

        }

    }
}
