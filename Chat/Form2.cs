using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Net;

namespace Chat
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Form3 GA = new Form3();
            GA.Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string passw = null;
            MySqlCommand command = new MySqlCommand(); ;
            string connectionString, commandString;
            connectionString = "Data source=79.174.44.240;UserId=triu;Password=120evazus;database=triumph;";
            MySqlConnection connection2 = new MySqlConnection(connectionString);
            StreamReader readers;
            HttpWebRequest httpWebRequest;
            HttpWebResponse httpWebResponse;
            string IP = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://checkip.dyndns.org");
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                readers = new StreamReader(httpWebResponse.GetResponseStream());
                IP = System.Text.RegularExpressions.Regex.Match(readers.ReadToEnd(), @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})").Groups[1].Value;
            }
            catch (Exception)
            {

            }
            string sss = String.Format("UPDATE chat SET Ip ='{0}',Online='true' WHERE Login='{1}'",IP, textBox1.Text);
            MySqlCommand cmd = new MySqlCommand(sss, connection2);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            MySqlConnection connection = new MySqlConnection(connectionString);
            commandString = "SELECT * FROM chat WHERE Login=\"" + textBox1.Text + "\"";
            command.CommandText = commandString;
            command.Connection = connection;
            MySqlDataReader reader;
            try
            {
                command.Connection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    passw = reader["Password"].ToString();
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: \r\n{0}", ex.ToString());
            }
            finally
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                string pass = Console.ReadLine();
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text));
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                command.Connection.Close();
                if (passw == result)
                {
                    Form1 GA = new Form1(textBox1.Text);
                    GA.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Wrong login/password");
                }
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
