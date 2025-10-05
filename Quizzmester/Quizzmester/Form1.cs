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

namespace Quizzmester
{
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=FunDatabasename;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        string playerAccountType = "";
        string questionPackCHosen = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnBasketballIwaa.Tag = "Basketball";
            btnFootballIwaa.Tag = "Football";
            btnAllCategoriesIwaa.Tag = "All";

            btnBasketballIwaa.Click += CategoryButton_Click;
            btnFootballIwaa.Click += CategoryButton_Click;
            btnAllCategoriesIwaa.Click += CategoryButton_Click;

        }
        private void btnLoginIwaa_Click(object sender, EventArgs e)
        {
            tbcScreensIwaa.SelectedIndex = 2;
        }

        private void btnRegisterIwaa_Click(object sender, EventArgs e)
        {
            tbcScreensIwaa.SelectedIndex = 1;
        }

        private void btnConfirmRegisterIwaa_Click(object sender, EventArgs e)
        {
            string username = txbUserNameRegisterIwaa.Text;
            string password = txbPasswordRegisterIwaa.Text;

            if (password == txbConfirmPasswordRegisterIwaa.Text)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string query = "INSERT INTO UserData (UserName, Password, AccountType) VALUES (@username, @password, @accountType)";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@accountType", "User"); // or use a variable if dynamic

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Registration successful!");
                                tbcScreensIwaa.SelectedIndex = 0; // Go back to start screen
                            }
                            else
                            {
                                MessageBox.Show("Registration failed. Please try again.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Fault: Passwords do not match");
            }
        }

        private void btnConfirmLoginIwaa_Click(object sender, EventArgs e)
        {
            string username = txbUsernameLoginIwaa.Text;
            string password = txbPasswordLoginIwaa.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "SELECT AccountType FROM UserData WHERE UserName = @username AND Password = @password";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Always use parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    try
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Login successful
                            playerAccountType = result.ToString();
                            MessageBox.Show("Login successful!" + playerAccountType);

                            tbcScreensIwaa.SelectedIndex = 3;
                        }
                        else
                        {
                            // No matching user found
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void btnPlayAsGuestIwaa_Click(object sender, EventArgs e)
        {
            playerAccountType = "guest";
            tbcScreensIwaa.SelectedIndex = 3;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string category)
            {
                tbcScreensIwaa.SelectedIndex = 4;
                string question = getNewQuestion(category);
                prepareQuizTabIwaa(question);
            }
        }

        private void prepareQuizTabIwaa(string Question)
        {
            lblQuestionIwaa.Text = Question;

        }

        private string getNewQuestion(string category)
        {
            int questionNr = 1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                if (category == "All")
                {
                    //fetch question from database
                    string query = "SELECT Question FROM Questions WHERE Id = @questionNr";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@questionNr", questionNr); // don’t forget this

                        try
                        {
                            object result = command.ExecuteScalar();

                            if (result != null)
                            {
                                questionNr++;
                                string question = result.ToString();
                                return question;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error fetching question: " + ex.Message);
                        }
                    }
                }
            }
            return null; // return null if no question found
        }
    }
}
