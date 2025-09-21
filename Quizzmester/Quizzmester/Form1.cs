using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quizzmester
{
    public partial class Form1 : Form
    {
        string playerAccountType = "";
        string questionPackCHosen = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnPlayAsGuestIwaa_Click(object sender, EventArgs e)
        {
            playerAccountType = "Guest";
            SelectCatagory("guest");
        }

        private void SelectCatagory (string type) 
        {
            
        }
    }
}
