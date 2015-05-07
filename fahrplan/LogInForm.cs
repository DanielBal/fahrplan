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

        private String connectionString = "Data Source=10.130.20.102;" +
                                            "Initial Catalog=fahrplanauskunft;" +
                                            "User id=fahrplanuser;" +
                                            "Password=Pa$$w0rd;";
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
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = connectionString;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select Count(*) as count from Benutzer where IsAdmin = 1 and Benutzer_ID = '" + name + "'  and Passwort = '" + pass + "'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    while (dReader.Read()) {
                        int result = int.Parse(dReader["count"].ToString());
                        if (result > 0) {
                            //eingeloggt
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        } else {
                            MessageBox.Show("Benutzername oder Passwort ungültig!");
                        }
                    }
                } catch (Exception ex) {
                    MessageBox.Show("Verbindungsfehler: \n\n" + ex.Message, "Database Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

                
        }

        private void label4_Click(object sender, EventArgs e)
        {
            new SignUpForm().Show();
            Close();
        }
    }
}
