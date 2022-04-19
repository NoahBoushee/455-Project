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
        public string patientName = "";
        public string patientPassword = "";
        public Form2(string name, string pass)
        {
            connection = new SQLiteConnection("Data Source=455DB.db");
            connection.Open();
            patientName = name;
            patientPassword = pass;
            InitializeComponent();
            for (int i = 8; i < 18; i++)
            {
                comboBox1.Items.Add(i + ":00");
            }
            getStaff();
            updateLab();
        }

        private void updateLab()
        {
            dataGridView1.Rows.Clear();
            string patientID = getPatient();
            SQLiteCommand comm = new SQLiteCommand(connection);
            comm.CommandText = "Select LabID, Date, Result From LabPatient where PatientID = @p1;";
            comm.CommandType = CommandType.Text;
            comm.Parameters.Add(new SQLiteParameter("@p1", patientID));
            SQLiteDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                string ID = reader["LabID"].ToString();
                comm = new SQLiteCommand("Select TestName From LabTest where ID = @p2;", connection);
                comm.Parameters.Add(new SQLiteParameter("@p2", ID));
                SQLiteDataReader reader2 = comm.ExecuteReader();
                reader2.Read();
                string name = reader2["TestName"].ToString();
                dataGridView1.Rows.Add(new object[] {
                    name,  // Or column name like this
                    reader["Date"].ToString(),
                    reader["Result"].ToString()
                });
                reader2.Close();
            }
            reader.Close();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            connection.Close();
            this.Close();
        }

        private void getStaff()
        {
            comboBox2.Items.Clear();
            SQLiteCommand cmd = new SQLiteCommand(connection);
            cmd.CommandText = "SELECT Name FROM Staff;";
            cmd.CommandType = CommandType.Text;

            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comboBox2.Items.Add(reader["Name"].ToString());
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            label6.Text = "";
            comboBox1.Text = null;
            comboBox2.Text = null;
            textBox1.Text = "";
            comboBox1.Items.Clear();
            for (int i = 8; i < 18; i++)
            {
                comboBox1.Items.Add(i + ":00");
            }
            getStaff();
            label4.Text = "";
            string tempDate = e.Start.ToShortDateString().Replace("/", "-");
            label3.Text = "Selected Date: " + tempDate;
            SQLiteCommand cmd = new SQLiteCommand(connection);
            cmd.CommandText = "SELECT Date, Time, Provider FROM AppointmentInfo;";
            cmd.CommandType = CommandType.Text;

            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string date = reader["Date"].ToString();
                if (date == tempDate)
                {
                    string provider = reader["Provider"].ToString();
                    string time = reader["Time"].ToString();
                    comboBox1.Items.Remove(time);
                    SQLiteCommand cmd2 = new SQLiteCommand(connection);
                    cmd2.CommandText = "SELECT Name FROM Staff where ID = @name;";
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Parameters.Add(new SQLiteParameter("@name", provider));
                    SQLiteDataReader reader2 = cmd2.ExecuteReader();
                    reader2.Read();
                    label4.Text = label4.Text + "Time: " + time + " Provider: " + reader2["Name"] + "\n";
                    reader2.Close();
                }
               
            }
            reader.Close();
            //connection.Close();
        }

        private string getStaff(string name)
        {
            SQLiteCommand cmd2 = new SQLiteCommand(connection);
            cmd2.CommandText = "SELECT ID FROM Staff where Name = @name;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@name", name));
            SQLiteDataReader reader2 = cmd2.ExecuteReader();
            reader2.Read();
            string providerID = reader2["ID"].ToString();
            reader2.Close();
            return providerID;
        }

        private string getPatient()
        {
            string patientID = "";
            SQLiteCommand cmd2 = new SQLiteCommand(connection);
            cmd2.CommandText = "SELECT ID FROM PatientLogOn where Username = @name AND Password = @pass;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@name", patientName));
            cmd2.Parameters.Add(new SQLiteParameter("@pass", patientPassword));
            SQLiteDataReader reader2 = cmd2.ExecuteReader();
            reader2.Read();
            patientID = reader2["ID"].ToString();
            reader2.Close();
            return patientID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                label6.Text = "Please select a Time and/or Provider";
                return;
            }
            //GET STAFF ID
            SQLiteCommand cmd2 = new SQLiteCommand(connection);

            string providerID = getStaff(comboBox2.Text);
            //GET PATIENT ID
            string patientID = getPatient();
            //INSERT INTO APPOINTMENT INFO
            string date = monthCalendar1.SelectionRange.Start.ToString().Replace("/", "-");
            int temp = date.IndexOf(" ");
            date = date.Substring(0, temp);

            //CHECK IF PATIENT HAS APPOINTMENT ALREADY TODAY
            cmd2.CommandText = "SELECT COUNT(*) FROM AppointmentInfo WHERE PID=@p1 AND Date=@p2;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@p1", patientID));
            cmd2.Parameters.Add(new SQLiteParameter("@p2", date));
            int result = 0;
            result = Convert.ToInt32(cmd2.ExecuteScalar());

            if (result != 0)
            {
                label6.Text = "Appointment already scheduled for today.";
                return;
            }

            SQLiteCommand cmd3 = new SQLiteCommand(connection);
            cmd2.CommandText = "INSERT INTO AppointmentInfo (PID, Date, Time, Diagnosis, Stats, Provider, Reason) " +
                "VALUES(@PID, @Date, @Time, 'NA', 'Upcoming', @Provider, @Reason)";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@PID", patientID));
            cmd2.Parameters.Add(new SQLiteParameter("@Date", date));
            cmd2.Parameters.Add(new SQLiteParameter("@Time", comboBox1.SelectedItem));
            cmd2.Parameters.Add(new SQLiteParameter("@Provider", providerID));
            cmd2.Parameters.Add(new SQLiteParameter("@Reason", textBox1.Text));
            cmd2.ExecuteNonQuery();

            cmd2 = new SQLiteCommand(connection);
            cmd2.CommandText = "SELECT ID FROM AppointmentInfo where PID = @name AND Date = @pass;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@name", patientID));
            cmd2.Parameters.Add(new SQLiteParameter("@pass", date));
            SQLiteDataReader reader2 = cmd2.ExecuteReader();
            reader2.Read();
            string ID = reader2["ID"].ToString();
            reader2.Close();

            label6.Text = "Schedule appointment successfully";
            comboBox1.Text = null;
            comboBox2.Text = null;
            textBox1.Text = "";
            
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label11.Text = "";
            SQLiteCommand cmd2 = new SQLiteCommand(connection);
            string patientID = getPatient();
            string date = monthCalendar1.SelectionRange.Start.ToString().Replace("/", "-");
            int temp = date.IndexOf(" ");
            date = date.Substring(0, temp);
            cmd2.CommandText = "SELECT COUNT(*) FROM AppointmentInfo WHERE PID=@p1 AND Date=@p2;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@p1", patientID));
            cmd2.Parameters.Add(new SQLiteParameter("@p2", date));
            int result = 0;
            result = Convert.ToInt32(cmd2.ExecuteScalar());

            if (result == 0)
            {
                label11.Text = "No appointment for selected day.";
                return;
            }
            cmd2 = new SQLiteCommand(connection);
            cmd2.CommandText = "SELECT ID FROM AppointmentInfo where PID = @name AND Date = @pass;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@name", patientID));
            cmd2.Parameters.Add(new SQLiteParameter("@pass", date));
            SQLiteDataReader reader2 = cmd2.ExecuteReader();
            reader2.Read();
            string ID = reader2["ID"].ToString();
            reader2.Close();

            cmd2 = new SQLiteCommand(connection);
            cmd2.CommandText = "DELETE FROM AppointmentInfo where ID = @pass;";
            cmd2.CommandType = CommandType.Text;
            cmd2.Parameters.Add(new SQLiteParameter("@pass", ID));
            cmd2.ExecuteNonQuery();
            label11.Text = "Canceled successfully.";

        }
    }
}
