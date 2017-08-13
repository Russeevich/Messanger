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

namespace Chat
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string serverName = "79.174.44.240"; // Адрес сервера (для локальной базы пишите "localhost")
            string userName = "triu"; // Имя пользователя
            string dbName = "triumph"; //Имя базы данных
            string port = "3306"; // Порт для подключения
            string password = "120evazus"; // Пароль для подключения
            string connStr = "server=" + serverName +
            ";user=" + userName +
            ";database=" + dbName +
            ";port=" + port +
            ";password=" + password + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = "INSERT INTO chat (Nickname, Login, Password, Email)VALUES(@nick,@login,@pass,@email);"; // Строка запроса
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                string pass = Console.ReadLine();
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(textBox3.Text));
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                // Добавить параметры
                cmd.Parameters.AddWithValue("@nick", textBox1.Text);
                cmd.Parameters.AddWithValue("@login", textBox2.Text);
                cmd.Parameters.AddWithValue("@pass", result);
                cmd.Parameters.AddWithValue("@email", textBox4.Text);

                cmd.ExecuteNonQuery();
            }
            Form2 GA = new Form2();
            GA.Show();
            this.Hide();
        }

        private void Form3_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (Form1.MousePosition.Y <= this.Top + 31)
                {
                    this.Capture = false;
                    var msg = Message.Create(this.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
                    this.WndProc(ref msg);
                }
            }
            catch (Exception)
            {

            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
