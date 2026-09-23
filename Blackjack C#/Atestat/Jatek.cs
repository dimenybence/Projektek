using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blackjack
{
    public partial class Jatek : Form
    {
        public Jatek()
        {
            InitializeComponent();
        }

        PictureBox[] osztoKartyak = new PictureBox[20];
        PictureBox[] jatekosKartyak = new PictureBox[20];
        Random kartyaErtek = new Random();
        Random kartyaTipus = new Random();
        int generaltKartyaErtek;
        int generaltKartyaTipus;
        string nev;
        int[,] leosztottKartyak = new int[18, 2];
        int osztoKartyakOsszessege = 0;
        int jatekosKartyakOsszessege = 0;
        int jatekosDarabKartya = 11;
        int j = 0;
        int dbOszto = 0;
        int kovKartya = 1;
        private void bKilep_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //kartyak kigenerálása
        private void generaltKartya()
        {
            bool jo = true;
            for (int i = 0; i < 18; i++)
            {
                generaltKartyaErtek = kartyaErtek.Next(1, 14);
                generaltKartyaTipus = kartyaErtek.Next(1, 5);
                jo = true;
                for (int j = 0; j < i; j++)
                {
                    if (leosztottKartyak[j, 0] == generaltKartyaErtek && leosztottKartyak[j, 1] == generaltKartyaTipus)
                    {
                        jo = false;
                    }
                }
                if (jo == true)
                {
                    leosztottKartyak[i, 0] = generaltKartyaErtek;
                    leosztottKartyak[i, 1] = generaltKartyaTipus;
                }
                else
                {
                    i--;
                }
            }
        }

        private void start()
        {
            generaltKartya();
            for (int i = 0; i < 9; i++)
            {
                osztoKartyak[i] = new PictureBox();
                osztoKartyak[i].Width = 140;
                osztoKartyak[i].Height = 240;
                osztoKartyak[i].Top = 0;
                osztoKartyak[i].Left = i * 145;
                osztoKartyak[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                if (i > 0)
                {
                    osztoKartyak[i].Visible = false;
                }

                generaltKartyaErtek = leosztottKartyak[i, 0];
                generaltKartyaTipus = leosztottKartyak[i, 1];
                if (generaltKartyaTipus == 1)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", generaltKartyaErtek, "clubs");
                    osztoKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                if (generaltKartyaTipus == 2)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", generaltKartyaErtek, "diamonds");
                    osztoKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                if (generaltKartyaTipus == 3)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", generaltKartyaErtek, "hearts");
                    osztoKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                if (generaltKartyaTipus == 4)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", generaltKartyaErtek, "spades");
                    osztoKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                panel2.Controls.Add(osztoKartyak[i]);

            }

            for (int i = 9; i < 18; i++)
            {
                jatekosKartyak[i] = new PictureBox();
                jatekosKartyak[i].Width = 140;
                jatekosKartyak[i].Height = 240;
                jatekosKartyak[i].Top = 0;
                jatekosKartyak[i].Left = j * 145;
                jatekosKartyak[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                if (i > 10)
                {
                    jatekosKartyak[i].Visible = false;
                }


                if (leosztottKartyak[i, 1] == 1)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", leosztottKartyak[i, 0], "clubs");
                    jatekosKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                if (leosztottKartyak[i, 1] == 2)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", leosztottKartyak[i, 0], "diamonds");
                    jatekosKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                if (leosztottKartyak[i, 1] == 3)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", leosztottKartyak[i, 0], "hearts");
                    jatekosKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }

                if (leosztottKartyak[i, 1] == 4)
                {
                    nev = string.Format("{0}" + "_of_" + "{1}", leosztottKartyak[i, 0], "spades");
                    jatekosKartyak[i].Image = Image.FromFile("kartyak//" + nev.ToString() + ".png");
                }
                panel1.Controls.Add(jatekosKartyak[i]);
                j++;
            }
            j = 0;
            for (int i = 0; i < 9; i++)
            {
                if (osztoKartyak[i].Visible == true)
                {
                    if (leosztottKartyak[i, 0] == 1)
                    {
                        if (osztoKartyakOsszessege > 10)
                        {
                            osztoKartyakOsszessege += 1;
                            label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                        }
                        else
                        {
                            osztoKartyakOsszessege += 11;
                            label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                        }
                    }
                    else if (leosztottKartyak[i, 0] > 10)
                    {
                        osztoKartyakOsszessege += 10;
                        label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                    }
                    else
                    {
                        osztoKartyakOsszessege += leosztottKartyak[i, 0];
                        label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                    }
                }
            }
            for (int i = 9; i < 18; i++)
            {
                if (jatekosKartyak[i].Visible == true)
                {
                    if (leosztottKartyak[i, 0] == 1)
                    {
                        if (jatekosKartyakOsszessege > 10)
                        {
                            jatekosKartyakOsszessege += 1;
                            label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                        }
                        else
                        {
                            jatekosKartyakOsszessege += 11;
                            label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                        }
                    }
                    else if (leosztottKartyak[i, 0] > 10)
                    {
                        jatekosKartyakOsszessege += 10;
                        label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                    }
                    else
                    {
                        jatekosKartyakOsszessege += leosztottKartyak[i, 0];
                        label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                    }
                }
            }
        }
        
        private void bUjJatek_Click(object sender, EventArgs e)
        {
            bUjJatek.Enabled = false;
            bUjJatek.Text = "Új játék";
            bKerek.Enabled = true;
            for(int i = 0; i < 9; i++)
            {
                osztoKartyak[i].Dispose();
            }
            for (int i = 9; i < 18; i++)
            {
                jatekosKartyak[i].Dispose();
            }
            jatekosKartyakOsszessege = 0;
            osztoKartyakOsszessege = 0;
            label1.Text = "";
            label2.Text = "";
            jatekosDarabKartya = 11;
            dbOszto = 0;
            kovKartya = 1;
            start();
        }

        private void Jatek_Load(object sender, EventArgs e)
        {
            start();
        }

        private void bKerek_Click(object sender, EventArgs e)
        {
            jatekosKartyak[jatekosDarabKartya].Visible = true;
            if(jatekosKartyak[jatekosDarabKartya].Visible == true)
            {
                if(leosztottKartyak[jatekosDarabKartya, 0] == 1)
                {
                    if (jatekosKartyakOsszessege > 10)
                    {
                        jatekosKartyakOsszessege += 1;
                        label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                    }
                    else
                    {
                        jatekosKartyakOsszessege += 11;
                        label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                    }
                }
                else if (leosztottKartyak[jatekosDarabKartya, 0] > 10)
                {
                    jatekosKartyakOsszessege += 10;
                    label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                }
                else
                {
                    jatekosKartyakOsszessege += leosztottKartyak[jatekosDarabKartya, 0];
                    label1.Text = String.Format("{0}", jatekosKartyakOsszessege);
                }
            }
            jatekosDarabKartya++;
            if(jatekosKartyakOsszessege > 21)
            {
                bKerek.Enabled = false;
            }
        }

        private void bMegall_Click(object sender, EventArgs e)
        {
            if(jatekosKartyakOsszessege > 15)
            {
                bKerek.Enabled = false;
                while(osztoKartyakOsszessege < 17)
                {
                    osztoKartyak[kovKartya].Visible = true;
                    if(osztoKartyak[kovKartya].Visible == true)
                    {
                        if (leosztottKartyak[kovKartya, 0] == 1)
                        {
                            if (osztoKartyakOsszessege > 10)
                            {
                                osztoKartyakOsszessege += 1;
                                label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                            }
                            else
                            {
                                osztoKartyakOsszessege += 11;
                                label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                            }
                            dbOszto++;
                            if (kovKartya == 2 && dbOszto == 2)
                            {
                                MessageBox.Show("Vége a játéknak, vesztettél:(");
                                label2.Text = String.Format("{0}", 21);
                            }
                        }
                        else if(leosztottKartyak[kovKartya, 0] > 10)
                        {
                            osztoKartyakOsszessege += 10;
                            label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                        }
                        else
                        {
                            osztoKartyakOsszessege += leosztottKartyak[kovKartya, 0];
                            label2.Text = String.Format("{0}", osztoKartyakOsszessege);
                        }
                        
                    }
                    kovKartya++;
                }
                if (osztoKartyakOsszessege < jatekosKartyakOsszessege && jatekosKartyakOsszessege < 22 || jatekosKartyakOsszessege < 22 && osztoKartyakOsszessege > 21)
                {
                    MessageBox.Show("Nyertél!");
                    bUjJatek.Enabled = true;
                }
                else if (osztoKartyakOsszessege > jatekosKartyakOsszessege && osztoKartyakOsszessege < 22 || osztoKartyakOsszessege < 22 && jatekosKartyakOsszessege > 21)
                {
                    MessageBox.Show("Vesztettél:(");
                    bUjJatek.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Döntetlen");
                    bUjJatek.Enabled = true;
                }
            }
        }
    }
}
