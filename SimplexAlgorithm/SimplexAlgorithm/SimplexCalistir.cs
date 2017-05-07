using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexAlgorithm
{
    class SimplexCalistir
    {
        SimplexFileReader file = new SimplexFileReader("ornek.mps");
        double[,] matris;   // Dosyadak matris
        double[,] yedekMatris;        //Birinci Aşamadak Yedek Matris
        double[,] ikinciAsamaMatris;   // İkinci Aşamada kullanılacak Matris
        double[,] yedekIkinciAsamaMatris;
        bool durum = false;
        int iterasyon_sayisi=0;
        private void matrisBoyutlariBelirle()
        {
            yedekMatris = new double[matris.GetLength(0), matris.GetLength(1)];
            ikinciAsamaMatris = new double[matris.GetLength(0),matris.GetLength(1)-file.r_index.Count];
            yedekIkinciAsamaMatris = new double[matris.GetLength(0), matris.GetLength(1) - file.r_index.Count];
        }  // Programda kullanılacak Matris Boyutları belirlendi
        public void fileRead()
        {
            matris=file.fileReader();
            matrisYazdir(matris);
            matrisBoyutlariBelirle(); 
            AsamayaGit();
        }  // Matris Dosyadan Okutuldu ve (matris) değişkenine atandı. -->>AsamayaGit();
        private void AsamayaGit()
        {
            if (file.r_index.Count == 0)
            {
                durum = true;
                ikinciAsamayaGit();
            }
            else
            {
                matrisiHazirla();
                matrisiTutarliHaleGetir();
                birinciAsamaSimplex();
                ikinciAsamaHazirlik();
                tutarliHaleGecir();
                ikiciAsamaSimplex();
            }
        }

        // Cost Sabitlari ikinci aşamada kullanılacağı için ikinciAsamaMatrise (-1) ile Çarparak direk olarak atıyorum ve
        // Birinci aşamada Ri'ler hariç hepsini sıfırlıyorum. 
        private void matrisiHazirla()
        {
            int index = 0;
            bool atama = true;
            for (int j = 0; j < matris.GetLength(1); j++)  // Sutunları gezecem
            { 
                for (int k = 0; k < file.r_index.Count; k++){
                    if (j == file.r_index[k])
                    {
                        matris[0, j] = -1;
                        atama = false;
                    }
                }
                if (atama) {
                    ikinciAsamaMatris[0, index] = (-1) * matris[0, j];
                    matris[0, j] = 0;
                    index++;
                }
            }
        } 
        private void matrisiTutarliHaleGetir()
        {
            for (int i = 0; i < file.r_index.Count; i++)  // R lerin Sayısı Kadar Dön
            {
                for (int j = 1; j < matris.GetLength(0); j++)  // Matrisin Satırlarını Gez
                    if (matris[j, file.r_index[i]] == 1)
                        for (int k = 0; k < matris.GetLength(1); k++) //Matrisin Sutunlarını Gez 
                            matris[0, k] += matris[j, k];
                  
            }
            Console.WriteLine("\nMatrisin Tutarlı Hali");
            matrisYazdir(matris);
        }  //Birinci Aşamada Simplex e geçeden önce (Matrisi Tutarlı) hale getirmem gerekecek
        private int girenSutunuBul(string amac,double[,]matris)
        {
            int input = -1;
            if (amac == "MIN")
            {
                double maximum = double.MinValue;
                for (int j = 0; j < matris.GetLength(1) - 1; j++)  // Sutunu Dolas (Cost Hariç)
                {
                    if (maximum < matris[0, j] && Math.Floor(matris[0, j]) > 0)
                    {
                        input = j;
                        maximum = matris[0, j];
                    }
                }
            }
            if (amac=="MAX")
            {
                double minDeger = double.MaxValue;
                for (int i = 0; i < matris.GetLength(1) - 1; i++) //sutunları gezecem
                {
                    if (minDeger > matris[0, i] && matris[0, i] < 0)
                    {
                        input = i;
                        minDeger = matris[0, i];
                    }
                }
            }
          
         
            return input;
        }  // Simplex Aşamasında Giren Sutunu Buluyor
        private int cikanSatiriBul(int girenSutun,double[,]matris)
        {
            int output = -1;
            double minimum = double.MaxValue;
            for (int i = 1; i < matris.GetLength(0); i++)  // Satırları Dolaş (Cost Hariç)
            {
                if (matris[i,girenSutun]!=0)
                {
                    double gecici = matris[i, matris.GetLength(1) - 1] / matris[i, girenSutun];
                    if (minimum > gecici && gecici>0)
                    {
                        output = i;
                        minimum = gecici;
                    }
                }
            }
            return output;
        }  // Simplex Aşamasında Çıkan Satırı Buluyor
        private void birinciAsamaSimplex()
        {
            while (true)
            {
                int girenSutun = girenSutunuBul("MIN",matris);
                if (girenSutun == -1) break;
                int cikanSatir = cikanSatiriBul(girenSutun,matris);
                double pivot = matris[cikanSatir, girenSutun];
                Console.Write($"\nGiren Sutun {girenSutun}   Çıkan Satır  {cikanSatir}  Pivot  {cikanSatir},{girenSutun}");

                for (int j = 0; j < matris.GetLength(1); j++)
                {
                    matris[cikanSatir, j] /= pivot;  // Bütün Satırı pivot a böldüm
                    yedekMatris[cikanSatir, j] = matris[cikanSatir, j];  // Oluşan değeri gecici Matrise ekledim. 
                }

                for (int i = 0; i < matris.GetLength(0); i++) //Satırlar
                {
                    if (cikanSatir != i){
                        for (int j = 0; j < matris.GetLength(1); j++) //Sutunlar
                            yedekMatris[i, j] = matris[i, j] - matris[i, girenSutun] * matris[cikanSatir, j];
                    }
                }

                iterasyon_sayisi++;

                matrisleriDegistir(matris,yedekMatris);
                matrisYazdir(matris);

                
            }
        }  // 1.Aşama simplex bitti . -->>> (İkinciAsamaHazirlik Metoduna Git)
        private void tutarliHaleGecir()
        {
            int j = 0;
            while (Math.Floor(ikinciAsamaMatris[0, j]) != 0 )
            {
                double deger = Math.Abs(ikinciAsamaMatris[0, j]);
                for (int i = 0; i < ikinciAsamaMatris.GetLength(0); i++)  //Satirlar
                {
                    if (ikinciAsamaMatris[i, j] == 1)
                    {
                        for (int k = 0; k < ikinciAsamaMatris.GetLength(1); k++)  //Sutunlar
                        {
                            ikinciAsamaMatris[0, k] += deger * ikinciAsamaMatris[i, k];
                        }
                    }
                }
                j++;
            }
            Console.WriteLine("\n Tutarli : > Matris Hazır");
            matrisYazdir(ikinciAsamaMatris);
        }  //2.Aşamaya Hazır . Matris Tutarlı
        private void ikiciAsamaSimplex()
        {
            while (true)
            {
                int girenSutun = girenSutunuBul(file.amac, ikinciAsamaMatris);
                if (girenSutun == -1) break;
                int cikanSatir = cikanSatiriBul(girenSutun, ikinciAsamaMatris);
                if (cikanSatir == -1) break;
                double pivot = ikinciAsamaMatris[cikanSatir, girenSutun];
                Console.Write($"\nGiren Sutun {girenSutun}   Çıkan Satır  {cikanSatir}  Pivot  {cikanSatir},{girenSutun}");

                for (int j = 0; j < ikinciAsamaMatris.GetLength(1); j++)
                {
                    ikinciAsamaMatris[cikanSatir, j] /= pivot;  // Bütün Satırı pivot a böldüm
                    yedekIkinciAsamaMatris[cikanSatir, j] = ikinciAsamaMatris[cikanSatir, j];  // Oluşan değeri gecici Matrise ekledim. 
                }

                for (int i = 0; i < ikinciAsamaMatris.GetLength(0); i++) //Satırlar
                {
                    if (cikanSatir != i)
                    {
                        for (int j = 0; j < ikinciAsamaMatris.GetLength(1); j++) //Sutunlar
                            yedekIkinciAsamaMatris[i, j] = ikinciAsamaMatris[i, j] - ikinciAsamaMatris[i, girenSutun] * ikinciAsamaMatris[cikanSatir, j];
                    }
                }
                iterasyon_sayisi++;
                matrisleriDegistir(ikinciAsamaMatris, yedekIkinciAsamaMatris);
                matrisYazdir(ikinciAsamaMatris);
            }
            int yeri=0;
            for (int i = 0; i < file.bound_check.Count; i++)
            {
                if (file.bound_check[i] != 0)
                {
                    yeri = i;
                }
            }

            if (yeri != 0)
                ikinciAsamaMatris[0, ikinciAsamaMatris.GetLength(1) - 1] += file.bound_check[yeri] * file.kisitlar[0].degerler[yeri];
            
            Console.Write("\nMatrisin Asıl Değerleri (değişken dönüşümünün geri alınmış)");
            matrisYazdir(ikinciAsamaMatris);
        }
        private void ikinciAsamaHazirlik()
        {
            int index = 0;
            for (int i = 1; i < matris.GetLength(0); i++)  //Satırlar
            {
                bool ekle = true;
                for (int j = 0; j < matris.GetLength(1); j++) //sutunlar
                {
                    for (int k = 0; k < file.r_index.Count; k++)
                        if (j == file.r_index[k])
                            ekle = false;
                    if (ekle)
                    {
                        ikinciAsamaMatris[i, index] = matris[i, j];
                        index++;
                    }
                    else
                        ekle = true;  
                }
                index = 0;
            }
            Console.WriteLine("\nİkinci Aşama Matrisi Oluşturuluyor");
            matrisYazdir(ikinciAsamaMatris);
        }  //2 .Aşama Matrisi oluşturuldu. Şimdi (tutarlı) hale çevirme zamanı -->>tutarliHaleGecir(); 

        private void ikiAsamaliMatriseCevir()
        {
            for (int i = 0; i < ikinciAsamaMatris.GetLength(0); i++)
            {
                for (int j = 0; j < ikinciAsamaMatris.GetLength(1); j++)
                { 
                    if (i == 0)
                        ikinciAsamaMatris[i, j] = (-1) * matris[i, j];
                    else
                        ikinciAsamaMatris[i, j] = matris[i, j];
                }
            }
        }  //file.r_index=0 ise matrisi İkiasamaliMatrise atmak zorunda kaldım
        private void ikinciAsamayaGit()
        {
            if(durum)
                ikiAsamaliMatriseCevir();
            ikiciAsamaSimplex();
        }
        private void matrisleriDegistir(double[,] matris, double[,] yedekMatris)
        {
            for (int i = 0; i < matris.GetLength(0); i++)
            {
                for (int j = 0; j < matris.GetLength(1); j++)
                {
                    matris[i, j] = yedekMatris[i, j];
                }
            }
        } // Matrislerin Yerlerini Değiştiriyorum
        private void matrisYazdir(double[,] gelenMatris)
        {
            Console.WriteLine("\n");
            for (int i = 0; i < gelenMatris.GetLength(0); i++)
            {
                for (int j = 0; j < gelenMatris.GetLength(1); j++)
                {
                    Console.Write("{0:F3}\t",gelenMatris[i,j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("iterasyon Sayisi: "+iterasyon_sayisi);
        }
    }
}
