using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class Decode
    {
        public static List<UcitaniSimbol> uzmiPrviRedIzFajla(string putanja)//prvi red je potreban za kodiranje i on je oblika vrednost:kod
        {
            List<UcitaniSimbol> simboli = new List<UcitaniSimbol>();
            try
            {
                using (StreamReader sr = new StreamReader(putanja, Encoding.UTF8))
                {
                    string prviRedBin = sr.ReadLine().Trim();//uklanjamo vodece i zavrsne praznine iz stringa, ako ih ima
                    string[] simboliRaw = prviRedBin.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);//delimo string po prazninama i ukljanjamo prazne unose
                    foreach (string simbolRaw in simboliRaw)
                    {
                        // Console.WriteLine("RAW: " + simbolRaw);
                        string[] parts = simbolRaw.Split(':');//delimo string po : 
                        UcitaniSimbol simbol = new UcitaniSimbol(parts[0], parts[1]);//kreiramo objekat i dodeljujemo odgovarajuce vrednosti za Vrednost i Kod 
                        simboli.Add(simbol);//dodajemo objekat u listu
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            return simboli;//vracamo listu
        }

        public static string uzmiOstatakFajla(string putanja)//uzimamo ostatak fajla bez prvog reda (prvi red je za kodiranje oblika vrednost:kod)
        {
            byte[] data = File.ReadAllBytes(putanja);

            int endIndex = 0;
            while (endIndex < data.Length && data[endIndex] != '\n')//dok ne dodjemo do kraja reda
            {
                endIndex++;
            }

            int startIndex = (endIndex < data.Length) ? endIndex + 1 : 0;// ako ima jos novih redova krecemo od sledece pozicije

            StringBuilder izlazniString = new StringBuilder();
            for (int i = startIndex; i < data.Length; i++)
            {
                izlazniString.Append(Convert.ToString(data[i], 2).PadLeft(8, '0'));//pretvaramo bajtove u string i dopunjavamo nulama sa leve strane do duzine 8 bitova
            }
            return izlazniString.ToString();
        }

        public static string UkloniCifre(string binarniNiz, int brojCifaraZaUklanjanje)
        {
            // Uzimanje poslednjih 8 cifara
            string poslednjih8 = binarniNiz.Substring(binarniNiz.Length - 8);
            poslednjih8 = poslednjih8.Substring(brojCifaraZaUklanjanje, 8 - brojCifaraZaUklanjanje);
            // Uzimanje ostalog dela niza
            string ostatak = binarniNiz.Substring(0, binarniNiz.Length - 8);

            // Formiranje krajnjeg niza
            string krajnjiNiz = ostatak + poslednjih8;

            return krajnjiNiz;
        }

        public static string DecodeString(string niz, List<UcitaniSimbol> simboli)
        {
            StringBuilder dekodiraniNiz = new StringBuilder();
            StringBuilder trenutniKod = new StringBuilder();
            foreach (char bit in niz)
            {
                trenutniKod.Append(bit);
                foreach (UcitaniSimbol simbol in simboli)
                {
                    if (trenutniKod.ToString().Equals(simbol.Kod))//ako je kod isti
                    {
                        dekodiraniNiz.Append(simbol.Vrednost);//dodaj samo vrednost u dekodirani niz
                        trenutniKod.Clear();//ocisti pomocni niz
                        break;
                    }
                }
            }
            return dekodiraniNiz.ToString();
        }

        public static string Compare(string output)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string input = Path.Combine(currentDirectory, @"..\..\..\Sample-text-file.txt");

            try
            {
                using (StreamReader reader = new StreamReader(input, Encoding.UTF8))
                {
                    StringBuilder encodedString = new StringBuilder();

                    string line;
                    while ((line = reader.ReadLine()) != null)//ucitamo sve linije iz ulaznog fajla
                    {
                        encodedString.Append(line);
                    }

                    if (encodedString.ToString().Equals(output))//ako je ulazni fajl jednak output-u (prosledjenom fajlu)
                    {
                        return "je uspesna. Sadrzaj ulaznog i dekompresovanog fajla je isti!";
                    }
                    return "nije uspesna. Sadrzaj ulaznog i dekompresovanog fajla je razlicit!";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "GRESKA";
            }
        }
    }
}
