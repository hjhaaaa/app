using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using versionup;

namespace main
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
            checkpwd();
        }
        private void checkpwd (){
            using (var con = Tool.getsqlitecon())
            {
                SQLiteCommand cmd = new SQLiteCommand("select value from info where key='user'", con);
                object var = cmd.ExecuteScalar();
                if (var != null)
                {
                    textBox1.Text = var.ToString();
                }


                  cmd = new SQLiteCommand($"select value from info where key='pwd'", con);
                object var2 = cmd.ExecuteScalar();
                if (var2 != null)
                {
                    textBox2.Text = var2.ToString();
                }
                cmd = new SQLiteCommand($"select value from info where key='remand'", con);
                object var3 = cmd.ExecuteScalar();
                if (var3!= null)
                {
                    if (var3.ToString() == "1")
                    {
                        checkBox1.Checked = true;
                    }
                  
                }



            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            using (HttpClient hc = new HttpClient())
            {
                var rep = hc.GetAsync($"{Model.Param.serviceurl}/api/login/login?user={textBox1.Text}&pwd={textBox2.Text}").Result;
                if (rep.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = rep.Content.ReadAsStringAsync().Result;
                    JObject job = JObject.Parse(result);
                    if (job["code"].ToString() == "1") {
                        using (var con = Tool.getsqlitecon())
                        {
                            SQLiteCommand cmd = new SQLiteCommand("select count(1) from info where key='user'", con);
                            string var = cmd.ExecuteScalar().ToString();
                            int count = Convert.ToInt16(cmd.ExecuteScalar());
                            if (count == 0)
                            {
                                cmd = new SQLiteCommand($"insert into info (key,value) values('user','{textBox1.Text.Trim()}')", con);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd = new SQLiteCommand($"update info set value='{textBox1.Text.Trim()}' where key='user'", con);
                                cmd.ExecuteNonQuery();
                            }
                        }
                            this.Hide();
                        f1 = new Form1(this);
                        f1.Show();
                        Model.Param.Mobile = textBox1.Text;
                    }
                    else
                    {
                        MessageBox.Show(job["msg"].ToString(), "提示");
                    }
                }
                
                //return false;
            }

            
            
        }

        private Form1 f1;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            string check = checkBox1.Checked == true ? "1" : "0";
                using (var con =Tool. getsqlitecon())
                {
                    SQLiteCommand cmd = new SQLiteCommand("select count(1) from info where key='remand'", con);
                string var = cmd.ExecuteScalar().ToString();
                    int count  = Convert.ToInt16( cmd.ExecuteScalar());
                    if(count==0)
                    {
                        cmd = new SQLiteCommand($"insert into info (key,value) values('remand','{check}')", con);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd = new SQLiteCommand($"update info set value='{check}' where key='remand'", con);
                        cmd.ExecuteNonQuery();
                    }
                    string pwd = checkBox1.Checked == true ? textBox2.Text.Trim() : "";
                   cmd = new SQLiteCommand("select count(1) from info where key='pwd'", con);
                    int count2 = Convert.ToInt16(cmd.ExecuteScalar());
                if (count2==0)
                    {
                        cmd = new SQLiteCommand($"insert into info (key,value) values('pwd','{pwd}')", con);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd = new SQLiteCommand($"update info set value='{pwd}' where key='pwd'", con);
                        cmd.ExecuteNonQuery();
                    }
                }                        
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            using (HttpClient hc = new HttpClient())
            {
                var rep = hc.GetAsync($"{Model.Param.serviceurl}/api/login/reg?user={textBox1.Text}&pwd={textBox2.Text}").Result;
                if (rep.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = rep.Content.ReadAsStringAsync().Result;
                    JObject job = JObject.Parse(result);
                    if (job["code"].ToString() == "1")
                    {
                       
                        MessageBox.Show(job["msg"].ToString(), "提示");
                    }
                    else
                    {
                        MessageBox.Show(job["msg"].ToString(), "提示");
                    }
                }

                //return false;
            }
        }
    }
}
