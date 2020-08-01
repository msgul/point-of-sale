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
    public partial class Graph : Form
    {

        private DataSet dataSet;
        private string sql;
        private NpgsqlDataAdapter add;

        public Graph()
        {
            InitializeComponent();
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            dataSet = new DataSet();
            sql = "select tarih,sum(fiyat) from kayıtlı group by tarih order by tarih desc";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            int i = 0;


            for (i = 0; i < 7; i++)
            {
                try
                {
                    string gün = dataSet.Tables[0].Rows[i]["tarih"].ToString();
                    string[] gün_ = gün.Split(' ');
                    string toplam = dataSet.Tables[0].Rows[i]["sum"].ToString();

                    chart1.Series["Toplam Satış"].Points.AddXY(gün_[0], Convert.ToDouble(toplam));
                }
                catch
                {
                    chart1.Series["Toplam Satış"].Points.AddXY("-",0);
                }
                
            }

        }
    }
}
