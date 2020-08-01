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
    public partial class Fatura : Form
    {
        private DataSet dataSet;
        private string sql;
        private NpgsqlDataAdapter add;

        private int secilenId=0;

        public Fatura()
        {
            InitializeComponent();
        }

        private void faturaFill()
        {
            dataSet = new DataSet();
            sql = "select * from faturalar";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.RowHeadersVisible = false;
        }

        private void button11_Click(object sender, EventArgs e) // EKLE
        {
            string firm_adı = textBox1.Text;
            string firm_tutar = textBox2.Text;

            try
            {

                dataSet = new DataSet();
                sql = "insert into faturalar(firma_adı,tutar) values('" + firm_adı + "'," + firm_tutar + ")";
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);
            }
            catch
            {
                
            }
            
            textBox1.Text = "";
            textBox2.Text = "";

            faturaFill();

        }

        private void Fatura_Load(object sender, EventArgs e)
        {
            faturaFill();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (secilenId == 0)
                return;


            try
            {

                dataSet = new DataSet();
                sql = "delete from faturalar where Id = " + secilenId;
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);
            }
            catch
            {

            }

            textBox1.Text = "";
            textBox2.Text = "";

            secilenId = 0;

            faturaFill();
        }

        private void FaturaCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow menuitem = new DataGridViewRow();
                menuitem = dataGridView1.Rows[e.RowIndex];

                textBox1.Text = menuitem.Cells["Firma_Adı"].Value.ToString();
                textBox2.Text = menuitem.Cells["Tutar"].Value.ToString();
                secilenId = Convert.ToInt32(menuitem.Cells["Id"].Value.ToString());
            }
        }
    }
}
