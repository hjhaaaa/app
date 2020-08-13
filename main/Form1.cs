using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net;

namespace main
{
    public partial class Form1 : Form
    {
        public Form1(login login)
        {
            this.log = login;
            InitializeComponent();
        }
        private login log;
        public MyNote note;
        private void Form1_Load(object sender, EventArgs e)
        {
            using (var con = Tool.getsqlitecon())
            {
                SQLiteCommand cmd = new SQLiteCommand("select value from info where key='logo'", con);
                pictureBox1.ImageLocation = cmd.ExecuteScalar().ToString();
                cmd = new SQLiteCommand("select value from info where key='version'", con);
                this.Text= cmd.ExecuteScalar().ToString();
            }                                    
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            log.Close();
            Application.Exit();
        }

        private void btnNote_Click(object sender, EventArgs e)
        {
            if(note==null||note.IsDisposed)
            {
                note = new MyNote();
                note.Show();
            }
            else
            {
                note.Show();
            }
        }
    }
    public class Tool
    {
        public static SQLiteConnection getsqlitecon()
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=param.db");
            con.Open();
            return con;
        }
        
    }
}
