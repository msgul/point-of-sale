using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BavyeraApp
{
    public partial class Ekle : Form
    {
        public int masano;

        public Ekle(int masaid)
        {
            InitializeComponent();
            masano = masaid;
        }

        private void Ekle_Load(object sender, EventArgs e)
        {

        }
    }
}
