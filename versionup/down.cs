using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static versionup.start;

namespace versionup
{
    public partial class down : Form
    {
        public down()
        {
            InitializeComponent();
        }
        public WebClient webClient = new WebClient();
        public List<versioninfo> infos;
        public down( List<versioninfo> versioninfos)
        {
            InitializeComponent();
            this.infos = versioninfos;
            this.progressBar1.Maximum = versioninfos.Count;
            new Thread(() =>
            {
                up(infos);
            }).Start();

        }
        public void up(List<versioninfo> listversions)
        {  
            listversions = listversions.OrderBy(x => x.vindex).ToList();
            int i = 0;int verindex = 0;string lastversion = "";
            foreach (var v in listversions)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string, int>((x, y) =>
                    {
                        label1.Text = x;
                        progressBar1.Value = y;
                    }), new object[] { v.version, i });
                }
               start.downfile(v.version, v.file,v.oprtype,v.downurl);
                i++;
                verindex = v.vindex; lastversion = v.version;
            }
            using (var con = getsqlitecon())
            {
                SQLiteCommand cmd = new SQLiteCommand("update info set value ="+verindex + " where key='versionindex'", con);
               int a= cmd.ExecuteNonQuery();
                 cmd = new SQLiteCommand("update info set value =" + lastversion + " where key='version'", con);
                cmd.ExecuteNonQuery();
            }
              Process.Start(Application.StartupPath + "\\main.exe", "123654");
          Application.Exit();
        }                     
    }
}
