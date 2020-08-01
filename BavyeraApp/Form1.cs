using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace BavyeraApp
{
    public partial class Form1 : Form
    {
        public string sqlId;
        public string sqlPass;
        public static NpgsqlConnection connection;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sqlId = textBox1.Text;
            sqlPass = textBox2.Text;

            connection = new NpgsqlConnection("Server=localhost; Port=5432; Database=bavyera; User Id=postgres; Password=6123571;");

            try
            {
                connection.Open();
            }
            catch(Exception Ex)
            {
                throw;
            }

            DataSet dataSet = new DataSet();
            string sql = "select * from menu";
            NpgsqlDataAdapter add = new NpgsqlDataAdapter(sql, connection);
            add.Fill(dataSet);

            Ana f2 = new Ana();
            connection.Close();
            this.Hide();
            f2.ShowDialog();
            this.Close();
        }
    }
}
