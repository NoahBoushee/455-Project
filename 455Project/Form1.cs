﻿using System;
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

        public static string username;
        public static int user_id;

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
            if ((String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text)))
            {
                label3.Text = "Please fill form out completly";
                return;
            }
            label3.Text = "";
            // connect to databas
            //cmd object to write sql
            //adapter to read db data
            SQLiteConnection connection = new SQLiteConnection("Data Source=455DB.db");
            connection.Open();
            //CHECK IF data is in sql
            SQLiteCommand cmd = new SQLiteCommand(connection);
            if (radioButton1.Checked) {
                cmd.CommandText = "SELECT COUNT(*) FROM StaffLogOn WHERE Username=@p1 AND Password=@p2;";
                
            }
            else {
                cmd.CommandText = "SELECT COUNT(*) FROM PatientLogOn WHERE Username=@p1 AND Password=@p2;";
            }
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@p1", textBox1.Text));
            cmd.Parameters.Add(new SQLiteParameter("@p2", textBox2.Text));
            int count = 0;
            count = Convert.ToInt32(cmd.ExecuteScalar());
            
            if (count == 0)
            {
                label3.Text = "Invalid username or password";
                return;
            }
            // Assign username for logged in user
            username = textBox1.Text;
            
            connection.Close();
            if (radioButton1.Checked)
            {
                //GO TO THE STAFF VIEW
                label3.Text = "Logged in as Staff";
                Form3 f3 = new Form3();

                // assign user ID for staff
                connection.Open();
                var staff_id_selectStatement = $"SELECT ID FROM StaffLogon WHERE Username = '{LogIn.username}'";
                SQLiteCommand userStaffID = new SQLiteCommand(staff_id_selectStatement, connection);
                LogIn.user_id = Convert.ToInt32(userStaffID.ExecuteScalar());
                connection.Close();

                f3.Show();
                this.Hide();
                return;
            }
            //GO TO THE PATIENT VIEW
            label3.Text = "Logged in as Patient";
            Form2 f2 = new Form2(textBox1.Text, textBox2.Text);
            // assign user ID for patient
            connection.Open();
            var patient_id_selectStatement = $"SELECT ID FROM PatientLogOn WHERE Username = '{LogIn.username}'";
            SQLiteCommand userPatientID = new SQLiteCommand(patient_id_selectStatement, connection);
            LogIn.user_id = Convert.ToInt32(userPatientID.ExecuteScalar());
            connection.Close();

            f2.Show();
            this.Hide();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
