using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Data.Linq;



namespace Condez_SIS_v2
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server= DESKTOP-G6J8QFP\\SQLEXPRESS01; Database= DcondezSISDB;Integrated Security= True;";
        private SqlConnection con;
        private string selectedImagePath = "";
        private string currentStudentImagePath = "";
        DataClasses1DataContext db = new DataClasses1DataContext();

        public Form1()
        {
            InitializeComponent();
            con = new SqlConnection(connectionString);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dateTimePicker1.ValueChanged += new EventHandler(dateTimePicker1_ValueChanged);
        }
        private void MoveToNextControl(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, false);
            }

        }
        private void AddKeyDownEvent(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox)
                {
                    ctrl.KeyDown += MoveToNextControl;
                }
                else if (ctrl.HasChildren)
                {
                    AddKeyDownEvent(ctrl.Controls);
                }
            }
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
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            AddKeyDownEvent(this.Controls);
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        ((TextBox)ctrl).KeyDown += MoveToNextControl;
                    }
                }
            }


        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        private void SaveImage()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE DBCONDEZ SET StudentImage = @image WHERE STUDENT_NO = @studentNo", con))
                    {
                        if (pictureBox1.Image != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                                byte[] imgData = ms.ToArray();
                                cmd.Parameters.Add("@image", SqlDbType.VarBinary).Value = imgData;

                            }
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@image", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@studentNo", textBox1.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private void LoadImage(string studentNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT StudentImage FROM DBCONDEZ WHERE STUDENT_NO = @studentNo", con))
                    {
                        cmd.Parameters.AddWithValue("@studentNo", studentNo);
                        byte[] imageBytes = cmd.ExecuteScalar() as byte[];
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            pictureBox1.Image = ByteArrayToImage(imageBytes);
                        }
                        else
                        {
                            pictureBox1.Image = null;

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void DeleteStudent(string studentNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleleStudent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@STUDENT_NO", studentNo);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime birthDate = dateTimePicker1.Value;
            int age = CalculateAge(birthDate);
            textBox7.Text = age.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "(Auto-Generated)";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            pictureBox1.Image = null;
            textBox8.Text = "";
            textBox9.Text = "";
            textBox2.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string newImagePath = "";


                if (pictureBox1.Image != null)
                {

                    string studentName = textBox2.Text.Trim().Replace(" ", "_");
                    string currentDate = DateTime.Now.ToString("yyyyMMdd");
                    string newFileName = studentName + "_" + currentDate + ".jpg";
                    string saveDirectory = @"C:\Users\dave\Pictures\StudentPic";

                    if (!Directory.Exists(saveDirectory))
                    {
                        Directory.CreateDirectory(saveDirectory);
                    }


                    if (!string.IsNullOrEmpty(currentStudentImagePath))
                    {
                        try
                        {

                            string searchPattern = studentName + "_*.jpg";
                            string[] oldFiles = Directory.GetFiles(saveDirectory, searchPattern);

                            foreach (string oldFile in oldFiles)
                            {
                                try
                                {
                                    File.Delete(oldFile);
                                }
                                catch (IOException ex)
                                {
                                    Console.WriteLine("Could not delete old file: " + ex.Message);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error finding old files: " + ex.Message);

                        }
                    }


                    newImagePath = Path.Combine(saveDirectory, newFileName);


                    using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                    {
                        bmp.Save(newImagePath, ImageFormat.Jpeg);
                    }
                }


                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_updatestudent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@STUDENT_NO", textBox1.Text);
                        cmd.Parameters.AddWithValue("@NAMEOFSTUDENT", textBox2.Text);
                        cmd.Parameters.AddWithValue("@YEARSECTIONMAJOR", textBox3.Text);
                        cmd.Parameters.AddWithValue("@COURSE", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@BIRTHDAY", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@CONTACTNUMBER", textBox6.Text);
                        cmd.Parameters.AddWithValue("@ADDRESS", textBox5.Text);
                        cmd.Parameters.AddWithValue("@CONTACTPERSON", textBox4.Text);
                        cmd.Parameters.AddWithValue("@CONTACTPERSONADDRESS", textBox9.Text);
                        cmd.Parameters.AddWithValue("@CONTACTPERSONNUMBER", textBox8.Text);

                        if (pictureBox1.Image != null)
                        {

                            byte[] imgData;
                            using (Image img = Image.FromFile(newImagePath))
                            using (MemoryStream ms = new MemoryStream())
                            {
                                img.Save(ms, ImageFormat.Jpeg);
                                imgData = ms.ToArray();
                            }

                            SqlParameter imgParam = cmd.Parameters.Add("@StudentImage", SqlDbType.VarBinary, imgData.Length);
                            imgParam.Value = imgData;
                        }
                        else
                        {
                            SqlParameter imgParam = cmd.Parameters.Add("@StudentImage", SqlDbType.VarBinary);
                            imgParam.Value = DBNull.Value;
                        }

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }


                if (!string.IsNullOrEmpty(newImagePath))
                {
                    currentStudentImagePath = newImagePath;
                }

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string studentNo = dataGridView1.SelectedRows[0].Cells["STUDENT_NO"].Value.ToString();

                DialogResult result = MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DeleteStudent(studentNo);
                }
            }
            else
            {
                MessageBox.Show("Please select a student to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Initialize DataContext
                DataClasses1DataContext db = new DataClasses1DataContext();

                // Create a new student record
                DBCONDEZ newStudent = new DBCONDEZ
                {
                    NAMEOFSTUDENT = textBox2.Text,
                    YEARSECTIONMAJOR = textBox3.Text,
                    COURSE = comboBox1.Text,
                    BIRTHDAY = dateTimePicker1.Value,
                    CONTACTNUMBER = textBox6.Text,
                    ADDRESS = textBox5.Text,
                    CONTACTPERSON = textBox4.Text,
                    CONTACTPERSONADDRESS = textBox9.Text,
                    CONTACTPERSONNUMBER = textBox8.Text
                };

                // Add and save changes
                db.DBCONDEZs.InsertOnSubmit(newStudent);
                db.SubmitChanges();
                LoadData();

                MessageBox.Show("Student added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    textBox1.Text = row.Cells["STUDENT_NO"].Value.ToString();
                    textBox2.Text = row.Cells["NAMEOFSTUDENT"].Value.ToString();
                    textBox3.Text = row.Cells["YEARSECTIONMAJOR"].Value.ToString();
                    comboBox1.Text = row.Cells["COURSE"].Value.ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(row.Cells["BIRTHDAY"].Value);
                    textBox6.Text = row.Cells["CONTACTNUMBER"].Value.ToString();
                    textBox5.Text = row.Cells["ADDRESS"].Value.ToString();
                    textBox4.Text = row.Cells["CONTACTPERSON"].Value.ToString();
                    textBox9.Text = row.Cells["CONTACTPERSONADDRESS"].Value.ToString();
                    textBox8.Text = row.Cells["CONTACTPERSONNUMBER"].Value.ToString();


                    if (row.Cells["StudentImage"].Value != DBNull.Value && row.Cells["StudentImage"].Value != null)
                    {
                        byte[] imageBytes = (byte[])row.Cells["StudentImage"].Value;
                        pictureBox1.Image = ByteArrayToImage(imageBytes);


                        string studentName = row.Cells["NAMEOFSTUDENT"].Value.ToString().Trim().Replace(" ", "_");
                        currentStudentImagePath = Path.Combine(@"C:\Users\dave\Pictures\StudentPic", studentName + "_*");
                    }
                    else
                    {
                        pictureBox1.Image = null;
                        currentStudentImagePath = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong " + ex);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (open.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = open.FileName;
                pictureBox1.Image = Image.FromFile(selectedImagePath);
            }
        }
    }
}
