using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Drawing.Printing;

namespace BavyeraApp
{
    public partial class Ana : Form
    {
        private int secilenMenuId=0;
        private bool adisyonozeti = false;
        private int secilenUstId = 0;
        Font butonFont = new Font("Calibri", 14);
        private int masano; // Tıklanmış olan masanın numarası
        private double masahesap=0;
        private static StreamReader dosyaAkimi;
        private int SipId=0;
        private Button clickedBut;
        private DataGridViewRow Tıklanan;
        DataSet dataSet;
        string sql;
        NpgsqlDataAdapter add;

        public Ana()
        {
            InitializeComponent();
        }

        private void MatrixButtonClick(object sender, EventArgs e)
        {
            if (sender is Button)
            {
               
                Button b = sender as Button;
                clickedBut = b;
                int masaid = Convert.ToInt32(b.Tag) + 1; // Basılan masanın numarası
                masano = masaid;

                FillData();
                SipId = 0;

                label2.Text = masano + "";

                if (b.BackColor == Color.LightPink)
                {
                    Point ptLowerLeft = new Point(0, b.Height);
                    ptLowerLeft = b.PointToScreen(ptLowerLeft);
                    contextMenuStrip1.Show(ptLowerLeft);
                }
            }
        }

        private double hesapGuncelle() // DEĞİŞTİRİLEN MASAYI GÜNCELLER
        {
            double toplam = 0;
            dataSet = new DataSet();
            sql = "select sum(Fiyat) from anlik where masaId =" + masano;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            if (!dataSet.Tables[0].Rows[0].IsNull(0))
            {
                clickedBut.BackColor = Color.LightBlue;
                toplam = Convert.ToDouble(dataSet.Tables[0].Rows[0]["sum"]);
            }
            else
            { 
                  toplam = 0;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                button10.Enabled = false;
            }

            if (toplam == 0 && clickedBut.BackColor!=Color.LightPink)
                clickedBut.BackColor = Color.LightGreen;

            clickedBut.Text = "MASA "+Convert.ToString(masano) + "\n\n" + Convert.ToString(toplam) + "  TL";

            masahesap = toplam;

            label4.Text = toplam + " TL";

            return toplam;
        }

        private void Yazdır(int rapor)
        {
            dosyaAkimi = new System.IO.StreamReader("C:\\Print.txt");

            PrintDocument PD = new PrintDocument();

            if(rapor==0)
                PD.PrintPage += new PrintPageEventHandler(OnPrintDocument); //hesap çıkarma
            else
                PD.PrintPage += new PrintPageEventHandler(OnPrintGunsonu);  //günsonu alma


            /////////////////////////////////////////////////////////////////////////
            PrintDialog printdlg = new PrintDialog();
            PrintPreviewDialog printPrvDlg = new PrintPreviewDialog();
            
            // preview the assigned document or you can create a different previewButton for it
            printPrvDlg.Document = PD;
            printPrvDlg.ShowDialog(); // this shows the preview and then show the Printer Dlg below

            printdlg.Document = PD;
            
            if (printdlg.ShowDialog() == DialogResult.OK)
            {
                PD.Print();
            }
            //////////////////////////////////////////////////////////////////////////


            /*
            try
            {
                PD.Print();
            }
            catch
            {
                Console.WriteLine("YAZICI HATASI...");
            }
            finally
            {
                PD.Dispose();
            }
            */
        }

        private void FillData()
        {
            dataSet = new DataSet();
            sql = "select a.Id,m.isim,a.Adet,a.Fiyat from anlik a,menu m where masaId = " + masano + " and m.Id = sipId";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            dataGridView1.DataSource = dataSet.Tables[0];
            this.dataGridView1.DefaultCellStyle.Font = new Font("Ariel", 12);
            dataGridView1.Columns["Id"].Visible = false;

            dataGridView1.ClearSelection();

            dataSet = new DataSet();
            sql = "select sum(fiyat) from anlik";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            if (!dataSet.Tables[0].Rows[0].IsNull(0))
                masahesap = Convert.ToDouble(dataSet.Tables[0].Rows[0]["sum"]);
            else
                masahesap = 0;

            if(clickedBut.BackColor==Color.LightPink)
            {
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                button13.Enabled = false;
            }
            else if(hesapGuncelle()==0)
            {
                button13.Enabled = true;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                button10.Enabled = false;
            }
            else
            {
                button13.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                button9.Enabled = true;
                button10.Enabled = true;
            }
        }

