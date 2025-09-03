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

namespace DatabaseSetup_Search
{
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=FunDatabasename;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSearchIwaa_Click(object sender, EventArgs e)
        {
            if (txbSearchIwaa.Text == "")
            {
                FillDataGridView();
            }
            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }

                    using (DataTable datatable = new DataTable("Datatablepropertyname"))
                    {
                        using (SqlCommand command = new SqlCommand("SELECT * FROM PlayerRanking where ranking = @ranking", sqlConnection))
                        {
                            command.Parameters.AddWithValue("@ranking", txbSearchIwaa.Text);
                            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                            sqlDataAdapter.Fill(datatable);
                            dgvInformation.DataSource = datatable;
                        }
                    }
                }
            }
            ResetDgvSize();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            FillDataGridView();
        }

        private void FillDataGridView()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (sqlConnection.State == ConnectionState.Closed) { sqlConnection.Open(); }

                using (DataTable dataTable = new DataTable("Datatablepropertyname"))
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM PlayerRanking", sqlConnection))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(dataTable);
                        dgvInformation.DataSource = dataTable;
                    }
                }
            }
            ResetDgvSize();
        }
        private void ResetDgvSize()
        {
            dgvInformation.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            int totalWidth = dgvInformation.RowHeadersWidth;
            foreach (DataGridViewColumn column in dgvInformation.Columns)
            {
                totalWidth += column.Width;
            }

            dgvInformation.Width = totalWidth + 20;
            this.Width = totalWidth + 50;
        }

        private void btnResetIwaa_Click(object sender, EventArgs e)
        {
            txbSearchIwaa.Text = "";
            FillDataGridView();
        }
    }
}
