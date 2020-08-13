using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace main
{
    public partial class MyNote : Form
    {
        public MyNote()
        {
            InitializeComponent();
        }
        public void refreshlistbox()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Mobile", Param.Mobile);
            string result = HttpClientReq.Req("g", dic, "api/note/list");
            JArray array = JArray.Parse(result);
            List<ListViewItem> listViewItems = new List<ListViewItem>();
            listBox1.Items.Clear();
            foreach (var v in array)
            {
                ListViewItem item = new ListViewItem();
                item.Text = v["Title"].ToString();
                item.Tag = v["Id"].Value<int>();
                item.SubItems.Add(v["Notes"].ToString());
                listViewItems.Add(item);
                listBox1.Items.Add(item);
            }                      
            listBox1.DisplayMember = "Title";
        }
        private void MyNote_Load(object sender, EventArgs e)
        {
            refreshlistbox();
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var box = sender as ListBox;
            ListViewItem a = box.SelectedItem as ListViewItem;
            if(a!=null)
            {
                textBox1.Text = a.Text;
                textBox2.Text = a.SubItems[1].Text;
            }          
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //1 add 2up 3 del
        //保存note
        private void button1_Click(object sender, EventArgs e)
        {
            var a = (ListViewItem)(listBox1.SelectedItem);
            var id = (int)a.Tag;
            var mobile = Param.Mobile;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Mobile", Param.Mobile);
            dic.Add("Id", id.ToString());
            dic.Add("title", textBox1.Text);
            dic.Add("note", textBox2.Text);
            dic.Add("isadd", "2");
            string result = HttpClientReq.Req("g", dic, "api/note/notesave");
            JObject jObject = JObject.Parse(result);
            MessageBox.Show("ok");
            refreshlistbox();
        }
        //添加note
        private void button2_Click(object sender, EventArgs e)
        {
            addNote addNote = new addNote();
            DialogResult result = addNote.ShowDialog();
            string title = addNote.textBox1.Text;
            string note = addNote.textBox2.Text;
            var mobile = Param.Mobile;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Mobile", Param.Mobile);
            dic.Add("Id", "0");
            dic.Add("title", title);
            dic.Add("note", note);
            dic.Add("isadd", "1");
            string jresult = HttpClientReq.Req("g", dic, "api/note/notesave");
            if (jresult == "fail") { MessageBox.Show("fail"); }
            else
            {
                JObject jObject = JObject.Parse(jresult);
                MessageBox.Show("ok");
            }
            refreshlistbox();
        }
        //删除note
        private void Button3_Click(object sender, EventArgs e)
        {
           var a=   (ListViewItem)(listBox1.SelectedItem)  ;
            var id = (int)a.Tag;
            var mobile = Param.Mobile;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Mobile", Param.Mobile);
            dic.Add("Id", id.ToString());
            dic.Add("title", "");
            dic.Add("note", "");
            dic.Add("isadd", "3");
            string result = HttpClientReq.Req("g", dic, "api/note/notesave");
            JObject jObject = JObject.Parse(result);
            MessageBox.Show("ok");
            refreshlistbox();
        }
    }
}
