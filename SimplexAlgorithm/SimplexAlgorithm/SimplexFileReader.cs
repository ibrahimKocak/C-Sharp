
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimplexAlgorithm
{
    class SimplexFileReader
    {
        StreamReader reader; //Dosya Okuma
        public List<Kisit> kisitlar = new List<Kisit>();
        List<Kisit> kisitlar2 = new List<Kisit>();
        public List<int> r_index = new List<int>();
        List<string> degisken = new List<string>();
        List<string> y_degisken = new List<string>();
        List<double> sabitler = new List<double>();
        double[,] matris;
        int kisit_sayisi = 0,s_sayisi=0,r_sayisi=0;
        public List<double> bound_check = new List<double>(); 
        public string amac;

        public SimplexFileReader(string path)
        {
            reader = new StreamReader(path);
        }
        public class Kisit
        {
            public string adi = null;
            public List<double> degerler = new List<double>();
        }

        public double[,] fileReader()
        {
            string satir;
            while ((satir = reader.ReadLine()) != null)
            {
                if (satir[0] != '*')
                {
                    //Console.WriteLine(satir);  // Yıldızlı Satırları Okuma
                    if (satir.Equals("OBJSENSE")) ReadObjsense(satir);  // Fonksiyon Max mı Min mi ?
                    if (satir == "ROWS") satir = ReadRows(satir);
                    if (satir == "COLUMNS") satir = ReadColumns(satir);
                    if (satir == "RHS") satir = ReadRhs(satir);
                    if (satir == "BOUNDS") ReadBounds(satir);
                }
            }
            Console.WriteLine("Ana Matris:");
            Matris_Olustur(kisitlar, kisitlar2);
            Yazdir(degisken,matris);

            reader.Close();

            return matris;
        }

        private string ReadBounds(string satir)
        {
            satir = reader.ReadLine();
            Kisit bounds = new Kisit();
            string[] kelime;
            int i;

            for (i = 0; i < degisken.Count; i++)
            {
                bound_check.Add(0);
            }

            while (satir != "ENDATA" && satir != null && satir != "")
            {
                kelime = satir.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                if (kelime[0] == "FR")
                {
                    degisken.Add(kelime[2] + "*");

                    foreach (var item in kisitlar)
                        item.degerler.Add(item.degerler[degisken.IndexOf(kelime[2])] * (-1));
                }
                else if (kelime[0] == "LO")
                {
                    if(double.Parse(kelime[3].Replace('.', ',')) > 0)
                    {
                        Kisit kisit = new Kisit(), kisit2 = new Kisit();
                        kisit.adi = "BND";
                        kisit2.adi = "BND";

                        for (i = 0; i < degisken.Count; i++)
                        {
                            kisit.degerler.Add(0);
                            kisit.degerler.Add(0);
                        }

                        kisit.degerler[degisken.IndexOf(kelime[2])] = 1;

                        for (i = 0; i < y_degisken.Count; i++)
                        {
                            kisit2.degerler.Add(0);
                            kisit2.degerler.Add(0);
                        }

                        kisit2.degerler.Add(-1);
                        kisit2.degerler.Add(1);

                        sabitler.Add((double.Parse(kelime[3].Replace('.', ','))));

                        kisitlar.Add(kisit);
                        kisitlar2.Add(kisit2);
                        kisit_sayisi++;
                        s_sayisi++;
                        y_degisken.Add("S" + s_sayisi);
                        r_sayisi++;
                        y_degisken.Add("R" + r_sayisi);
                    }
                    else
                        bound_check[degisken.IndexOf(kelime[2])] = double.Parse(kelime[3].Replace('.', ','));

                }
                else if (kelime[0] == "UP")
                {
                    Kisit kisit = new Kisit(), kisit2 = new Kisit();
                    kisit.adi = "BND";
                    kisit2.adi = "BND";

                    for (i = 0; i < degisken.Count; i++)
                        kisit.degerler.Add(0);

                    kisit.degerler[degisken.IndexOf(kelime[2])] = 1;

                    for (i = 0; i < y_degisken.Count; i++)
                        kisit2.degerler.Add(0);
                    
                    for (i = 0; i < kisit_sayisi; i++)
                        kisitlar2[i].degerler.Add(0);

                    kisit2.degerler.Add(1);

                    sabitler.Add((double.Parse(kelime[3].Replace('.', ','))));

                    kisitlar.Add(kisit);
                    kisitlar2.Add(kisit2);
                    kisit_sayisi++;

                    s_sayisi++;
                    y_degisken.Add("S" + s_sayisi);
                }
                
                satir = reader.ReadLine();
            }

            for (int k = 0; k < bound_check.Count; k++)
            {
                if (bound_check[k] != 0)
                {
                    for (int j = 1; j < kisitlar.Count; j++)
                    {
                        if (kisitlar[j].degerler[k] != 0)
                            sabitler[j] += kisitlar[j].degerler[k]*bound_check[k] * (-1);
                    }
                }
            }

            return satir;
        }

        private string ReadRhs(string satir)
        {
            satir = reader.ReadLine();
            string[] kelime;

            for (int i = 0; i < kisit_sayisi; i++)
                sabitler.Add(0);

            while (satir != "BOUNDS")
            {
                kelime = satir.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                for (int i = 1; i <= kelime.Length - 1; i = i + 2)
                    foreach (var item in kisitlar)
                        if (kelime[i] == item.adi)
                            sabitler[kisitlar.IndexOf(item)] = double.Parse(kelime[i + 1].Replace('.', ','));

                satir = reader.ReadLine();
            }
            return satir;
        }

        private string ReadColumns(string satir)
        {
            satir = reader.ReadLine();
            string[] kelime;
            int[] bilgi = new int[2];
            int i;

            while (satir != "RHS")
            {
                i = 0;
                kelime = satir.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                bilgi = DegiskenSay(degisken, kelime[0]);


                if (bilgi[0] == 1)
                    foreach (var item in kisitlar)
                        item.degerler.Add(0);

                for (i = 1; i <= kelime.Length - 1; i = i + 2)
                    foreach (var item in kisitlar)
                        if (kelime[i] == item.adi)
                            item.degerler[bilgi[1]] = double.Parse(kelime[i + 1].Replace('.', ','));

                satir = reader.ReadLine();
            }
            return satir;
        }

        private void Matris_Olustur(List<Kisit> degiskenler, List<Kisit> y_degiskenler)
        {
            int sutun_sayisi = degisken.Count + y_degisken.Count + 1, i, j;
            matris = new double[kisit_sayisi, sutun_sayisi];

            for (i = 0; i < kisit_sayisi; i++)
                for (j = 0; j < degisken.Count; j++)
                    matris[i, j] = kisitlar[i].degerler[j];

            for (i = 0; i < kisit_sayisi; i++)
                for (j = degisken.Count; j < sutun_sayisi - 1; j++)
                    matris[i, j] = kisitlar2[i].degerler[j - degisken.Count];

            for (i = 0; i < kisit_sayisi; i++)
                matris[i, sutun_sayisi - 1] = sabitler[i];

            for (i = 0; i < r_index.Count; i++)
                matris[0, degisken.Count + r_index[i]] = -1;

            for (i = 0; i < r_index.Count; i++)
                r_index[i] += degisken.Count;

            for (i = 0; i < y_degisken.Count; i++)
                degisken.Add(y_degisken[i]);
            degisken.Add("STS");
        }

        private int[] DegiskenSay(List<string> degisken, string kelime)
        {
            int[] bilgi = new int[2];
            bilgi[0] = 1;

            foreach (string item in degisken)
            {
                if (item == kelime)
                {
                    bilgi[0] = 0;
                    bilgi[1] = degisken.IndexOf(item);
                }
            }

            if (bilgi[0] == 1)
            {
                bilgi[1] = degisken.Count;
                degisken.Add(kelime);
            }

            return bilgi;
        }

        private string ReadRows(string satir)
        {
            satir = reader.ReadLine();

            Kisit kisit = new Kisit(), kisit2 = new Kisit();
            List<string> amaclar = new List<string>();
            string[] kelime;
            kisitlar.Add(kisit);
            kisitlar2.Add(kisit2);

            while (satir != "COLUMNS")
            {
                kisit = new Kisit();
                kisit2 = new Kisit();

                kelime = satir.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                kisit.adi = kelime[1];
                kisit2.adi = kelime[1];

                if (kelime[0] == "N")
                {
                    kisitlar[0] = kisit;
                    kisitlar2[0] = kisit2;
                }
                else
                {
                    kisitlar.Add(kisit);
                    kisitlar2.Add(kisit2);
                    amaclar.Add(kelime[0]);
                }
                kisit_sayisi++;

                satir = reader.ReadLine();
            }

            //S ve R yapay degiskenlerinin belirlendigi yer
            for (int i = 0; i < amaclar.Count; i++)
            {
                foreach (var item2 in kisitlar2)
                {
                    if (amaclar[i] == "L")
                    {
                        if (i == kisitlar2.IndexOf(item2) - 1)
                            item2.degerler.Add(1);
                        else
                            item2.degerler.Add(0);
                    }
                    else if (amaclar[i] == "G")
                    {
                        if (i == kisitlar2.IndexOf(item2) - 1)
                        {
                            item2.degerler.Add(-1);
                            item2.degerler.Add(1);
                            r_index.Add(item2.degerler.Count - 1);
                        }
                        else
                        {
                            item2.degerler.Add(0);
                            item2.degerler.Add(0);
                        }
                    }
                    else if (amaclar[i] == "E")
                    {
                        if (i == kisitlar2.IndexOf(item2) - 1)
                        {
                            item2.degerler.Add(1);
                            r_index.Add(item2.degerler.Count - 1);
                        }
                        else
                            item2.degerler.Add(0);
                    }
                }

                if (amaclar[i] == "L")
                {
                    s_sayisi++;
                    y_degisken.Add("S"+s_sayisi);
                }
                else if (amaclar[i] == "G")
                {
                    s_sayisi++;
                    y_degisken.Add("S" + s_sayisi);
                    r_sayisi++;
                    y_degisken.Add("R" + r_sayisi);
                }
                else if (amaclar[i] == "E")
                {
                    r_sayisi++;
                    y_degisken.Add("R" + r_sayisi);
                }
            }

            return satir;
        }

        private void ReadObjsense(string satir)
        {
            satir = reader.ReadLine();
            if (satir.Trim() == "MIN") amac = "MIN";
            if (satir.Trim() == "MAX") amac = "MAX";
        }

        public void Yazdir(List<string> degisken, double[,] matris)
        {
            Console.WriteLine();
            Console.Write("\t");
            for(int i = 0; i < degisken.Count; i++)
                Console.Write(degisken[i] + "\t");
            Console.WriteLine();

            for (int i = 0; i < kisit_sayisi; i++)
            {
                Console.Write(kisitlar[i].adi+"\t");
                for (int j = 0; j < degisken.Count; j++)
                {
                    Console.Write("{0:F3}\t", matris[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