        private void Ana_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;
            flowLayoutPanel1.Visible = false;
            panel1.Visible = false;
            panel2.Visible = false;

            dataSet = new DataSet();
            sql = "select count(*) from menuler";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            int üstcount = Convert.ToInt32(dataSet.Tables[0].Rows[0]["count"]);

            ToolStripMenuItem[] üstmenü = new ToolStripMenuItem[üstcount];

            dataSet = new DataSet();
            sql = "select * from menuler";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            
            for(int i=0;i<üstcount;i++)
            {
                üstmenü[i] = new ToolStripMenuItem();
                üstmenü[i].Text = Convert.ToString(dataSet.Tables[0].Rows[i]["isim"]);
                menuStrip1.Items.Add(üstmenü[i]);
            }
            

            masano = 0;
            double toplam=0;
            button13.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled =false;

            int tag = 0;
            Button[,] tables = new Button[6,2];
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 2; j++)
                {
                    tables[i, j] = new Button
                    {
                        BackColor = Color.LightGreen, // boş masa açık renk
                        Location = new Point(45 + j * 200, 35 + i * 100),
                        Width = 170,
                        Height = 75,
                        Text = Convert.ToString((i * 2 + j) + 1)
                    };
                    //masanın durumuna göre renk ver

                    tables[i, j].Tag = tag++;
                    tables[i, j].MouseClick += MatrixButtonClick;

                    dataSet = new DataSet();
                    sql = "select sum(Fiyat) from anlik where masaId =" + Convert.ToString((i * 2 + j) + 1);
                    add = new NpgsqlDataAdapter(sql, Form1.connection);
                    add.Fill(dataSet);
                    if (!dataSet.Tables[0].Rows[0].IsNull(0))
                    {
                        tables[i, j].BackColor = Color.LightBlue;
                        toplam = Convert.ToDouble(dataSet.Tables[0].Rows[0]["sum"]);
                    }
                    else
                        toplam = 0;


                    tables[i, j].Text = "MASA " + Convert.ToString((i * 2 + j) + 1) + "\n\n" + Convert.ToString(toplam) + "  TL";
                    this.Controls.Add(tables[i, j]);
                }

        }

        private void button13_Click(object sender, EventArgs e) // ÜRÜN EKLE - ONAYLA
        {
            if(button13.Text=="Ürün Ekle")
            {
                button13.Text = "Onayla";
                button13.BackColor = Color.GreenYellow;
                flowLayoutPanel1.Visible = true;
                panel1.Visible = true;
            }
            else
            {
                button13.BackColor = DefaultBackColor;
                button13.Text = "Ürün Ekle";
                flowLayoutPanel1.Visible = false;
                if(panel2.Visible==false)
                    panel1.Visible = false;
                hesapGuncelle();
            }
            
        }

        private void button10_Click(object sender, EventArgs e) //HESAP ISTEME
        {

            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button13.Enabled = false;

            clickedBut.BackColor = Color.LightPink;
            DateTime now = DateTime.Now;


            
            string output = "Tarih: " + String.Format("{0:dd/MM/yyyy}", now) + "\n";
            output += "Saat: " + String.Format("{0:HH:mm:ss tt}",now) + "\n";
            output += "Masa No : " + masano + " \n\n";
            output += "ÜRÜN ADI                MİK.       TUTAR\n ──────────         ────     ─────\n";



            System.IO.File.WriteAllText(@"C:\Print.txt",output);
            Yazdır(0);
        }

        private  void OnPrintDocument(object sender, PrintPageEventArgs e)
        {
            //header
            Font font = new Font("Calibri",10);
            Font font1 = new Font("Calibri",10,FontStyle.Bold);
            Font font2 = new Font("Calibri", 12, FontStyle.Bold);

            float yPozisyon = 0; int LineCount = 0;
            float leftMargin = 30;//e.MarginBounds.Left;
            float topMargin = 0; //e.MarginBounds.Top;

            //BAVYERA PUB YAZDIR
            string output = "BAVYERA PUB";
            e.Graphics.DrawString(output, font2, Brushes.Black, leftMargin+50, yPozisyon);
            LineCount=LineCount+2;

            string line = null;

            float SayfaBasinaDusenSatir = e.MarginBounds.Height / font.GetHeight();

            while (((line = dosyaAkimi.ReadLine()) != null) && LineCount < SayfaBasinaDusenSatir)
            {
                yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, font, Brushes.Black, leftMargin, yPozisyon);

                LineCount++;
            }

            if (line == null)
                e.HasMorePages = false;
            else
                e.HasMorePages = true;

            //body
            dataSet = new DataSet();
            sql = "select * from anlik a,menu m where masaId = " + masano + " and m.Id=a.sipId";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            int count = dataSet.Tables[0].Rows.Count;
            output=null;
            //  çizgiyi hallet
            for (int i = 0; i < count; i++)
            {
                yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
                output = dataSet.Tables[0].Rows[i]["isim"].ToString();
                e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);
                output = String.Format("{0,-9}", dataSet.Tables[0].Rows[i]["adet"].ToString());
                e.Graphics.DrawString(output, font, Brushes.Black, leftMargin+110, yPozisyon);
                output = String.Format("{0,-10}", dataSet.Tables[0].Rows[i]["fiyat"].ToString()); 
                e.Graphics.DrawString(output, font, Brushes.Black, leftMargin+155, yPozisyon);
  
                LineCount++;
            }
            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
            output = "───────────────────────────";
            LineCount++;
            e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);
            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
            output = "Toplam";
            e.Graphics.DrawString(output, font1, Brushes.Black, leftMargin, yPozisyon);

            dataSet = new DataSet();
            sql = "select sum(fiyat) from anlik where masaId = " + masano;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            masahesap = Convert.ToDouble(dataSet.Tables[0].Rows[0]["sum"]);

            output = Math.Round(Convert.ToDecimal(masahesap),2) + " TL";
            e.Graphics.DrawString(output, font2, Brushes.Black, leftMargin+155, yPozisyon);
            //footer

            /*
            Image newImage = Image.FromFile("C:\\bavyera.jpeg");
            e.Graphics.DrawImage(newImage,leftMargin+45,yPozisyon+45,140,140);
            */
            dosyaAkimi.Close();

        }

        private void OnPrintGunsonu(object sender, PrintPageEventArgs e)
        {
            //header
            Font font = new Font("Calibri", 10);
            Font font1 = new Font("Calibri", 10, FontStyle.Bold);
            Font font2 = new Font("Calibri", 12, FontStyle.Bold);

            float yPozisyon = 0; int LineCount = 0;
            float leftMargin = 30;//e.MarginBounds.Left;
            float topMargin = 0; //e.MarginBounds.Top;

            string output = "         GÜNSONU RAPORU\n\n";
            e.Graphics.DrawString(output, font2, Brushes.Black, leftMargin,yPozisyon);
            LineCount++;

            string line = null;

            float SayfaBasinaDusenSatir = e.MarginBounds.Height / font.GetHeight();

            while (((line = dosyaAkimi.ReadLine()) != null) && LineCount < SayfaBasinaDusenSatir)
            {
                yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, font, Brushes.Black, leftMargin, yPozisyon);

                LineCount++;
            }

            if (line == null)
                e.HasMorePages = false;
            else
                e.HasMorePages = true;

            //body
            string nakitodeme, kartodeme;

            dataSet = new DataSet();
            sql = " select sum(fiyat) from gunsonu where odeme = 0"; // nakit odeme toplamı
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            nakitodeme = dataSet.Tables[0].Rows[0]["sum"].ToString();

            dataSet = new DataSet();
            sql = " select sum(fiyat) from gunsonu where odeme = 1"; // kart odeme toplamı
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            kartodeme = dataSet.Tables[0].Rows[0]["sum"].ToString();

            if(nakitodeme=="")
                nakitodeme = "0";
            if (kartodeme == "")
                kartodeme = "0";

            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
            output = "Nakit";
            e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);
            output = nakitodeme + " TL";
            e.Graphics.DrawString(output, font1, Brushes.Black, leftMargin + 150, yPozisyon);
            LineCount++;
            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
            output = "Kredi";
            e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);
            output = kartodeme + " TL";
            e.Graphics.DrawString(output, font1, Brushes.Black, leftMargin + 150, yPozisyon);
            LineCount++;
            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));

            output = "\nÜRÜN ADI                MİK.       TUTAR\n ──────────         ────     ─────\n";
            e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);

            LineCount+=3;
            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));

            dataSet = new DataSet();
            sql = "select m.isim,sum(g.adet) as adet_,sum(g.fiyat) as fiyat_ from gunsonu g,menu m where m.Id = g.sipId group by m.isim order by sum(g.fiyat) desc";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            int count1 = dataSet.Tables[0].Rows.Count;

            for(int i = 0; i < count1; i++)
                {
                yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
                output = dataSet.Tables[0].Rows[i]["isim"].ToString();
                e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);
                output = String.Format("{0,-9}", dataSet.Tables[0].Rows[i]["adet_"].ToString());
                e.Graphics.DrawString(output, font, Brushes.Black, leftMargin + 110, yPozisyon);
                output = String.Format("{0,-10}", dataSet.Tables[0].Rows[i]["fiyat_"].ToString());
                e.Graphics.DrawString(output, font, Brushes.Black, leftMargin + 155, yPozisyon);
                LineCount++;
            }

            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
            output = "───────────────────────────";
            LineCount++;
            e.Graphics.DrawString(output, font, Brushes.Black, leftMargin, yPozisyon);
            yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
            output = "Toplam";
            e.Graphics.DrawString(output, font1, Brushes.Black, leftMargin, yPozisyon);

            dataSet = new DataSet();
            sql = "select sum(fiyat) from gunsonu";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            try
            {
                masahesap = Convert.ToDouble(dataSet.Tables[0].Rows[0]["sum"]);
            }
            catch
            {
                masahesap = 0;
            }

            output = Math.Round(Convert.ToDecimal(masahesap), 2) + " TL";
            e.Graphics.DrawString(output, font2, Brushes.Black, leftMargin + 155, yPozisyon);
            

            dosyaAkimi.Close();

            if(!adisyonozeti)
            {
                dataSet = new DataSet();
                sql = "insert into kayıtlı select * from gunsonu"; // günsonu kaydı kayıtlıya aktarılır
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);

                dataSet = new DataSet();
                sql = "delete from gunsonu"; // günsonu kaydı silinir
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);
            }

        }

        private void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Tıklanan=new DataGridViewRow();
                Tıklanan = dataGridView1.Rows[e.RowIndex];

                SipId = Convert.ToInt32(Tıklanan.Cells["Id"].Value.ToString());
 
            }
        }

        private void button7_Click(object sender, EventArgs e) // ARTTIR
        {
            if (SipId == 0)
                return;

            dataSet = new DataSet();
            sql = "update anlik set fiyat=fiyat+fiyat/adet,adet=adet+1 where Id = " + SipId;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            FillData();

        }

        private void button8_Click(object sender, EventArgs e) // AZALT 0 a bolmeye bak
        {
            if (SipId == 0)
                return;
            
            try
            {
                if (Convert.ToInt32(Tıklanan.Cells["adet"].Value.ToString()) == 1)
                    return;
            }
            catch(Exception Ex)
            {
                return;
            }
            
            

            dataSet = new DataSet();
            sql = "update anlik set fiyat=fiyat-fiyat/adet,adet=adet-1 where Id = " + SipId;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            //dataGridView1.DataSource = dataSet.Tables[0];

            FillData();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (SipId == 0)
                return;

            dataSet = new DataSet();
            sql = "delete from anlik where Id = " + SipId;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            //dataGridView1.DataSource = dataSet.Tables[0];

            FillData();
        }

        private void MenuButtonClick(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button b = sender as Button;

                dataSet = new DataSet();
                sql = "select * from menu where isim = '"+b.Text+"'";
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);

                int sip = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Id"]);

                //varsa sadece adet ve fiyat arttır
                dataSet = new DataSet();
                sql = "select * from anlik where sipId = " + sip + " and masaId = " + masano;
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count!=0)
                {
                    dataSet = new DataSet();
                    sql = "update anlik set fiyat=fiyat+fiyat/adet,adet=adet+1 where masaid = "+masano+"and sipId = " + sip;
                    add = new NpgsqlDataAdapter(sql, Form1.connection);
                    add.Fill(dataSet);

                    //dataGridView1.DataSource = dataSet.Tables[0];

                    FillData();
                    hesapGuncelle();

                    return;
                }


                dataSet = new DataSet();
                sql = "insert into anlik(masaId,Adet,Fiyat,sipId) values("+masano+",1,"+b.Tag+","+sip+")";
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);



                FillData();
                SipId = 0;
            }
        }

        private void MSitemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string clickedMenu= e.ClickedItem.Text;

            dataSet = new DataSet();
            sql = "select * from menuler where isim = '" + clickedMenu + "'";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            secilenUstId = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Id"]);

            flowLayoutPanel1.Controls.Clear();
            dataSet = new DataSet();
            sql = "select m.isim,m.fiyat from menuler ml,menu m where ml.isim = '" + clickedMenu +  "' and m.ustmenu=ml.Id";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            int count = dataSet.Tables[0].Rows.Count;

            for(int i=0;i<count;i++)
            {
                Button btn = new Button();
                btn.Text = Convert.ToString(dataSet.Tables[0].Rows[i]["isim"]);
                btn.Tag = Convert.ToDouble(dataSet.Tables[0].Rows[i]["fiyat"]);
                btn.Height = 66;
                btn.Width = 130;
                btn.Font = butonFont;
                btn.Click += MenuButtonClick;
                flowLayoutPanel1.Controls.Add(btn);
            }

            dataSet = new DataSet();
            sql = "select m.Id,m.isim,m.fiyat from menu m,menuler ml where m.ustmenu = ml.Id and ml.isim = '" + clickedMenu + "'";
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            dataGridView2.DataSource = dataSet.Tables[0];


            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e) // MENU DEGISTIR
        {
            if (button1.Text == "Menü Değiştir/Ekle")
            {
                button1.Text = "Kaydet";
                button1.BackColor = Color.GreenYellow;
                panel2.Visible = true;
                panel1.Visible = true;
                /*
                button13.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                button10.Enabled = false;*/
            }
            else
            {
                button1.BackColor = Color.Wheat;
                button1.Text = "Menü Değiştir/Ekle";
                panel2.Visible = false;
                if(flowLayoutPanel1.Visible==false)
                    panel1.Visible = false;
            }
        }

        private void menu_guncelle()
        {
            dataSet = new DataSet();
            sql = "select m.Id,m.isim,m.fiyat from menu m,menuler ml where m.ustmenu = ml.Id and ml.Id = "+secilenUstId;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);
            dataGridView2.DataSource = dataSet.Tables[0];

            secilenMenuId = 0;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void MenüListesiClick(object sender, DataGridViewCellEventArgs e) // MENÜ DEĞİŞTİRDE MENU SEÇME
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow menuitem = new DataGridViewRow();
                menuitem = dataGridView2.Rows[e.RowIndex];

                textBox1.Text = menuitem.Cells["isim"].Value.ToString();
                textBox2.Text = menuitem.Cells["fiyat"].Value.ToString();
                secilenMenuId = Convert.ToInt32(menuitem.Cells["Id"].Value.ToString());
            }
        }

        private void button11_Click(object sender, EventArgs e) // MENÜ EKLE
        {
            string ürün_adı = textBox1.Text;
            string ürün_fiyat = textBox2.Text;
            
            try
            {
                dataSet = new DataSet();
                sql = "insert into menu(isim,ustmenu,fiyat) values('" + ürün_adı + "'," + secilenUstId + "," + ürün_fiyat + ")";
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);
            }
            catch(Exception exx)
            {
                MessageBox.Show(exx.Message);
            }

            textBox1.Text = "";
            textBox2.Text = "";

            menu_guncelle();
        }

        private void button12_Click(object sender, EventArgs e) // MENÜ DEĞİŞTİR
        {
            string ürün_adı = textBox1.Text;
            string ürün_fiyat = textBox2.Text;

            try
            {
                dataSet = new DataSet();
                sql = "update menu set isim = '" + ürün_adı + "',fiyat= " + ürün_fiyat + "    where Id = " + secilenMenuId;
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            textBox1.Text = "";
            textBox2.Text = "";

            secilenMenuId = 0;

            menu_guncelle();
        }

        private void button14_Click(object sender, EventArgs e) // MENÜ KALDIR
        {
            if (secilenMenuId == 0)
                return;

            dataSet = new DataSet();
            sql = "delete from menu where Id =  " + secilenMenuId;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            textBox1.Text = "";
            textBox2.Text = "";

            secilenMenuId = 0;

            menu_guncelle();
        }

        private void button2_Click(object sender, EventArgs e) //GÜNSONU
        {
            if (MessageBox.Show("Gün sonu raporu almak istediğinizden emin misiniz?", "Günsonu Raporu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            adisyonozeti = false;

            DateTime now = DateTime.Now;
            string output = "\nTarih: " + String.Format("{0:dd/MM/yyyy}", now) + "\n";
            output += "Saat: " + String.Format("{0:HH:mm:ss tt}", now) + "\n\n";
            output += "YAPILAN SATIŞLAR             TOPLAM\n" +
                     "────────────────          ──────\n";

            System.IO.File.WriteAllText(@"C:\Print.txt", output);
            Yazdır(1);

            //GÜNSONU İŞLEMLERİ
            // gunsonu tablosundaki hepsini yazdır
            // tabloyu kayıtlıya aktar
            // gunsonu tablosunu boşalt

            //satış toplamı nakit kredi...
            //ürün satış özeti
            //Bira
            //meşrubat çoktan aza fiyat toplamları ayrı ayrı
            //bu kadar


        }

        private void button3_Click(object sender, EventArgs e) // FATURALAR
        {
            Fatura fa = new Fatura();
            fa.Show();
        }

        private void button4_Click(object sender, EventArgs e) // ADİSYON ÖZETİ
        {
            adisyonozeti = true;

            Yazdır(1);
        }

        private void krediToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void CMSitemClicked(object sender, ToolStripItemClickedEventArgs e) // NAKİT - KREDİ SEÇİMİ
        {
            string masa, sip, adet, fiyat, odeme;

            ToolStripItem item = e.ClickedItem;
            if (item.Text == "Nakit")
                odeme = "0";
            else
                odeme = "1";
            dataSet = new DataSet();
            sql = "select * from anlik where masaId = " + masano;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);


            int count = dataSet.Tables[0].Rows.Count;

            for (int i = 0; i < count; i++)
            {
                masa = dataSet.Tables[0].Rows[i]["masaId"].ToString();
                sip = dataSet.Tables[0].Rows[i]["sipId"].ToString();
                adet = dataSet.Tables[0].Rows[i]["Adet"].ToString();
                fiyat = dataSet.Tables[0].Rows[i]["Fiyat"].ToString();

                DataSet dataSet2 = new DataSet();
                sql = "insert into gunsonu(masaId,sipId,Adet,Fiyat,odeme) values(" + masa + "," + sip + "," + adet + "," + fiyat + "," + odeme +")";
                add = new NpgsqlDataAdapter(sql, Form1.connection);
                add.Fill(dataSet2);
            }

            dataSet = new DataSet();
            sql = "delete from anlik where masaId =" + masano;
            add = new NpgsqlDataAdapter(sql, Form1.connection);
            add.Fill(dataSet);

            clickedBut.BackColor = Color.LightGreen;

            hesapGuncelle();
            FillData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Graph graph = new Graph();
            graph.Show();
        }

        private void button6_Click(object sender, EventArgs e) // GİDERLER
        {
            Gider gider = new Gider();
            gider.Show();
        }
    }
}

