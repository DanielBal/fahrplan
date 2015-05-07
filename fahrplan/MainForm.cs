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
        private AutoCompleteStringCollection bhf_namesCollection = new AutoCompleteStringCollection();
        bool isAdmin = false;
        bool isLoggedIn = false;
        String user_ID;

        public MainForm()
        {
            InitializeComponent();

            //insert default values for some components
            txt_hours.Text = DateTime.Now.Hour.ToString();
            txt_minutes.Text = DateTime.Now.Minute.ToString();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd-MM-yyyy";

            try {
                //befüllen der ListView mit Fahrplaneinträgen
                SqlConnection con = new SqlConnection();
                con.ConnectionString = fahrplan.Properties.Settings.Default.dbConnection;

                fillListView(con);
                lst_overview.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lst_overview.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                con.Close();

                //befülle bhf_collection für die Auto-Vervollständigung
                SqlDataReader dReader;
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = fahrplan.Properties.Settings.Default.dbConnection;
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
            lbl_info.Text = "";
            if (!this.isLoggedIn) {
                //show log in dialog:
                using (LogInForm loginForm = new LogInForm()) {
                    if (loginForm.ShowDialog() == DialogResult.OK) {
                        //logged in
                        //now show a log off button instead of a log in
                        logInToolStripMenuItem.Text = "Log off";
                        this.isAdmin = loginForm.isAdmin;
                        this.user_ID = loginForm.user_ID;
                        this.isLoggedIn = true;
                        if (this.isAdmin) {
                            verwaltenToolStripMenuItem.Visible = true;
                        }
                    }
                }
            } else {
                this.isLoggedIn = false;
                logInToolStripMenuItem.Text = "Log in";
                verwaltenToolStripMenuItem.Visible = false;
            }
        }

        private void btn_search_Click(object sender, EventArgs e) {
            lbl_info.Text = "";

            if (txt_from.Text == "" || txt_to.Text == "") {
                lbl_info.Text = "Bitte alle Felder ausfüllen!";
            } else {
                try {
                    lst_overview.Items.Clear();
                    //build the where clause
                    StringBuilder whereClauseBuilder = new StringBuilder("where bhf1.bhf_name = '");
                    whereClauseBuilder.Append(txt_from.Text);
                    whereClauseBuilder.Append("' and bhf2.bhf_name = '");
                    whereClauseBuilder.Append(txt_to.Text);
                    whereClauseBuilder.Append("' and Ankunft_Datum >= '");
                    whereClauseBuilder.Append(dateTimePicker1.Text);
                    whereClauseBuilder.Append("'");
                    if (txt_hours.Text.Length > 0 && txt_minutes.Text.Length > 0
                        && isNumberBetween(int.Parse(txt_hours.Text), 0, 24)
                        && isNumberBetween(int.Parse(txt_minutes.Text), 0, 59)) {
                        whereClauseBuilder.Append(" and Abfahrt_Zeit >= '");
                        whereClauseBuilder.Append(txt_hours.Text);
                        whereClauseBuilder.Append(":");
                        whereClauseBuilder.Append(txt_minutes.Text);
                        whereClauseBuilder.Append("' order by Ankunft_Datum, Abfahrt_Zeit");
                    } else {
                        lbl_info.Text = "Bitte geben Sie eine gültige Abfahrtszeit an!";
                    }
                    SqlConnection con = new SqlConnection();
                    con.ConnectionString = fahrplan.Properties.Settings.Default.dbConnection;
                    //query the database
                    fillListView(con, whereClauseBuilder.ToString());

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
                    lbl_info.Text = "leider keine passende Verbindung gefunden..";
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
                    lbl_info.Text = "leider keine passende Verbindung gefunden..";
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

        private void txt_hours_keyPressed(object sender, KeyPressEventArgs e) {
            if (!char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void txt_minutes_keyPressed(object sender, KeyPressEventArgs e) {
            if (!char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private bool isNumberBetween(int num, int min, int max) {
            //inklusive Grenzen
            return num >= min && num <= max ? true : false;
        }

        private void benutzerHinzufuegenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SignUpForm signupform = new SignUpForm(true);
            signupform.Show();
        }

        private void lst_overview_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }

}
