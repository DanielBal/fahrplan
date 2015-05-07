using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fahrplan
{
    public partial class LogInForm : Form
    {
        public bool isAdmin = false;
        public String user_ID = null;

        public LogInForm()
        {
            InitializeComponent();
        }

        private void LogInForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name, pass;
            name = textBox1.Text;
            pass = textBox2.Text;
          
            if (name=="" | pass==""){
                    MessageBox.Show("Bitte alle Felder ausfüllen!");
            } else {
                try {
                    SqlDataReader dReader;
                    bool bExists = false;
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = fahrplan.Properties.Settings.Default.dbConnection;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select IsAdmin as admin from Benutzer where Benutzer_ID = '" + name + "' and Passwort = '" + pass + "'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                     
                    while (dReader.Read()) {
                        if (dReader["admin"].ToString().ToLower().Equals("true")) {
                            //setze User auf admin
                            this.isAdmin = true;
                        } 
                        //eingeloggt
                        this.user_ID = name;
                        this.DialogResult = DialogResult.OK;
                        bExists = true;
                        this.Close();
                    }
                    if (!bExists)
                        MessageBox.Show("Benutzername oder Passwort ungültig!");
                } catch (Exception ex) {
                    MessageBox.Show("Verbindungsfehler: \n\n" + ex.Message, "Database Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e) {
            this.Visible = false;
            using (SignUpForm signUpForm = new SignUpForm()) {
                if (signUpForm.ShowDialog() == DialogResult.OK) {

                }
            }
        }
    }
}
