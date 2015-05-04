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
        private String connectionString = "Data Source=10.0.0.13;" +
                                            "Initial Catalog=fahrplanauskunft;" +
                                            "User id=fahrplanuser;" +
                                            "Password=Pa$$w0rd;";
        private AutoCompleteStringCollection bhf_namesCollection = new AutoCompleteStringCollection();

        public MainForm()
        {
            InitializeComponent();

            try {
                //befüllen der ListView mit Fahrplaneinträgen
                SqlConnection con = new SqlConnection();
                con.ConnectionString = this.connectionString;

                fillListView(con);
                con.Close();

                //befülle bhf_collection für die Auto-Vervollständigung
                SqlDataReader dReader;
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                "Select bhf_name from bahnhof";
                conn.Open();
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true) {
                    while (dReader.Read())
                        bhf_namesCollection.Add(dReader["bhf_name"].ToString());

                } else {
                    MessageBox.Show("ERROR: Data not found");
                }
                dReader.Close();

                txt_from.AutoCompleteMode = AutoCompleteMode.Suggest;
                txt_from.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txt_from.AutoCompleteCustomSource = bhf_namesCollection;
                txt_to.AutoCompleteMode = AutoCompleteMode.Suggest;
                txt_to.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txt_to.AutoCompleteCustomSource = bhf_namesCollection;
            } catch (Exception ex) {
                MessageBox.Show("Fehler beim Zugriff auf die Datenbank: \n\n"+ex.Message, "Database Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                try {
                    lst_overview.Items.Clear();
                    SqlConnection con = new SqlConnection();
                    con.ConnectionString = this.connectionString;
                    fillListView(con, "where bhf1.bhf_name = '"+txt_from.Text+"' and bhf2.bhf_name = '"+txt_to.Text+"'");

                    con.Close();
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
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

        #region Database-Methods

        private void fillListView(SqlConnection con) {
            //Funktion befüllt die ListView "lst_overview" mit allen Fahrplaneinträgen aus der Datenbank

            try {
                SqlDataAdapter fahrplanTableDA = new SqlDataAdapter("select Zugnummer, bhf1.bhf_name, bhf2.bhf_name, Ankunft_Datum, Abfahrt_Zeit, Verspaetung " +
                                                                        "from fahrplan inner join bahnhof bhf1 on fahrplan.Abfahrt_Bhf = bhf1.Bhf_ID " +
                                                                        "inner join bahnhof bhf2 on fahrplan.Ankunft_Bhf = bhf2.Bhf_ID", con);

                DataTable fahrplanDataTable = new DataTable();
                fahrplanTableDA.Fill(fahrplanDataTable);

                if (fahrplanDataTable.Rows.Count < 1) {
                    lst_overview.Items.Add("leider keine passende Verbindung gefunden..");
                }
                for (int i = 0; i < fahrplanDataTable.Rows.Count; i++) {
                    DataRow dr = fahrplanDataTable.Rows[i];
                    ListViewItem listitem = new ListViewItem(dr[0].ToString());
                    listitem.SubItems.Add(dr[1].ToString().PadLeft(3));
                    listitem.SubItems.Add(dr[2].ToString());
                    listitem.SubItems.Add(dr[3].ToString());
                    listitem.SubItems.Add(dr[4].ToString());
                    listitem.SubItems.Add(dr[5].ToString());
                    lst_overview.Items.Add(listitem);
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        private void fillListView(SqlConnection con, String where_clause) {
            //Funktion befüllt die ListView "lst_overview" mit Fahrplaneinträgen aus der Datenbank
            //where-klausel muss im Format "where 'x' = 'y'" angegeben werden.

            try {
                SqlDataAdapter fahrplanTableDA = new SqlDataAdapter("select Zugnummer, bhf1.bhf_name, bhf2.bhf_name, Ankunft_Datum, Abfahrt_Zeit, Verspaetung " +
                                                                        "from fahrplan inner join bahnhof bhf1 on fahrplan.Abfahrt_Bhf = bhf1.Bhf_ID " +
                                                                        "inner join bahnhof bhf2 on fahrplan.Ankunft_Bhf = bhf2.Bhf_ID "+
                                                                        where_clause, con);

                DataTable fahrplanDataTable = new DataTable();
                fahrplanTableDA.Fill(fahrplanDataTable);
                if (fahrplanDataTable.Rows.Count < 1) {
                    lst_overview.Items.Add("leider keine passende Verbindung gefunden..");
                }
                for (int i = 0; i < fahrplanDataTable.Rows.Count; i++) {
                    DataRow dr = fahrplanDataTable.Rows[i];
                    ListViewItem listitem = new ListViewItem(dr[0].ToString());
                    listitem.SubItems.Add(dr[1].ToString().PadLeft(3));
                    listitem.SubItems.Add(dr[2].ToString());
                    listitem.SubItems.Add(dr[3].ToString());
                    listitem.SubItems.Add(dr[4].ToString());
                    listitem.SubItems.Add(dr[5].ToString());
                    lst_overview.Items.Add(listitem);
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
    }

}
