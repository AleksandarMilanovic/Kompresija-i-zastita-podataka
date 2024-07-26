using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class LZW
    {
        //public liste za pristup iz maina i drugih klasa gde je to potrebno
        private static List<char> uniqueSymbols = new List<char>();
        private static List<string> uniqueSymbolsFromFile = new List<string>();

        public List<int> LzwEncoder(string input)//enkodiranje
        {
            var output = new List<int>();//lista int-ova za cuvanje izlaznog kodiranog niza
            var dictionary = new Dictionary<string, int>();//recnik sa kljucem tipa string i vrednoscu tipa int za cuvanje sekvenci

            using (var reader = new StreamReader(input, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)//citamo fajl liniju po liniju
                {
                    var inputString = line;

                    foreach (var x in inputString)
                    {
                        if (!uniqueSymbols.Contains(x))//ako ne postoji u listi jedinstvenih simbola
                        {
                            uniqueSymbols.Add(x);//dodajemo u listu jedinstvenih
                        }
                    }

                    foreach (var x in inputString)
                    {
                        var key = x.ToString();
                        if (!dictionary.ContainsKey(key))//ako u recniku ne postoji element sa kljucem key
                        {
                            dictionary[key] = dictionary.Count + 1;//u recniku sa kljucem key povecavamo vrednost za 1 
                        }
                    }

                    var currentSequence = inputString[0].ToString();//inicijalna sekvenca iz input stringa

                    for (int i = 1; i < inputString.Length; i++)//iteracija kroz input string pocinje od drugog karaktera
                    {
                        var x = inputString[i];//uzimamo trenutni karakter iz input stringa
                        var key = currentSequence + x;//formiramo novi kljuc tako sto dodamo trenutni karakter na kraj trenutne sekvence

                        if (dictionary.ContainsKey(key))//ako recnik vec sadrzi sekvencu, azuriramo currentSequence
                        {
                            currentSequence = key;
                        }
                        else
                        {
                            output.Add(dictionary[currentSequence]);//dodavanje koda iz recnika u izlaznu listu
                            dictionary[key] = dictionary.Count + 1;//dodavanje nove sekvence u recnik
                            currentSequence = x.ToString();//resetovanje currentSequence na trenutni karakter
                        }
                    }

                    output.Add(dictionary[currentSequence]);//dodavanje poslednje sekvence u izlaznu listu
                }
            }

            return output;
        }

        public static List<char> GetUniqueSymbols()//vracamo listu jedinstvenih simbola
        {
            return uniqueSymbols;
        }

        public void SaveToFile(List<int> encodedOutput)
        {
            // Spajanje jedinstvenih simbola u string sa zarezima kao separatorima
            var uniqueSymbolsString = string.Join(",", uniqueSymbols);
            // Spajanje kodiranog izlaza u string sa zarezima kao separatorima
            var codes = string.Join(",", encodedOutput);

            try
            {
                using (var writer = new StreamWriter(@"..\..\..\LZW_compress.bin", false, Encoding.UTF8))
                {
                    writer.WriteLine(uniqueSymbolsString);//upisujemo string sa jedinstvenim simbolima u fajl
                    writer.WriteLine(codes);//upisujemo string sa enkodiranim podacima u fajl
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public List<int> ReadFromFile()//citanje koda iz kompresovanog fajla
        {
            List<int> lzwIndeksi = null;//lista indeksa sekvenci iz recnika

            try
            {
                var lines = File.ReadAllLines(@"..\..\..\LZW_compress.bin");

                if (lines.Length >= 2)
                {
                    // Prva linija sadrži jedinstvene simbole, odvojene zarezima
                    uniqueSymbolsFromFile = lines[0].Split(',').ToList();
                    // Druga linija sadrži kodirane indekse, odvojene zarezima, koje konvertujemo u listu celih brojeva
                    lzwIndeksi = lines[1].Split(',').Select(int.Parse).ToList();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return lzwIndeksi;
        }

        public string LzwDecoder(List<int> encodedLZWFromFile)//dekodiranje
        {
            //recnik za dekodiranje, sa kljucem tipa int i vrednoscu tipa string.
            var dictionary = new Dictionary<int, string>();

            //popunjavamo recnik sa pocetnim simbolima iz originalnog niza jedinstvenih simbola.
            for (int i = 0; i < uniqueSymbolsFromFile.Count; i++)
            {
                dictionary[i + 1] = uniqueSymbolsFromFile[i];
            }

            // Postavljamo trenutni indeks na sledeći dostupan indeks u rečniku
            int currentIndex = dictionary.Count + 1;
            // Inicijalizujemo rezultat sa prvim dekodiranim simbolom
            var result = new StringBuilder(dictionary[encodedLZWFromFile[0]]);
            var currentSequence = dictionary[encodedLZWFromFile[0]];

            // Prolazimo kroz preostale indekse u listi
            foreach (var index in encodedLZWFromFile.Skip(1))
            {
                string newSequence;
                if (dictionary.TryGetValue(index, out newSequence))// Proveravamo da li se indeks već nalazi u rečniku
                {
                    newSequence = newSequence;
                }
                else if (index == currentIndex)// Ako indeks nije u rečniku i jednak je trenutnom indeksu
                {
                    // generišemo novu sekvencu tako što dodajemo prvi karakter trenutne sekvence
                    newSequence = currentSequence + currentSequence[0];
                }
                else
                {
                    // Ako je indeks nevažeći, bacamo izuzetak.
                    throw new ArgumentException("Invalid index sequence.");
                }

                result.Append(newSequence);// Dodajemo novu sekvencu u rezultat
                dictionary[currentIndex] = currentSequence + newSequence[0];// Dodajemo novu sekvencu u rečnik
                currentIndex++;

                currentSequence = newSequence;// Postavljamo trenutnu sekvencu na novu sekvencu
            }

            return result.ToString();//vracamo dekodirani string
        }
    }
}
