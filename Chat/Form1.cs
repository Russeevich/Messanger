using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Net;

namespace Chat
{
    public partial class Form1 : Form
    {
        private string fromname = null;
        string localname = null;
        TcpClient tcp = new TcpClient();
        private static IPAddress remoteIPAddress = IPAddress.Parse("79.174.44.240");
        private static int remotePort = 5001;
        private static int localPort = 5000;
        public Form1(string name)
        {
            InitializeComponent();
            localname = name;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Cursor=Cursors.Hand;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Cursor = Cursors.Hand;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            int x=60, y=90;
            string online,name = null;
            MySqlCommand command = new MySqlCommand(); ;
            string connectionString, commandString;
            connectionString = "Data source=79.174.44.240;UserId=triu;Password=120evazus;database=triumph;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            commandString = String.Format("SELECT Nickname,Online FROM chat WHERE Login!='{0}';",localname);
            command.CommandText = commandString;
            command.Connection = connection;
            MySqlDataReader reader;
                command.Connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                online = reader["Online"].ToString();
                    name = reader["Nickname"].ToString();
                        Label label = new Label();
                if (online == "true")
                {
                    PictureBox picture = new PictureBox();
                    picture.BackgroundImage = Chat.Properties.Resources.online;
                    picture.Location = new Point(x + 100, y + 10);
                    picture.Size = new Size(7, 7);
                    picture.BackColor = Color.FromArgb(0, 0, 0, 0);
                    this.Controls.Add(picture);
                }
                        label.Location = new Point(x, y);
                        label.BackColor = Color.FromArgb(0, 0, 0, 0);
                        label.Font = new System.Drawing.Font("Segoe print", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        label.ForeColor = Color.FromArgb(255, 255, 255);
                        y += 50;
                        label.Text = name;
                        label.MouseClick += cl;
                        this.Controls.Add(label);
                }
                reader.Close();
            command.Connection.Close();
        }

        private void cl(object sender, EventArgs e)
        {
            string s = sender.ToString();
            string a = s.Remove(0, s.IndexOf(':')+2);
            fromname = a;
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.Cursor = Cursors.Hand;
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            pictureBox4.Cursor = Cursors.Hand;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ты пидор!!!");
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            pictureBox5.Cursor = Cursors.Hand;
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            pictureBox6.Cursor = Cursors.Hand;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcp.Close();
            Application.Exit();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
          
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (Form1.MousePosition.Y <= this.Top+31)
                {
                    this.Capture = false;
                    var msg = Message.Create(this.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
                    this.WndProc(ref msg);
                }
            } catch(Exception)
            {

            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pictureBox9_MouseEnter(object sender, EventArgs e)
        {
            pictureBox9.Cursor = Cursors.Hand;
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {
            pictureBox8.Cursor = Cursors.Hand;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.TextLength > 10)
            {
                textBox1.ScrollBars = ScrollBars.Vertical;
                textBox1.WordWrap = true;
                textBox1.AcceptsReturn = true;
                textBox1.AcceptsTab = true;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            string Message = string.Format("/send from {0} to {1} :{2}", localname, fromname, textBox1.Text);
            Send(Message);
            textBox2.Text += localname + ":" + textBox1.Text + Environment.NewLine;
            textBox1.Text = null;
        }

        delegate void Del();

        public void Receiver()
        {
            Socket sock = tcp.Client;
            do
            {
                try
                {
                    byte[] data = new byte[1024];
                    sock.Receive(data);
                    if (data.Length > 0)
                    {
                        TextBox text = textBox2;
                        text.Invoke(new Del(() => text.Text += Encoding.UTF8.GetString(data)));
                    }
                }
                catch (Exception)
                {

                }
            } while (sock.Connected);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void Send(string datagram)
        {
            try
            {
                int element = 0;
                for (int i = 0; i < textBox1.TextLength; i++)
                    if (textBox1.Text.ElementAt(i) == ' ')
                        element++;
                Console.WriteLine("Колво пробелов {0}\nКолво символов {1}", element, textBox1.TextLength);
                if(element != textBox1.TextLength)
                {
                    Socket sock = tcp.Client;
                    sock.Send(Encoding.UTF8.GetBytes(datagram));
                }
            } catch(Exception){

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                tcp.Connect(remoteIPAddress, remotePort);
                Thread tRec = new Thread(new ThreadStart(Receiver));
                tRec.Name = "Receive";
                tRec.Start();
            } catch(Exception){
                MessageBox.Show("Ошибка подключения к серверу");
                Application.Exit();
            }
        }
    }
}
