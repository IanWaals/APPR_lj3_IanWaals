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

        // Timer variables
        private Timer shotClockTimer;
        private Timer gameClockTimer;
        private int shotClockSeconds = 5;
        private int gameClockSeconds = 0;
        private int totalGameTime = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeTimers();
        }

        private void InitializeTimers()
        {
            // Shot clock timer (counts down from 5)
            shotClockTimer = new Timer();
            shotClockTimer.Interval = 1000; // 1 second
            shotClockTimer.Tick += ShotClockTimer_Tick;

            // Game clock timer (counts up to total time)
            gameClockTimer = new Timer();
            gameClockTimer.Interval = 1000; // 1 second
            gameClockTimer.Tick += GameClockTimer_Tick;
        }

        private void ShotClockTimer_Tick(object sender, EventArgs e)
        {
            shotClockSeconds--;
            UpdateShotClockDisplay();

            if (shotClockSeconds <= 0)
            {
                shotClockTimer.Stop();
                HandleTimeExpired();
            }
        }

        private void GameClockTimer_Tick(object sender, EventArgs e)
        {
            gameClockSeconds++;
            UpdateGameClockDisplay();

            // Optional: End game if total time exceeded
            if (gameClockSeconds >= totalGameTime)
            {
                StopTimers();
                MessageBox.Show("Time's up! Game over.");
                EndQuiz();
            }
        }

        private void UpdateShotClockDisplay()
        {
            // Update the label - you'll need to add this label to your form
            if (lblShotClockIwaa != null)
            {
                lblShotClockIwaa.Text = $"Time: {shotClockSeconds}s";

                // Change color when time is running out
                if (shotClockSeconds <= 5)
                    lblShotClockIwaa.ForeColor = Color.Red;
                else if (shotClockSeconds <= 10)
                    lblShotClockIwaa.ForeColor = Color.Orange;
                else
                    lblShotClockIwaa.ForeColor = Color.White;
            }
        }

        private void UpdateGameClockDisplay()
        {
            // Update the label - you'll need to add this label to your form
            if (lblGameClockIwaa != null)
            {
                int remainingSeconds = totalGameTime - gameClockSeconds;
                int minutes = remainingSeconds / 60;
                int seconds = remainingSeconds % 60;
                lblGameClockIwaa.Text = $"Total Time: {minutes:D2}:{seconds:D2}";
            }
        }

        private void HandleTimeExpired()
        {
            MessageBox.Show("Time's up! The correct answer was: " + currentCorrectAnswer);
            CorrectInARow = 0; // Reset streak
            multiplier = 1; // Reset multiplier
            LoadNextQuestion();
        }

        private void StopTimers()
        {
            shotClockTimer.Stop();
            gameClockTimer.Stop();
        }

        private void StartTimers()
        {
            shotClockSeconds = 5;
            shotClockTimer.Start();
            gameClockTimer.Start();
            UpdateShotClockDisplay();
            UpdateGameClockDisplay();
        }

        private void EndQuiz()
        {
            StopTimers();
            AddScoreToGameLog();
            tbcScreensIwaa.SelectedIndex = 5;
            FillDataGridView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set tags for category buttons
            btnBasketballIwaa.Tag = "Basketball";
            btnFootballIwaa.Tag = "Football"; // match your DB category
            btnAllCategoriesIwaa.Tag = "All";

            btnBasketballIwaa.Click += CategoryButton_Click;
            btnFootballIwaa.Click += CategoryButton_Click;
            btnAllCategoriesIwaa.Click += CategoryButton_Click;

            // Add answer button handlers
            button1.Click += AnswerButton_Click;
            button2.Click += AnswerButton_Click;
            button3.Click += AnswerButton_Click;
            button4.Click += AnswerButton_Click;

            cbxLeaderboardIWaa.SelectedIndexChanged += cbxLeaderboardIWaa_SelectedIndexChanged;

            tbcScreensIwaa.Appearance = TabAppearance.FlatButtons;
            tbcScreensIwaa.ItemSize = new Size(0, 1);
            tbcScreensIwaa.SizeMode = TabSizeMode.Fixed;

            LoadUsers();
            LoadQuestions();
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

                            tbcScreensIwaa.Appearance = TabAppearance.Normal;
                            tbcScreensIwaa.ItemSize = new Size(0, 0);
                            tbcScreensIwaa.SizeMode = TabSizeMode.Normal;

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

                // Initialize game clock based on number of questions
                totalGameTime = 60;
                gameClockSeconds = 0;
                score = 0; // Reset score
                CorrectInARow = 0;
                multiplier = 1;

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
                StopTimers();
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

                // Start/restart the shot clock for this question
                shotClockSeconds = 5;
                shotClockTimer.Start();

                // Start game clock if first question
                if (!gameClockTimer.Enabled)
                {
                    gameClockTimer.Start();
                }

                UpdateShotClockDisplay();
                UpdateGameClockDisplay();
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
                // Stop the shot clock when answer is selected
                shotClockTimer.Stop();

                string selectedAnswer = button.Text;

                if (selectedAnswer == currentCorrectAnswer)
                {
                    MessageBox.Show("Correct!");

                    CorrectInARow += 1;

                    if (CorrectInARow >= 10)
                    {
                        multiplier = 3;
                    }
                    else if (CorrectInARow >= 5)
                    {
                        multiplier = 2;
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT Difficulty FROM Questions WHERE Id = @questionId";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
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

                    lblScoreIwaa.Text = score.ToString();
                }
                else
                {
                    MessageBox.Show("Wrong! The correct answer was: " + currentCorrectAnswer);
                    CorrectInARow = 0;
                    multiplier = 1;
                }

                LoadNextQuestion();
            }
        }

        private void FillDataGridView()
        {
            string selectedCategory = cbxLeaderboardIWaa?.Text;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                using (DataTable dataTable = new DataTable("Datatablepropertyname"))
                {
                    string query;
                    SqlCommand command;

                    // If no category selected or "All" selected, show all records
                    if (string.IsNullOrEmpty(selectedCategory) || selectedCategory == "All Categories")
                    {
                        query = "SELECT * FROM GameLog ORDER BY PointsScored DESC";
                        command = new SqlCommand(query, sqlConnection);
                    }
                    else
                    {
                        query = "SELECT * FROM GameLog WHERE CategoryPlayed = @selectedCategory ORDER BY PointsScored DESC";
                        command = new SqlCommand(query, sqlConnection);
                        command.Parameters.AddWithValue("@selectedCategory", selectedCategory);
                    }

                    using (command)
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

                string query = @"INSERT INTO GameLog (UserName, PointsScored, CategoryPlayed)
                         VALUES (@username, @points, @category)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // If the player is a guest, we can store "Guest" or empty string
                    string usernameToSave = playerAccountType == "guest" ? "Guest" : txbUsernameLoginIwaa.Text;

                    command.Parameters.AddWithValue("@username", usernameToSave);
                    command.Parameters.AddWithValue("@points", score);
                    command.Parameters.AddWithValue("@category", currentCategory);

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

        private void cbxLeaderboardIWaa_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGridView();
        }

        private void btnSkipIwaa_Click(object sender, EventArgs e)
        {
            btnSkipIwaa.Enabled = false;
            btnSkipIwaa.Visible = false;
            lblSkipIwaa.Visible = false;
            LoadNextQuestion();
        }

        private void btnCreateUserIwaa_Click(object sender, EventArgs e)
        {
            string username = txbAdminUserNameIwaa.Text;
            string password = txbAdminPasswordIwaa.Text;
            string accounttype = "";

            if (rdbAdminIwaa.Checked)
            {
                accounttype = "Admin";
            }
            else if (rdbAdminIwaa.Checked)
            {
                accounttype = "User";
            }

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
                    command.Parameters.AddWithValue("@accountType", accounttype);

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

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, UserName FROM UserData";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Columns.Contains("Id") && dt.Columns.Contains("UserName"))
                {
                    cbxRemoveUserIwaa.DataSource = dt;
                    cbxRemoveUserIwaa.DisplayMember = "UserName";
                    cbxRemoveUserIwaa.ValueMember = "Id";
                    cbxRemoveUserIwaa.SelectedIndex = -1; // optional
                }
                else
                {
                    MessageBox.Show("Expected columns not found in query result.");
                }
            }
        }

        private void btnRemoveUserIwaa_Click(object sender, EventArgs e)
        {
            string UserToRemove = cbxRemoveUserIwaa.Text;

            if (string.IsNullOrWhiteSpace(UserToRemove))
            {
                MessageBox.Show("Please select a user to remove.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM UserData WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", UserToRemove);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show("User removed successfully!");
                    else
                        MessageBox.Show("User not found.");
                }
            }
        }

        private void LoadQuestions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Question FROM Questions";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Columns.Contains("Id") && dt.Columns.Contains("Question"))
                {
                    cbxRemoveQuestionIwaa.DataSource = dt;
                    cbxRemoveQuestionIwaa.DisplayMember = "Question";
                    cbxRemoveQuestionIwaa.ValueMember = "Id";
                    cbxRemoveQuestionIwaa.SelectedIndex = -1; // optional

                    cbxEditQuestionIwaa.DataSource = dt;
                    cbxEditQuestionIwaa.DisplayMember = "Question";
                    cbxEditQuestionIwaa.ValueMember = "Id";
                    cbxEditQuestionIwaa.SelectedIndex = -1; // optional
                }
                else
                {
                    MessageBox.Show("Expected columns not found in query result.");
                }

            }
        }

        private void btnRemoveQuestionIwaa_Click(object sender, EventArgs e)
        {
            string QuestionToRemove = cbxRemoveQuestionIwaa.Text;

            if (string.IsNullOrWhiteSpace(QuestionToRemove))
            {
                MessageBox.Show("Please select a question to remove.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Get the Question ID
                        string getIdQuery = "SELECT Id FROM Questions WHERE Question = @Question";
                        int questionId = -1;

                        using (SqlCommand getIdCommand = new SqlCommand(getIdQuery, connection, transaction))
                        {
                            getIdCommand.Parameters.AddWithValue("@Question", QuestionToRemove);
                            object result = getIdCommand.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("Question not found.");
                                return;
                            }
                            questionId = Convert.ToInt32(result);
                        }

                        // Step 2: Delete related entries from CorrectAnswers
                        string deleteCorrectAnswersQuery = "DELETE FROM CorrectAnswers WHERE QuestionId = @QuestionId";
                        using (SqlCommand deleteCorrectAnswersCmd = new SqlCommand(deleteCorrectAnswersQuery, connection, transaction))
                        {
                            deleteCorrectAnswersCmd.Parameters.AddWithValue("@QuestionId", questionId);
                            deleteCorrectAnswersCmd.ExecuteNonQuery();
                        }

                        // Step 3: Delete related entries from Answers
                        string deleteAnswersQuery = "DELETE FROM Answers WHERE Id = @QuestionId";
                        using (SqlCommand deleteAnswersCmd = new SqlCommand(deleteAnswersQuery, connection, transaction))
                        {
                            deleteAnswersCmd.Parameters.AddWithValue("@QuestionId", questionId);
                            deleteAnswersCmd.ExecuteNonQuery();
                        }

                        // Step 4: Delete from Questions
                        string deleteQuestionQuery = "DELETE FROM Questions WHERE Id = @QuestionId";
                        using (SqlCommand deleteQuestionCmd = new SqlCommand(deleteQuestionQuery, connection, transaction))
                        {
                            deleteQuestionCmd.Parameters.AddWithValue("@QuestionId", questionId);
                            int rowsAffected = deleteQuestionCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                MessageBox.Show("Question and related data removed successfully!");
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Question not found or could not be removed.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("An error occurred while removing the question: " + ex.Message);
                    }
                }
            }
        }

        private void btnAddQuestionIwaa_Click(object sender, EventArgs e)
        {
            // Collect variables from add question form
            string Question = txbQuestionIwaa.Text;
            string Category = cbxAddCategoryIwaa.Text;
            string Difficulty = "";
            string Answer1 = txbAnswer1Iwaa.Text;
            string Answer2 = txbAnswer2Iwaa.Text;
            string Answer3 = txbAnswer3Iwaa.Text;
            string Answer4 = txbAnswer4Iwaa.Text;

            // Validate user input
            if (string.IsNullOrWhiteSpace(Question) ||
                string.IsNullOrWhiteSpace(Category) ||
                string.IsNullOrWhiteSpace(Answer1) ||
                string.IsNullOrWhiteSpace(Answer2) ||
                string.IsNullOrWhiteSpace(Answer3) ||
                string.IsNullOrWhiteSpace(Answer4) ||
                cbxCorrectAnswerIwaa.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields before adding a question.");
                return;
            }

            // Get difficulty as single letter
            switch (cbxDifficultyIwaa.Text)
            {
                case "Easy":
                    Difficulty = "e";
                    break;
                case "Medium":
                    Difficulty = "m";
                    break;
                case "Hard":
                    Difficulty = "h";
                    break;
                default:
                    MessageBox.Show("Please select a difficulty.");
                    return;
            }

            int CorrectAnswer = Convert.ToInt32(cbxCorrectAnswerIwaa.SelectedItem);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Add question and get its new ID
                    int questionId = addQuestion(Question, Category, Difficulty, connection, transaction);

                    // Upload answers and correct answer
                    uploadAnswers(Answer1, Answer2, Answer3, Answer4, questionId, connection, transaction);
                    uploadCorrectAnswer(questionId, CorrectAnswer, connection, transaction);

                    // Commit all inserts
                    transaction.Commit();
                    MessageBox.Show("Question added successfully!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private int addQuestion(string Question, string Category, string Difficulty, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO Questions (Question, Category, Difficulty)
        VALUES (@Question, @Category, @Difficulty);
        SELECT CAST(SCOPE_IDENTITY() AS int);
    ";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Question", Question);
                command.Parameters.AddWithValue("@Category", Category);
                command.Parameters.AddWithValue("@Difficulty", Difficulty);

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception("Failed to retrieve the new Question ID.");
                }
            }
        }

        private void uploadAnswers(string Answer1, string Answer2, string Answer3, string Answer4, int questionId, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "INSERT INTO Answers (Id, Answer1, Answer2, Answer3, Answer4) VALUES (@QuestionId, @Answer1, @Answer2, @Answer3, @Answer4)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@QuestionId", questionId);
                command.Parameters.AddWithValue("@Answer1", Answer1);
                command.Parameters.AddWithValue("@Answer2", Answer2);
                command.Parameters.AddWithValue("@Answer3", Answer3);
                command.Parameters.AddWithValue("@Answer4", Answer4);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to add answers.");
                }
            }
        }

        private void uploadCorrectAnswer(int questionId, int correctAnswer, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "INSERT INTO CorrectAnswers (QuestionId, CorrectAnswer) VALUES (@questionId, @correctAnswer)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@questionId", questionId);
                command.Parameters.AddWithValue("@correctAnswer", correctAnswer);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to add correct answer.");
                }
            }
        }

        private void cbxEditQuestionIwaa_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedQuestion = cbxEditQuestionIwaa.Text;

            if (string.IsNullOrWhiteSpace(selectedQuestion))
                return;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get question ID and details
                string questionQuery = "SELECT Id, Question, Category, Difficulty FROM Questions WHERE Question = @selectedQuestion";
                int questionId = -1;

                using (SqlCommand command = new SqlCommand(questionQuery, connection))
                {
                    command.Parameters.AddWithValue("@selectedQuestion", selectedQuestion);

                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            questionId = reader.GetInt32(0); // Get the Id
                            txbEditQuestionIwaa.Text = reader["Question"].ToString();
                            cbxEditCategoryIwaa.Text = reader["Category"].ToString();

                            string difficulty = reader["Difficulty"].ToString().ToLower();
                            if (difficulty == "e") cbxEditDifficultyIwaa.Text = "Easy";
                            else if (difficulty == "m") cbxEditDifficultyIwaa.Text = "Medium";
                            else if (difficulty == "h") cbxEditDifficultyIwaa.Text = "Hard";
                        }
                        else
                        {
                            MessageBox.Show("Question not found.");
                            return;
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading question details: " + ex.Message);
                        return;
                    }
                }

                // Get answers
                string answersQuery = "SELECT Answer1, Answer2, Answer3, Answer4 FROM Answers WHERE Id = @questionId";
                using (SqlCommand command = new SqlCommand(answersQuery, connection))
                {
                    command.Parameters.AddWithValue("@questionId", questionId);

                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            txbEditAnswer1Iwaa.Text = reader["Answer1"].ToString();
                            txbEditAnswer2Iwaa.Text = reader["Answer2"].ToString();
                            txbEditAnswer3Iwaa.Text = reader["Answer3"].ToString();
                            txbEditAnswer4.Text = reader["Answer4"].ToString();
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading answers: " + ex.Message);
                    }
                }

                // Get correct answer
                string correctAnswerQuery = "SELECT CorrectAnswer FROM CorrectAnswers WHERE QuestionId = @questionId";
                using (SqlCommand command = new SqlCommand(correctAnswerQuery, connection))
                {
                    command.Parameters.AddWithValue("@questionId", questionId);

                    try
                    {
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            cbxEditCorrectAnswerIwaa.Text = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading correct answer: " + ex.Message);
                    }
                }
            }
        }

        private void btnEditQuestionIwaa_Click(object sender, EventArgs e)
        {
            // Get the selected question ID
            if (cbxEditQuestionIwaa.SelectedValue == null)
            {
                MessageBox.Show("Please select a question to edit.");
                return;
            }

            int questionId = Convert.ToInt32(cbxEditQuestionIwaa.SelectedValue);

            // Collect variables from edit question form
            string Question = txbEditQuestionIwaa.Text;
            string Category = cbxEditCategoryIwaa.Text;
            string Difficulty = "";
            string Answer1 = txbEditAnswer1Iwaa.Text;
            string Answer2 = txbEditAnswer2Iwaa.Text;
            string Answer3 = txbEditAnswer3Iwaa.Text;
            string Answer4 = txbEditAnswer4.Text;

            // Validate user input
            if (string.IsNullOrWhiteSpace(Question) ||
                string.IsNullOrWhiteSpace(Category) ||
                string.IsNullOrWhiteSpace(Answer1) ||
                string.IsNullOrWhiteSpace(Answer2) ||
                string.IsNullOrWhiteSpace(Answer3) ||
                string.IsNullOrWhiteSpace(Answer4) ||
                cbxEditCorrectAnswerIwaa.SelectedItem == null ||
                cbxEditDifficultyIwaa.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields before updating the question.");
                return;
            }

            // Get difficulty as single letter
            switch (cbxEditDifficultyIwaa.Text)
            {
                case "Easy":
                    Difficulty = "e";
                    break;
                case "Medium":
                    Difficulty = "m";
                    break;
                case "Hard":
                    Difficulty = "h";
                    break;
                default:
                    MessageBox.Show("Please select a difficulty.");
                    return;
            }

            int CorrectAnswer = Convert.ToInt32(cbxEditCorrectAnswerIwaa.SelectedItem);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Update the question
                    string updateQuestionQuery = @"UPDATE Questions 
                                          SET Question = @Question, 
                                              Category = @Category, 
                                              Difficulty = @Difficulty 
                                          WHERE Id = @QuestionId";

                    using (SqlCommand command = new SqlCommand(updateQuestionQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Question", Question);
                        command.Parameters.AddWithValue("@Category", Category);
                        command.Parameters.AddWithValue("@Difficulty", Difficulty);
                        command.Parameters.AddWithValue("@QuestionId", questionId);

                        command.ExecuteNonQuery();
                    }

                    // Update the answers
                    string updateAnswersQuery = @"UPDATE Answers 
                                         SET Answer1 = @Answer1, 
                                             Answer2 = @Answer2, 
                                             Answer3 = @Answer3, 
                                             Answer4 = @Answer4 
                                         WHERE Id = @QuestionId";

                    using (SqlCommand command = new SqlCommand(updateAnswersQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Answer1", Answer1);
                        command.Parameters.AddWithValue("@Answer2", Answer2);
                        command.Parameters.AddWithValue("@Answer3", Answer3);
                        command.Parameters.AddWithValue("@Answer4", Answer4);
                        command.Parameters.AddWithValue("@QuestionId", questionId);

                        command.ExecuteNonQuery();
                    }

                    // Update the correct answer
                    string updateCorrectAnswerQuery = @"UPDATE CorrectAnswers 
                                               SET CorrectAnswer = @CorrectAnswer 
                                               WHERE QuestionId = @QuestionId";

                    using (SqlCommand command = new SqlCommand(updateCorrectAnswerQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CorrectAnswer", CorrectAnswer);
                        command.Parameters.AddWithValue("@QuestionId", questionId);

                        command.ExecuteNonQuery();
                    }

                    // Commit all updates
                    transaction.Commit();
                    MessageBox.Show("Question updated successfully!");

                    // Refresh the question list
                    LoadQuestions();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error updating question: " + ex.Message);
                }
            }
        }
    }
}