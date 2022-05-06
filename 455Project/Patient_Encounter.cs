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
using Microsoft.Data.SqlClient;

namespace _455Project
{
    public partial class Patient_Encounter : Form
    {
        // Connection to Azure Database
        public Microsoft.Data.SqlClient.SqlConnection connectionString = new Microsoft.Data.SqlClient.SqlConnection(@"Data Source = csci455-emr.database.windows.net; Initial Catalog = csci455-emr; User ID = SuperUser; Password=Pword123!;Connect Timeout = 30; Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

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
