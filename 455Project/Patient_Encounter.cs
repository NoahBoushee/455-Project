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
    public partial class Patient_Encounter : Form
    {
        public SQLiteConnection connection;
        public Patient_Encounter()
        {
            InitializeComponent();
            connection = new SQLiteConnection("Data Source=455DB.db");
            connection.Open();
            fillListBox();
        }
        public void fillListBox()
        {
            var drugSelectionStatement = "SELECT * FROM Drug";
            SQLiteCommand drugFill = new SQLiteCommand(drugSelectionStatement, connection);

            SQLiteDataAdapter drugAdp = new SQLiteDataAdapter(drugFill);

            DataSet drugTable = new DataSet();
            drugAdp.Fill(drugTable);
            drugFill.ExecuteNonQuery();
            comboBox1.DataSource = drugTable.Tables[0];
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Description";

        }
    }
}
