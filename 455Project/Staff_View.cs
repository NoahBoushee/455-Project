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
    public partial class Staff_View : Form
    {
        public SQLiteConnection connection;
        
        public Staff_View()
        {
            connection = new SQLiteConnection("Data Source=455DB.db");
            connection.Open();
            InitializeComponent();
            showdata();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            connection.Close();
            this.Close();
            LogIn f1 = new LogIn();
            f1.Show();
        }

        private void fillDataGrid(object sender, string e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            var selectStatement = $"SELECT Fname, Lname, DOB, SSN, Reason, Appt_Time, Provider FROM Patient INNER JOIN AppointmentInfo ON Patient.ID = AppointmentInfo.PID WHERE AppointmentInfo.Date = '{e}' AND AppointmentInfo.Provider = '{LogIn.user_id}' ORDER BY Lname";
            SQLiteCommand comm = new SQLiteCommand(selectStatement, connection);
            using (SQLiteDataReader read = comm.ExecuteReader())
            {
                while (read.Read())
                {
                    dataGridView1.Rows.Add(new object[] {
                    read.GetValue(read.GetOrdinal("Fname")),  // Or column name like this
                    read.GetValue(read.GetOrdinal("Lname")),
                    read.GetValue(read.GetOrdinal("DOB")),
                    read.GetValue(read.GetOrdinal("SSN")),
                    read.GetValue(read.GetOrdinal("Reason")),
                    read.GetValue(read.GetOrdinal("Appt_Time")),
                    });
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = dateTimePicker1.Value.Date;
            var date = dt.ToString("M-d-yyyy");                            
            fillDataGrid(this, date);
        }

        public void showdata()
        {
            DateTime dt = dateTimePicker1.Value.Date;
            var date = dt.ToString("M-d-yyyy");
            
            fillDataGrid(this, date);
            
            label1.Text = "Logged in as: " +  LogIn.username;
            label1.Font = new Font(label1.Font.Name, 12, FontStyle.Bold);
        }

        
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Patient_Encouter pe = new Patient_Encouter();
            pe.Show();
        }
    }
}
