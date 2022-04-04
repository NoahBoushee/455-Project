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

namespace _455Project
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text)))
            {
                label3.Text = "Please fill form out completly";
            }
            // connect to databas
            //cmd object to write sql
            //adapter to read db data
            SQLiteConnection connection = new SQLiteConnection("DATABASE PATH");
            //CHECK IF data is in sql
            string sqlCommand = "SELECT EXISTS(SELECT 1 FROM Patient WHERE userName = '" + textBox1.Text + "' AND password = '" + textBox2.Text + "');";
            SQLiteCommand cmd = new SQLiteCommand(sqlCommand, connection);
            string result = cmd.ExecuteScalar().ToString();
            if (!(result.ToUpper() == "TRUE"))
            {
                label3.Text = "Invalid usernam or password";
            }
            //GO TO THE NEXT FORM

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
