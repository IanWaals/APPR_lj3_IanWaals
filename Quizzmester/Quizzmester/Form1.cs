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
        string currentCorrectAnswer = "";
        string currentCategory = "";
        int currentQuestionId = 0;

        int CorrectInARow = 0;
        int multiplier = 1;

        int score = 0;

        // For question tracking
        List<int> askedQuestionIds = new List<int>();
        List<int> availableQuestionIds = new List<int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set tags for category buttons
            btnBasketballIwaa.Tag = "Basketball";
            btnFootballIwaa.Tag = "American Football"; // match your DB category
            btnAllCategoriesIwaa.Tag = "All";

            btnBasketballIwaa.Click += CategoryButton_Click;
            btnFootballIwaa.Click += CategoryButton_Click;
            btnAllCategoriesIwaa.Click += CategoryButton_Click;

            // Add answer button handlers
            button1.Click += AnswerButton_Click;
            button2.Click += AnswerButton_Click;
            button3.Click += AnswerButton_Click;
            button4.Click += AnswerButton_Click;
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
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@accountType", "User");

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Registration successful!");
                                tbcScreensIwaa.SelectedIndex = 0;
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
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    try
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            playerAccountType = result.ToString();
                            MessageBox.Show("Login successful! " + playerAccountType);

                            tbcScreensIwaa.SelectedIndex = 3;
                        }
                        else
                        {
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

        // ---------------- QUIZ LOGIC ----------------

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string category)
            {
                currentCategory = category;
                tbcScreensIwaa.SelectedIndex = 4;

                LoadQuestionIds(category);
                LoadNextQuestion();
            }
        }

        private void LoadQuestionIds(string category)
        {
            askedQuestionIds.Clear();
            availableQuestionIds.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query;
                if (category == "All")
                    query = "SELECT Id FROM Questions";
                else
                    query = "SELECT Id FROM Questions WHERE Category = @category";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (category != "All")
                        command.Parameters.AddWithValue("@category", category);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            availableQuestionIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            // Shuffle the question IDs
            Random rng = new Random();
            availableQuestionIds = availableQuestionIds.OrderBy(x => rng.Next()).ToList();
        }

        private void LoadNextQuestion()
        {
            if (availableQuestionIds.Count == 0)
            {
                MessageBox.Show("No more questions left! Quiz complete 🎉");

                // Save the score to the GameLog
                AddScoreToGameLog();

                tbcScreensIwaa.SelectedIndex = 5;
                FillDataGridView();
                return;
            }


            int nextId = availableQuestionIds[0];
            availableQuestionIds.RemoveAt(0);
            askedQuestionIds.Add(nextId);

            QuestionData questionData = GetQuestionById(nextId);
            if (questionData != null)
            {
                currentQuestionId = questionData.QuestionId;
                prepareQuizTabIwaa(questionData);
            }

        }

        private QuestionData GetQuestionById(int questionId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Id, Question FROM Questions WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", questionId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new QuestionData
                            {
                                QuestionId = reader.GetInt32(0),
                                QuestionText = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            return null;
        }

        private void prepareQuizTabIwaa(QuestionData questionData)
        {
            lblQuestionIwaa.Text = questionData.QuestionText;
            lblScoreIwaa.Text = score.ToString();

            // Get all answers (shuffled)
            string correctAnswer;
            List<string> answers = getAllAnswers(questionData.QuestionId, out correctAnswer);

            // Store the correct answer
            currentCorrectAnswer = correctAnswer;

            // Assign answers to buttons
            if (answers.Count >= 1) button1.Text = answers[0];
            if (answers.Count >= 2) button2.Text = answers[1];
            if (answers.Count >= 3) button3.Text = answers[2];
            if (answers.Count >= 4) button4.Text = answers[3];

            // Enable buttons based on available answers
            button1.Enabled = answers.Count >= 1;
            button2.Enabled = answers.Count >= 2;
            button3.Enabled = answers.Count >= 3;
            button4.Enabled = answers.Count >= 4;
        }

        private List<string> getAllAnswers(int questionId, out string correctAnswer)
        {
            List<string> allAnswers = new List<string>();
            correctAnswer = "";
            int correctAnswerNumber = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get which answer is correct (1-4)
                string correctQuery = "SELECT CorrectAnswer FROM CorrectAnswers WHERE QuestionId = @questionId";
                using (SqlCommand command = new SqlCommand(correctQuery, connection))
                {
                    command.Parameters.AddWithValue("@questionId", questionId);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        correctAnswerNumber = Convert.ToInt32(result);
                    }
                }

                // Get all 4 answers from Answers table
                string answersQuery = "SELECT Answer1, Answer2, Answer3, Answer4 FROM Answers WHERE Id = @questionId";
                using (SqlCommand command = new SqlCommand(answersQuery, connection))
                {
                    command.Parameters.AddWithValue("@questionId", questionId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Add all non-null answers
                            if (!reader.IsDBNull(0)) allAnswers.Add(reader.GetString(0));
                            if (!reader.IsDBNull(1)) allAnswers.Add(reader.GetString(1));
                            if (!reader.IsDBNull(2)) allAnswers.Add(reader.GetString(2));
                            if (!reader.IsDBNull(3)) allAnswers.Add(reader.GetString(3));

                            // Determine which answer is correct
                            if (correctAnswerNumber >= 1 && correctAnswerNumber <= 4 && !reader.IsDBNull(correctAnswerNumber - 1))
                            {
                                correctAnswer = reader.GetString(correctAnswerNumber - 1);
                            }
                        }
                    }
                }
            }

            // Shuffle answers randomly
            Random rng = new Random();
            allAnswers = allAnswers.OrderBy(a => rng.Next()).ToList();

            return allAnswers;
        }

        private void AnswerButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string selectedAnswer = button.Text;

                if (selectedAnswer == currentCorrectAnswer)
                {
                    MessageBox.Show("Correct!");

                    CorrectInARow += 1;

                    if (CorrectInARow >= 5)
                    {
                        multiplier = 2;
                    }
                    else if (CorrectInARow >= 10)
                    {
                        multiplier = 3;
                    }

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string query = "SELECT Difficulty FROM Questions WHERE Id = @questionId";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                // You need to know which question is currently being answered
                                // The simplest way is to store it in a field when you load it.
                                command.Parameters.AddWithValue("@questionId", currentQuestionId);

                                object result = command.ExecuteScalar();
                                if (result != null)
                                {
                                    string difficulty = result.ToString().ToLower();

                                    if (difficulty == "e")
                                        score += (1 * multiplier);
                                    else if (difficulty == "m")
                                        score += (2 * multiplier);
                                    else if (difficulty == "h")
                                        score += (3 * multiplier);
                                }
                            }
                        }

                    lblScoreIwaa.Text = score.ToString(); // ✅ update the label on screen
                }
                else
                {
                    MessageBox.Show("Wrong! The correct answer was: " + currentCorrectAnswer);
                }

                LoadNextQuestion();
            }
        }

        private void FillDataGridView()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed) { sqlConnection.Open(); }

                using (DataTable dataTable = new DataTable("Datatablepropertyname"))
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM GameLog", sqlConnection))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                        dgvScoreBoardIwaa.DataSource = dataTable;
                    }
                }
            }
        }

        private void AddScoreToGameLog()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO GameLog (UserName, PointsScored, CategoryPlayed, HighestStreak)
                         VALUES (@username, @points, @category, @streak)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // If the player is a guest, we can store "Guest" or empty string
                    string usernameToSave = playerAccountType == "guest" ? "Guest" : txbUsernameLoginIwaa.Text;

                    command.Parameters.AddWithValue("@username", usernameToSave);
                    command.Parameters.AddWithValue("@points", score);
                    command.Parameters.AddWithValue("@category", currentCategory);
                    command.Parameters.AddWithValue("@streak", CorrectInARow);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save score: " + ex.Message);
                    }
                }
            }
        }

    }
}
