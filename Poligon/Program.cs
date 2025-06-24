using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PoligonceProjekat
{
    public class Tačka
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Tačka(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Tačka t = (Tačka)obj;
            return Math.Abs(X - t.X) < 1e-9 && Math.Abs(Y - t.Y) < 1e-9;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
        public static List<Tačka> Umotavanje(List<Tačka> tacke)
        {
            if (tacke.Count < 3) return new List<Tačka>(tacke);
            List<Tačka> omotac = new List<Tačka>();
            Tačka leva = tacke[0];
            foreach (var p in tacke)
            {
                if (p.X < leva.X || (Math.Abs(p.X - leva.X) < 1e-9 && p.Y < leva.Y))
                    leva = p;
            }

            Tačka trenutni = leva;

            do
            {
                omotac.Add(trenutni);
                Tačka next = null;

                foreach (var p in tacke)
                {
                    if (p.Equals(trenutni)) continue;

                    if (next == null)
                    {
                        next = p;
                        continue;
                    }

                    double cross = (next.X - trenutni.X) * (p.Y - trenutni.Y) - (next.Y - trenutni.Y) * (p.X - trenutni.X);

                    if (cross < -1e-9)
                    {
                        next = p;
                    }
                    else if (Math.Abs(cross) < 1e-9)
                    {
                        if (Distanca(trenutni, p) > Distanca(trenutni, next))
                            next = p;
                    }
                }

                trenutni = next;

            } while (!trenutni.Equals(leva));

            return omotac;
        }
        public static bool JelTeProst(List<Tačka> poligon)
        {
            int n = poligon.Count;
            for (int i = 0; i < n; i++)
            {
                Tačka A = poligon[i];
                Tačka B = poligon[(i + 1) % n];

                for (int j = i + 1; j < n; j++)
                {
                    if (j == i || j == (i + 1) % n || (j + 1) % n == i) continue;

                    Tačka C = poligon[j];
                    Tačka D = poligon[(j + 1) % n];

                    if (JelSeSece(A, B, C, D))
                    {
                        Console.WriteLine($"Edges ({i},{(i + 1) % n}) and ({j},{(j + 1) % n}) intersect.");
                        return false;
                    }
                }
            }
            return true;
        }
        public static double Perimetar(List<Tačka> poligon)
        {
            double suma = 0;
            for (int i = 0; i < poligon.Count; i++)
            {
                Tačka a = poligon[i];
                Tačka b = poligon[(i + 1) % poligon.Count];
                suma += Udaljenost(a, b);
            }
            return suma;
        }
        public static double Povrsina(List<Tačka> poligon)
        {
            int n = poligon.Count;
            double suma1 = 0, suma2 = 0;
            for (int i = 0; i < n; i++)
            {
                Tačka trenutni = poligon[i];
                Tačka sledeci = poligon[(i + 1) % n];
                suma1 += trenutni.X * sledeci.Y;
                suma2 += trenutni.Y * sledeci.X;
            }
            return Math.Abs(suma1 - suma2) / 2.0;
        }
        public static bool JelUPoligonu(List<Tačka> poligon, Tačka p)
        {
            int n = poligon.Count;
            int br = 0;

            for (int i = 0; i < n; i++)
            {
                Tačka a = poligon[i];
                Tačka b = poligon[(i + 1) % n];

                if (USegm(a, b, p))
                    return true;

                if (Presek(p, a, b))
                    br++;
            }

            return (br % 2 == 1);
        }
        public static bool USegm(Tačka a, Tačka b, Tačka p)
        {
            double cross = (p.Y - a.Y) * (b.X - a.X) - (p.X - a.X) * (b.Y - a.Y);
            if (Math.Abs(cross) > 1e-9)
                return false;

            if (p.X < Math.Min(a.X, b.X) - 1e-9 || p.X > Math.Max(a.X, b.X) + 1e-9)
                return false;
            if (p.Y < Math.Min(a.Y, b.Y) - 1e-9 || p.Y > Math.Max(a.Y, b.Y) + 1e-9)
                return false;

            return true;
        }
        public static bool Presek(Tačka p, Tačka a, Tačka b)
        {
            if (a.Y > b.Y)
            {
                var temp = a;
                a = b;
                b = temp;
            }
            if (p.Y == a.Y || p.Y == b.Y)
                p = new Tačka(p.X, p.Y + 1e-10);

            if (p.Y < a.Y || p.Y > b.Y) return false;
            if (p.X >= Math.Max(a.X, b.X)) return false;

            if (p.X < Math.Min(a.X, b.X)) return true;

            double PresekX = a.X + (p.Y - a.Y) * (b.X - a.X) / (b.Y - a.Y);
            return p.X < PresekX;
        }
        public static double Distanca(Tačka a, Tačka b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static bool JelSeSece(Tačka p1, Tačka p2, Tačka q1, Tačka q2)
        {
            int o1 = OrijentisiSe(p1, p2, q1);
            int o2 = OrijentisiSe(p1, p2, q2);
            int o3 = OrijentisiSe(q1, q2, p1);
            int o4 = OrijentisiSe(q1, q2, p2);

            return o1 != o2 && o3 != o4;
        }
        public static int OrijentisiSe(Tačka a, Tačka b, Tačka c)
        {
            double val = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            if (Math.Abs(val) < 1e-9) return 0;
            return val > 0 ? 1 : 2;
        }
        public static double Udaljenost(Tačka a, Tačka b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public class TezišteMomentInercije
        {
            public double Area { get; set; }
            public double Cx { get; set; }
            public double Cy { get; set; }
            public double MomentInercije { get; set; }
        }
        public static TezišteMomentInercije IzracunajTezisteIMoment(List<Tačka> poligon)
        {
            int n = poligon.Count;
            double A = 0, Cx = 0, Cy = 0, Iz = 0;
            double rho = 1; // Gustina (pretpostavimo 1 za jednostavnost)

            // 1) Računanje površine (formula pertlanja)
            // delimično sumiranje za koordinate težišta
            for (int i = 0; i < n; i++)
            {
                double xi = poligon[i].X;
                double yi = poligon[i].Y;
                double xi1 = poligon[(i + 1) % n].X; // sledeća tačka (ciklički)
                double yi1 = poligon[(i + 1) % n].Y;

                // Doprinos za površinu (determinanta)
                double faktor = xi * yi1 - xi1 * yi;
                A += faktor;

                // Doprinos za težište (centroid)
                Cx += (xi + xi1) * faktor;
                Cy += (yi + yi1) * faktor;
            }

            // Konačna površina (polovina determinante)
            A = A / 2.0;

            // Konačne koordinate težišta (centroid)
            Cx = Cx / (6.0 * A);
            Cy = Cy / (6.0 * A);

            // Računanje momenta inercije oko ose OZ
            // Formula: Iz = (rho/12) * sum[(xi*yi+1 - xi+1*yi) * ((xi-Cx)^2 + (xi-Cx)*(xi+1-Cx) + (xi+1-Cx)^2 + isto za Y)]
            for (int i = 0; i < n; i++)
            {
                double xi = poligon[i].X;
                double yi = poligon[i].Y;
                double xi1 = poligon[(i + 1) % n].X;
                double yi1 = poligon[(i + 1) % n].Y;
                double faktor = xi * yi1 - xi1 * yi;// Doprinos za determinantu
                // Računanje kvadratnih i mešovitih termina za X i Y u odnosu na težište
                double termX = (xi - Cx) * (xi - Cx) + (xi - Cx) * (xi1 - Cx) + (xi1 - Cx) * (xi1 - Cx);
                double termY = (yi - Cy) * (yi - Cy) + (yi - Cy) * (yi1 - Cy) + (yi1 - Cy) * (yi1 - Cy);
                Iz += faktor * (termX + termY);// Ukupno doprinosi za moment inercije (oko OZ ose)
            }
            Iz = Math.Abs(rho * Iz / 12.0);// Konačni moment inercije (apsolutna vrednost zbog orijentacije)
            return new TezišteMomentInercije // Vraćanje rezultata: površina, koordinate težišta, moment inercije
            {
                Area = Math.Abs(A), // uvek pozitivna površina
                Cx = Cx,
                Cy = Cy,
                MomentInercije = Iz
            };
        }
        }
        static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}


