using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var t = Task.Factory.StartNew(RecieveLoop);
        }
        System.Net.Sockets.UdpClient udp =
                        new System.Net.Sockets.UdpClient(9999);
        delegate void ListViewEdit(ListViewItem item);
        void RecieveLoop()
        {
            while (true)
            {
                System.Net.IPEndPoint remoteEP = null;
                byte[] rcvBytes = udp.Receive(ref remoteEP);
                string rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);
                var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(rcvMsg);
                var item = new ListViewItem(DateTime.Now.ToShortTimeString());
                item.SubItems.Add(msg.Name);
                item.SubItems.Add(msg.Body);
                if (item.Name != System.Environment.UserName)
                {
                    Invoke(new ListViewEdit(CalcListView), item);
                    //listView1.Items.Add(item);//これだとエラー
                }

            }
        }
        void CalcListView(ListViewItem item)
        {
            listView1.Items.Add(item);
        }

        class Message
        {
            public string Name { get; set; }
            public string Body { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var obj = new Message()
            {
                Name = System.Environment.UserName,
                Body = textBox1.Text
            };
            string sendMsg = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
            udp.Send(sendBytes, sendBytes.Length, "192.168.10.255", 9999);
            textBox1.Text = "";
        }
    }
}
