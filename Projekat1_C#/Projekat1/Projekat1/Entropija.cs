using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class Entropija
    {
        //public liste za pristup iz Main programa i ostalih klasa gde je potrebno
        public static List<char> karakteriNiz = new List<char>(); // jedinstveni karakteri: a b c d e
        public static List<double> verovatnocaSvakogKaraktera = new List<double>(); // verovatnoca za svaki od karaktera u nizu

        public double RacunanjeBajtEntropije(string putanja)
        {
            try
            {
                using (StreamReader reader = new StreamReader(putanja))//reader uzima liniju po liniju iz ulaznog fajla
                {
                    long N = 0; // ukupan broj karakera
                    int charRead;
                    long[] characterCount = new long[256];//niz za broj pojavljivanja svakog karaktera

                    while ((charRead = reader.Read()) != -1)//dok postoje karakteri u reader-u
                    {
                        N++;
                        characterCount[charRead & 0xFF]++;//pomocu & 0xFF (bitovsko AND sa 255) lako povecavamo broj pojavljivanja Unicode karaktera na poziciji 0-255
                    }

                    Console.WriteLine($"Ukupan broj simbola ulaznog fajla: {N}\n");

                    double[] p = new double[256];//pomocni niz za racunanje verovatnoca pi
                    for (int i = 0; i < characterCount.Length; i++)
                    {
                        p[i] = (double)characterCount[i] / N;//verovatnoca po formuli pi=Ni/N
                    }

                    for (int byteValue = 0; byteValue < characterCount.Length; byteValue++)
                    {
                        if (characterCount[byteValue] > 0)
                        {
                            char karakter = (char)byteValue;
                            double p_i = p[byteValue];
                            karakteriNiz.Add(karakter);
                            verovatnocaSvakogKaraktera.Add(p_i);
                            Console.WriteLine("Simbol {0}: Pojavljuje se {1} puta, Verovatnoca: {2:F2}", karakter, characterCount[byteValue], p_i);
                        }
                    }

                    double entropija = 0;
                    for (int i = 0; i < p.Length; i++)//racunanje bajt-entropije po formuli
                    {
                        if (p[i] != 0)
                        {
                            //entropija -= p[i] * (Math.Log(p[i]) / Math.Log(2));//za slucaj da se koristi starija verzija .NET-a od 3.0 ona nema ugradjenu Log2 funkciju pa primenjujemo ovu matematicku transformaciju
                            entropija -= p[i] * Math.Log2(p[i]);
                        }
                    }
                    return entropija;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
            return 0.0;
        }
    }
}
