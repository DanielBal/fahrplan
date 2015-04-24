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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogInForm().Show();
        }

        private void btn_search_Click(object sender, EventArgs e) {
            //just for testing || to be separated into a function

            if (txt_from.Text == "" || txt_to.Text == "") {
                MessageBox.Show("Bitte alle Felder ausfüllen!");
            } else {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = "Data Source=10.0.0.5;" +
                                        "Initial Catalog=fahrplanauskunft;" +
                                        "User id=fahrplanuser;" +
                                        "Password=Pa$$w0rd;";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from fahrplan";
                cmd.Connection = con;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                con.Close();

                MessageBox.Show(reader.GetString(0));
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show(@"extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen
                                extremnützlicheinformationen", "Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

    }

}
