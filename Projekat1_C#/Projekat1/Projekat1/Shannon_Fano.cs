using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class Shannon_Fano
    {
        public static void ShannonFanoEncoding(List<Symbol> symbols)
        {
            if (symbols.Count == 1)//uslov za zaustavljanje rekurzije (kada nakon sazimanja grupa ostane samo jedan simbol na kraju)
            {
                return;
            }

            double sumOfProbabilities = symbols.Sum(s => s.Probability);//ukupna suma verovatnoca
            double currentSum = 0;//trenutna suma verovatnoca
            List<double> differences = new List<double>();//lista za razlike suma

            for (int i = 0; i < symbols.Count; i++)
            {
                currentSum += symbols[i].Probability;
                double remainder = sumOfProbabilities - currentSum;
                differences.Add(Math.Abs(currentSum - remainder));
            }

            double minDifference = differences.Min();//uzimamo minimalnu mogucu razliku
            int splitIndex = differences.IndexOf(minDifference);//indeks po kome delimo listu simbola na dve grupe

            //Simbole delimo na 2 grupe, ali da te dve grupe budu u sto boljoj ravnotezi gledajuci verovatnocu
            //1) Ako se simbol nalazi u prvoj grupi dobija kod 0.
            //2) Ako se simbol nalazi u drugoj grupi dobija kod 1.

            for (int i = 0; i < symbols.Count; i++)//dodeljujemo odgovarajuce kodove simbolima
            {
                symbols[i].Code += (i <= splitIndex) ? "0" : "1";//ako simbol pripada prvoj grupi (indeks<=splitIndex) dobija kod 0 u suprotnom kod 1
            }

            ShannonFanoEncoding(symbols.GetRange(0, splitIndex + 1));//rekurzivni poziv za prvu grupu 
            ShannonFanoEncoding(symbols.GetRange(splitIndex + 1, symbols.Count - splitIndex - 1));//rekurzivni poziv za drugu grupu
        }

        public static void DisplayCodes(List<Symbol> symbols)//prikaz kodova za simbole
        {
            foreach (Symbol symbol in symbols)
            {
                Console.WriteLine($"Simbol {symbol.Value} => kod: {symbol.Code}");
            }
        }

        public static string EncodeString(List<Symbol> symbols, string putanja)//bitovsko enkodiranje ulaznog alfabeta
        {
            try
            {
                using (StreamReader reader = new StreamReader(putanja, Encoding.UTF8))
                {
                    StringBuilder encodedString = new StringBuilder();

                    string line;
                    while ((line = reader.ReadLine()) != null)//za svaku liniju iz ulaznog alfabeta
                    {
                        foreach (char ch in line)//za svaki karakter 
                        {
                            foreach (Symbol symbol in symbols)//za svaki simbol
                            {
                                if (symbol.Value == ch)//ako nadjemo simbol sa istom vrednoscu kao procitani karakter
                                {
                                    encodedString.Append(symbol.Code);//enkodiramo taj karakter
                                    break;
                                }
                            }
                        }
                    }

                    return encodedString.ToString();//vracamo enkodirani string
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return "greska";
            }
        }

        public static List<byte> CompressShannonFano(string kodiraniString)//kompresija fajla
        {
            List<byte> bajtovi = new List<byte>();
            for (int i = 0; i < kodiraniString.Length; i += 8)//uzimamo grupe po 8 bitova tj 1 bajt
            {
                string byteString = kodiraniString.Substring(i, Math.Min(8, kodiraniString.Length - i));
                byte bajt = Convert.ToByte(byteString, 2);//konvertujemo string u bajt
                bajtovi.Add(bajt);
            }
            return bajtovi;
        }

        public static void WriteInFile(List<Symbol> symbols, List<byte> bajtovi)//upis u kompresovani fajl 
        {
            try
            {
                using (FileStream f = new FileStream(@"..\..\..\shannon_fano_compress.bin", FileMode.Create))//kreiramo fajl za kopresovani kod
                {
                    foreach (Symbol symbol in symbols)
                    {
                        string infoSymbol = $"{symbol.Value}:{symbol.Code}";//kodiranje oblika vrednost:kod
                        byte[] infoBytes = Encoding.UTF8.GetBytes(infoSymbol);//pretvaranje u bajtove
                        f.Write(infoBytes, 0, infoBytes.Length);//upisujemo blok bajtova u fajl stream
                        f.WriteByte((byte)' ');//upisujemo razmak
                    }
                    f.WriteByte((byte)'\n');//upisujemo nov red

                    foreach (byte bajt in bajtovi)//dodajemo bajtove koji su kodirani
                    {
                        f.WriteByte(bajt);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
