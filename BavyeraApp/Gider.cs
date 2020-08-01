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
    public partial class Gider : Form
    {
        private int secilenId=0;
        private DataSet dataSet;
        private string sql;
        private NpgsqlDataAdapter add;

        public Gider()
        {
            InitializeComponent();
        }

        private void Gider_Load(object sender, EventArgs e)
        {
            dataSet = new DataSet();
            sql = "select * from gider";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.RowHeadersVisible = false;
        }

        private void button1_Click_1(object sender, EventArgs e) // EKLE
        {
            //
            secilenId = 0;
        }

        private void button2_Click(object sender, EventArgs e) // SİL
        {
            //
            secilenId = 0;
        }

        private void GiderCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow menuitem = new DataGridViewRow();
                menuitem = dataGridView1.Rows[e.RowIndex];

                secilenId = Convert.ToInt32(menuitem.Cells["Id"].Value.ToString());
            }
        }
    }
}
