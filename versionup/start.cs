using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Configuration;
using Model;

namespace versionup
{
    public class start
    {
       
        public static string localdownurl = Application.StartupPath + "//";
       // public static string domain = System.Configuration.ConfigurationManager.AppSettings["domain"].ToString();
        internal static List<versioninfo> checkversion()
        {
            using (var con = getsqlitecon())
            {
                SQLiteCommand cmd = new SQLiteCommand("select value from info where key='versionindex'", con);
                string versionindex = "";
                SQLiteDataReader reader= cmd.ExecuteReader();
                while(reader.Read())
                {
                    versionindex = reader["value"].ToString();
                }
                using (HttpClient hc = new HttpClient())
                {
                    var rep = hc.GetAsync(Param.serviceurl + "api/compareveison/cop?version=" + versionindex).Result;
                    if (rep.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = rep.Content.ReadAsStringAsync().Result;
                        JArray array = JArray.Parse(result);
                        List<versioninfo> vinfos = new List<versioninfo>();
                        foreach (var arr in array)
                        {
                            versioninfo info = new versioninfo
                            {
                                version = arr["version"].ToString(),
                                vindex = arr["vindex"].Value<int>(),
                                file = arr["file"].ToString(),
                                oprtype = arr["oprtype"].Value<int>(),
                                 downurl=arr["downurl"].ToString()

                            };
                            vinfos.Add(info);
                        }
                        return vinfos;
                    }
                    else
                    {
                        List<versioninfo> vinfos = new List<versioninfo>();
                        return vinfos;
                    }
                }
            }                         
            
        }
        public static SQLiteConnection getsqlitecon()
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=param.db");
            con.Open();
            return con;
        }
        public static void downfile(string version,string file,int mode,string downurl)
        {
            if (File.Exists(downurl + file))
            {
                File.Delete(downurl + file);
            }               
             if(mode==1)
            {
                WebClient client = new WebClient();
                
                client.DownloadFile(Param.serviceurl + downurl, localdownurl+ file);
            }
        }
        public static void refreshlogo()
        {
            using (HttpClient hc = new HttpClient())
            {
                try
                {
                    var rep = hc.GetAsync(Param.serviceurl + "api/download/logo").Result;
                    if (rep.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = rep.Content.ReadAsStringAsync().Result;
                        using (var con = getsqlitecon())
                        {
                            SQLiteCommand cmd = new SQLiteCommand("update info set value =" + result.Trim() + " where key='logo'", con);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show("连接服务器error or up sqlite error");
                    
                }
                finally
                {
                    Application.Exit();
                }
               

            }
        }
        public static string getlogo()
        {
            using (var con = getsqlitecon())
            {
                SQLiteCommand cmd = new SQLiteCommand("select value from info where key='logo'", con);
                return cmd.ExecuteScalar().ToString();
            }
        }
        public static bool login(string user,string pwd)
        {
            using (HttpClient hc = new HttpClient())
            {
                var rep = hc.GetAsync(Param.serviceurl + $"login/login?user={user}&pwd={pwd}").Result;
                if (rep.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = rep.Content.ReadAsStringAsync().Result;
                    if (result == "1") { return true; }
                }
                return false;
            }
        }
    }
}
