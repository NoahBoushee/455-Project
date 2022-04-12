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
    public partial class Form2 : Form
    {
        public SQLiteConnection connection;
        public Form2()
        {
            connection = new SQLiteConnection("Data Source=455DB.db");
            connection.Open();
            InitializeComponent();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            connection.Close();
            this.Close();
        }
    }
}
