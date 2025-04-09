using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Condez_SIS_v2
{
    public partial class Form2 : Form
    {
        private string connectionString = "Server= DESKTOP-G6J8QFP\\SQLEXPRESS01; Database= DcondezSISDB;Integrated Security= True;";
        public Form2()
        {
            InitializeComponent();
        }
        public void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT STUDENT_NO, NAMEOFSTUDENT, YEARSECTIONMAJOR, COURSE, BIRTHDAY, CONTACTNUMBER, ADDRESS, CONTACTPERSON, CONTACTPERSONADDRESS, CONTACTPERSONNUMBER, StudentImage FROM dbo.DBCONDEZ";


                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    dataGridView1.Columns["STUDENT_NO"].Width = 30;




                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void SearchbyCourse(string course)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string DAVECONDEZZ = "SELECT * FROM DBCONDEZ WHERE COURSE = @course";

                    using (SqlCommand command = new SqlCommand(DAVECONDEZZ, con))
                    {
                        command.Parameters.AddWithValue("@course", course);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            dataGridView1.DataSource = table;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void Search(string searchname)
        {
            string connectionString = "Server= DESKTOP-G6J8QFP\\SQLEXPRESS01; Database= DcondezSISDB;Integrated Security= True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM  DBCONDEZ WHERE NAMEOFSTUDENT LIKE @name";


                    using (SqlCommand command = new SqlCommand(query, con))
                    {

                        command.Parameters.AddWithValue("@name", "%" + searchname + "%");


                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);


                            dataGridView1.DataSource = table;
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem != null)
                {
                    string selectedCourse = comboBox1.SelectedItem.ToString();
                    SearchbyCourse(selectedCourse);
                }
                else
                {
                    MessageBox.Show("Please select a course before searching.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                Search(searchText);
            }
            else
            {
                MessageBox.Show("Please enter a name to search.");
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
                form1.Show();
            this.Hide();
        }
    }
}
