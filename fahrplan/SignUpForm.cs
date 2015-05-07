using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace fahrplan
{
    public partial class SignUpForm : Form
    {
        private String adminSignUp;

        public SignUpForm(bool isAdminSignUp)
        {
            InitializeComponent();
            this.adminSignUp = isAdminSignUp ? "true" : "false";
        }

        private void SignUpForm_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name, pass1, pass2;
            name = textBox1.Text;
            pass1 = textBox2.Text;
            pass2 = textBox3.Text;

            if (name == "" || pass1 == "" || pass2 == "") {
                MessageBox.Show("Bitte alle Felder ausfüllen!");
            } else {
                if (pass1 != pass2) {
                    MessageBox.Show("Passwörter sind unterschiedlich!");
                } else {

                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = fahrplan.Properties.Settings.Default.dbConnection;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        cmd.CommandText = "insert into dbo.Benutzer values ('" + name + "','" + pass1 + "','"+this.adminSignUp+"')";
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        this.DialogResult = DialogResult.OK;
                        Close();
                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("Benutzername ist schon verwendet!");
                    }                  
                }
            }
        }
    }
}
