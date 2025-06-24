using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PoligonceProjekat
{
    public partial class Form1 : Form
    {
        private List<Tačka> tacke = new List<Tačka>();
        private List<Tačka> omotac = null;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.TextBox txtRezultat;
        private System.Windows.Forms.Button btnDodaj;
        private System.Windows.Forms.Button btnIzracunaj;
        private System.Windows.Forms.Button btnOcisti;
        private System.Windows.Forms.Button btnUcitajPoligon;
        private System.Windows.Forms.Button btnSnimiPoligon;
        private System.Windows.Forms.Button btnProvera;
        private ListBox lstTacke;
        private System.Windows.Forms.Button btnFizika;
        private System.Windows.Forms.Label lblPovrš;
        private System.Windows.Forms.Label lblCentar;
        private System.Windows.Forms.Label lblMomenat;
        public Form1()
        {
            InitializeComponent();

            btnProvera.Click += BtnProvera_Click;
            btnUcitajPoligon.Click += BtnUcitajPoligon_Click;
            btnSnimiPoligon.Click += BtnSnimiPoligon_Click;
            btnDodaj.Click += BtnDodaj_Click;
            btnIzracunaj.Click += BtnIzracunaj_Click;
            btnOcisti.Click += BtnCiscenje_Click;
            btnFizika.Click += BtnFizika_Click;
        }
        private void InitializeComponent()
        {
            this.btnFizika = new System.Windows.Forms.Button() { Location = new Point(20, 260), Text = "Fizikališi" }; // Ovo dugme ce ičunati sve vezano za fiziku
            this.lblPovrš = new System.Windows.Forms.Label() { Location = new Point(240, 260), Size = new Size(300, 20), Text = "Površina: " };
            this.lblCentar = new System.Windows.Forms.Label() { Location = new Point(240, 280), Size = new Size(300, 20), Text = "Težište: " };
            this.lblMomenat = new System.Windows.Forms.Label() { Location = new Point(240, 300), Size = new Size(300, 20), Text = "Moment inercije: " };
            this.btnUcitajPoligon = new System.Windows.Forms.Button() { Location = new Point(260, 60), Text = "Učitaj" };
            this.btnProvera = new System.Windows.Forms.Button() { Location = new Point(380, 20), Text = "Provera tačke" };
            this.btnSnimiPoligon = new System.Windows.Forms.Button() { Location = new Point(380, 60), Text = "Snimi" };
            this.btnDodaj = new System.Windows.Forms.Button() { Location = new Point(260, 20), Text = "Dodaj" };
            this.btnIzracunaj = new System.Windows.Forms.Button() { Location = new Point(20, 60), Text = "Izračunaj" };
            this.btnOcisti = new System.Windows.Forms.Button() { Location = new Point(140, 60), Text = "Očisti" };

            this.lstTacke = new ListBox() { Location = new Point(20, 100), Size = new Size(200, 150) };

            this.txtX = new System.Windows.Forms.TextBox() { Location = new Point(20, 20), Width = 100 };
            this.txtY = new System.Windows.Forms.TextBox() { Location = new Point(140, 20), Width = 100 };
            this.txtRezultat = new System.Windows.Forms.TextBox()
            {
                Location = new Point(240, 100),
                Size = new Size(300, 150),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            this.ClientSize = new Size(560, 270);

            this.Controls.Add(btnProvera);
            this.Controls.Add(txtX);
            this.Controls.Add(txtY);
            this.Controls.Add(btnDodaj);
            this.Controls.Add(btnIzracunaj);
            this.Controls.Add(btnOcisti);
            this.Controls.Add(btnUcitajPoligon);
            this.Controls.Add(btnSnimiPoligon);
            this.Controls.Add(lstTacke);
            this.Controls.Add(txtRezultat);
            this.Controls.Add(btnFizika);
            this.Controls.Add(lblPovrš);
            this.Controls.Add(lblCentar);
            this.Controls.Add(lblMomenat);

            this.Text = "Poligon Projekat";
        }
        private void BtnDodaj_Click(object sender, EventArgs e)
        {
            try
            {
                double x = double.Parse(txtX.Text);
                double y = double.Parse(txtY.Text);
                tacke.Add(new Tačka(x, y));
                lstTacke.Items.Add($"({x}, {y})");
                txtX.Clear();
                txtY.Clear();
                txtX.Focus();

                omotac = null;
                Invalidate();
            }
            catch (Exception)
            {
                MessageBox.Show("Unesi validne brojeve za X i Y.", "Greška");
            }
        }
        private void BtnProvera_Click(object sender, EventArgs e)
        {
            if (omotac == null || omotac.Count < 3)
            {
                MessageBox.Show("Poligon nije izračunat ili nema dovoljno tačaka.", "Greška");
                return;
            }

            try
            {
                double x = double.Parse(txtX.Text);
                double y = double.Parse(txtY.Text);
                Tačka p = new Tačka(x, y);

                bool inside = Tačka.JelUPoligonu(omotac, p);

                txtRezultat.AppendText($"\r\nTačka ({x}, {y}) je {(inside ? "unutar" : "izvan")} poligona.\r\n");
            }
            catch
            {
                MessageBox.Show("Unesite validne koordinate tačke.", "Greška");
            }
        }

        private void BtnIzracunaj_Click(object sender, EventArgs e)
        {
            if (tacke.Count < 3)
            {
                MessageBox.Show("Unesite bar 3 tačke.", "Greška");
                return;
            }
            omotac = Tačka.Umotavanje(tacke);
            bool prost = Tačka.JelTeProst(tacke);
            double obim = Tačka.Perimetar(omotac);
            double povrsina = prost ? Tačka.Povrsina(omotac) : 0;
            txtRezultat.Clear();
            txtRezultat.AppendText("Konveksni omotač (poligon):\r\n");
            for (int i = 0; i < omotac.Count; i++)
                txtRezultat.AppendText($"{i + 1}: ({omotac[i].X}, {omotac[i].Y})\r\n");
            txtRezultat.AppendText($"\r\nJel prost poligon: {prost}\r\n");
            txtRezultat.AppendText($"Obim poligona: {obim}\r\n");
            if (prost)
                txtRezultat.AppendText($"Površina poligona: {povrsina}\r\n");
            else
                txtRezultat.AppendText("Nije moguće izračunati površinu (poligon nije prost).\r\n");
            Invalidate();
        }
        private void BtnCiscenje_Click(object sender, EventArgs e)
        {
            omotac = null;
            lstTacke.Items.Clear();
            txtRezultat.Clear();
            txtX.Clear();
            txtY.Clear();
            tacke.Clear();
            txtX.Focus();
            Invalidate();
        }
        private void BtnFizika_Click(object sender, EventArgs e)
        {
            if (omotac == null || omotac.Count < 3)
            {
                MessageBox.Show("Poligon nije izračunat ili nema dovoljno tačaka za izračun fizike.", "Greška");
                return;
            }

            var rezultat = Tačka.IzracunajTezisteIMoment(omotac);

            lblPovrš.Text = $"Površina: {rezultat.Area:F4}";
            lblCentar.Text = $"Težište: ({rezultat.Cx:F4}, {rezultat.Cy:F4})";
            lblMomenat.Text = $"Moment inercije: {rezultat.MomentInercije:F4}";
        }
        private void BtnUcitajPoligon_Click(object sender, EventArgs e)
        {
            try
            {
                tacke.Clear();
                lstTacke.Items.Clear();
                using (StreamReader sr = new StreamReader("poligon.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(' ');
                        if (parts.Length == 2 &&
                            double.TryParse(parts[0], out double x) &&
                            double.TryParse(parts[1], out double y))
                        {
                            tacke.Add(new Tačka(x, y));
                            lstTacke.Items.Add($"({x}, {y})");
                        }
                    }
                }
                omotac = null;
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju poligona: " + ex.Message, "Greška");
            }
        }
        private void BtnSnimiPoligon_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("poligon.txt"))
                {
                    foreach (Tačka t in tacke)
                        sw.WriteLine($"{t.X} {t.Y}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri snimanju poligona: " + ex.Message, "Greška");
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (tacke.Count == 0) return;
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;
            foreach (var t in tacke)
            {
                if (t.X < minX) minX = t.X;
                if (t.X > maxX) maxX = t.X;
                if (t.Y < minY) minY = t.Y;
                if (t.Y > maxY) maxY = t.Y;
            }
            double width = maxX - minX;
            double height = maxY - minY;
            float maxDrawWidth = 300f;
            float maxDrawHeight = 200f;
            float scaleX = width > 0 ? maxDrawWidth / (float)width : 1f;
            float scaleY = height > 0 ? maxDrawHeight / (float)height : 1f;
            float scale = Math.Min(scaleX, scaleY);
            float offsetX = 600f + (maxDrawWidth - (float)width * scale) / 2f;
            float offsetY = 60f + (maxDrawHeight - (float)height * scale) / 2f;
            foreach (var t in tacke)
            {
                float px = (float)(t.X - minX) * scale + offsetX;
                float py = (float)(t.Y - minY) * scale + offsetY;
                g.FillEllipse(Brushes.Blue, px - 4, py - 4, 8, 8);
            }
            if (tacke.Count > 1)
            {
                PointF[] polyPoints = new PointF[tacke.Count];
                for (int i = 0; i < tacke.Count; i++)
                {
                    polyPoints[i] = new PointF((float)(tacke[i].X - minX) * scale + offsetX, (float)(tacke[i].Y - minY) * scale + offsetY);
                }
                g.DrawPolygon(Pens.Gray, polyPoints);
            }
            if (omotac != null && omotac.Count > 1)
            {
                PointF[] hullPoints = new PointF[omotac.Count];
                for (int i = 0; i < omotac.Count; i++)
                {
                    hullPoints[i] = new PointF((float)(omotac[i].X - minX) * scale + offsetX, (float)(omotac[i].Y - minY) * scale + offsetY);
                }
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    g.DrawPolygon(pen, hullPoints);
                }
                using (Brush brush = new SolidBrush(Color.FromArgb(60, Color.Red)))
                {
                    g.FillPolygon(brush, hullPoints);
                }
            }
        }
    }
}
