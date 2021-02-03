using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOL;


namespace DAL
{

    public class back
    {
        string connetionString;
        SqlConnection cnn;
       
        SqlConnection cnn2;
        List<filelog> dbfileinfo = new List<filelog>();
        List<cmndlog> dbcmdinfo = new List<cmndlog>();


        public List<cmndlog> DALgetsuccesscmdlogs(string filename)
        {
            DALlogsconnect();

            SqlCommand cmd = new SqlCommand("select * from [dbo].[cmndlogs] where [filename]='" + filename + "' and querystatus='success'", cnn2);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    cmndlog data = new cmndlog();
                    data.fileid = Convert.ToInt32(dr["fileid"]);
                    data.query = dr["query"].ToString();
                    data.query_status = dr["querystatus"].ToString();
                    data.query_message = dr["querymessage"].ToString();
                    data.failed_query_line_no = dr["failedquerylineno"].ToString();
                    data.query_datetime = dr["querydatetime"].ToString();
                    
                    dbcmdinfo.Add(data);

                }
            }
            cnn2.Close();
            return dbcmdinfo;
        }

        public List<filelog> DALlogserverconnectfilelog(string servername)
        {
            DALlogsconnect();
           
            SqlCommand cmd = new SqlCommand("select * from [dbo].[filelogs] where [servername]='" + servername + "' order by srno desc", cnn2);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr != null)
            {
                while (dr.Read())
                {
                    filelog data = new filelog();
                    data.fileid = Convert.ToInt32(dr["fileid"]);
                    data.filename = dr["filename"].ToString();
                    data.filestatus = dr["filestatus"].ToString();
                    data.filedatetime = dr["filedatetime"].ToString();
                    data.osusername = dr["osusername"].ToString();
                    data.servername = dr["servername"].ToString();
                    data.dbname = dr["dbname"].ToString();
                    dbfileinfo.Add(data);
                    
                }
            }
            cnn2.Close();
            return dbfileinfo;
           

        }

        public void DALlogsconnect()
        {
            connetionString = "data source=" + "SLB-6899XT2" + ";initial catalog =" + "logs" + ";integrated security = SSPI;persist security info = False;Trusted_Connection = Yes";
            cnn2 = new SqlConnection(connetionString);
            cnn2.Open();
        }

        public void DALsetupwinconnect(string server, string dbName)
        {
            
            connetionString = "data source=" + server + ";initial catalog =" + dbName + ";integrated security = SSPI;persist security info = False;Trusted_Connection = Yes";
            cnn = new SqlConnection(connetionString);            
           
            

        }

        public List<filelog> DALsetupsqlconnect(string server, string dbName, string user, string password)
        {
            connetionString = "Data Source=" + server + ";Initial Catalog=" + dbName + ";User ID=" + user + ";Password=" + password;
            cnn = new SqlConnection(connetionString);            
            DALlogserverconnectfilelog(server);
            return dbfileinfo;
        }

        public void DALcloseconnection()
        {
            cnn.Close();
        }

        public void DALopenconnection()
        {
            cnn.Open();
        }

        public void storenegcmndlogs(SqlException ex,string _query,int _fileid,string filename)
        {
            string spError = _query;
            spError = spError.Replace("'", "''");
            string errmess = ex.Message.Replace("'","''");
            string _queryforcmndlog = "Insert into cmndlogs values(" + _fileid + ",'" + spError + "','" + "failed" + "','" + errmess + "'," + ex.LineNumber + ",'" + DateTime.Now + "','" + filename +"')";

            cnn2.Open();
            var command = new SqlCommand(_queryforcmndlog, cnn2);
            command.ExecuteNonQuery();
            cnn2.Close();

        }

        public void storeposcmndlogs(string _query,int _fileid, string filename)
        {
            string spError = _query;
            spError = spError.Replace("'", "''");
            string _queryforcmndlog = "Insert into cmndlogs values(" + _fileid + ",'" + spError + "','" + "success" + "','" + "1 row affected" + "'," + 0 + ",'" + DateTime.Now + "','" + filename + "')";

            cnn2.Open();
            var command = new SqlCommand(_queryforcmndlog, cnn2);
            command.ExecuteNonQuery();
            cnn2.Close();
        }

        public string executeandsave(string _query,int _fileid, string filename)
        {
            string flag;
            
            try
            {
                var command = new SqlCommand(_query, cnn);
                command.ExecuteNonQuery();
                storeposcmndlogs(_query,_fileid, filename);
                flag = "cancontinue";
            }
            catch (SqlException ex)
            {
                storenegcmndlogs(ex,_query,_fileid,  filename);
                flag = "cannotcontinue";
            }
            

            return flag;

      


        }


        public int DALgetlastfileid()
        {
            int id=0;
            string sql = "SELECT TOP 1 [fileid] FROM [dbo].[filelogs] ORDER BY [fileid] DESC";
            DALlogsconnect();
            
            SqlCommand newCommand = new SqlCommand(sql, cnn2);
            string output = Convert.ToString(newCommand.ExecuteScalar());
            if (output !="")
            {
                id = Int32.Parse(output);
            }
           cnn2.Close();
            return id;
        }



        public void DALenternewfilelog(int fileid, string filename, string filestatus, DateTime dtt, string UserName, string servername, string dbname)
        {
            string _queryforcmndlog = "Insert into filelogs values(" + fileid + ",'" + filename + "','" + filestatus + "','" + dtt + "','" + UserName + "','" + servername + "','"+dbname+"')";
            DALlogsconnect();
            
            var command = new SqlCommand(_queryforcmndlog, cnn2);
            command.ExecuteNonQuery();
           cnn2.Close();
        }



        public void DALupdatefilelog(int fileid, string filestatus)
        {
            cnn2.Open();
            SqlCommand cmd = new SqlCommand("update [dbo].[filelogs] set [filestatus] = ' " + filestatus + " ' where fileid =" + fileid, cnn2);
            cmd.ExecuteNonQuery();
            cnn2.Close();

        }



    }
}
