using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class LZ77
    {
        public static List<LZ77Tuple> LZ77Compression(string putanja, int windowSize)
        {
            var compressedOutput = new List<LZ77Tuple>();//kompresovani podaci (lista tuplova)

            try
            {
                string inputString = File.ReadAllText(putanja, Encoding.UTF8);//ucitavamo podatke i enkodiramo ih u string
                int i = 0;
                while (i < inputString.Length)
                {
                    int maxMatchLength = 0;//duzina najduzeg poklapanja
                    int maxMatchIndex = 0;//indeks najduzeg poklapanja

                    for (int j = 1; j <= Math.Min(windowSize, i); j++)
                    {
                        int matchLength = 0;
                        while (i + matchLength < inputString.Length &&
                               inputString[i + matchLength] == inputString[i - j + matchLength])//sve dok se javlja ponavljanje
                        {
                            matchLength++;
                        }
                        if (matchLength > maxMatchLength)
                        {
                            maxMatchLength = matchLength;//azuriramo duzinu najduzeg poklapanja
                            maxMatchIndex = i - j;//azuriramo indeks najduzeg poklapanja
                        }
                    }

                    if (maxMatchLength > 0)//ako je duzina poklapanja veca od 0
                    {
                        compressedOutput.Add(new LZ77Tuple(1, i - maxMatchIndex, maxMatchLength));//dodajemo tuple sa indeksom i maksimalnom duzinom ponavljanja
                        i += maxMatchLength;//pomeramo indeks za duzinu ponavljanja
                    }
                    else
                    {
                        compressedOutput.Add(new LZ77Tuple(0, inputString[i]));//dodajemo tuple sa karakterom do kojeg se stiglo bez ponavljanja
                        i++;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return compressedOutput;//vracamo listu tuplova
        }

        public static void WriteLZ77ToBinaryFile(List<LZ77Tuple> compressedOutput)//upis u binarni fajl
        {
            using (var fileOutputStream = new FileStream(@"..\..\..\LZ77_compress.bin", FileMode.Create, FileAccess.Write))
            {
                foreach (var element in compressedOutput)//za svaki tuple iz liste
                {
                    if (element.Bit == 0)//ako je bit za ponavljanja 0 samo upisujemo 0 i karakter
                    {
                        fileOutputStream.WriteByte(0x00);//upisujemo bajt sa vrednoscu 0
                        fileOutputStream.Write(Encoding.UTF8.GetBytes(element.Karakter.ToString()));//pretvaramo karakter u bajt i upisujemo
                    }
                    else if (element.Bit == 1)//ako je bit za ponavljanja 1 upisujemo 1, od kog indeksa karaktera je krenulo ponavljanje,duzinu ponavljanja
                    {
                        fileOutputStream.WriteByte(0x01);//upisujemo bajt sa vredoscu 1
                        var moveBytes = BitConverter.GetBytes((short)element.Move);//pretvaramo indeks od koga je krenulo ponavljanje u bajt
                        var lengthBytes = BitConverter.GetBytes((short)element.Length);//pretvaramo duzinu ponavljanja u bajt
                        fileOutputStream.Write(moveBytes, 0, moveBytes.Length);//upisujemo indeks 
                        fileOutputStream.Write(lengthBytes, 0, lengthBytes.Length);//upisujemo duzinu ponavljanja
                    }
                }
            }
        }

        public static string LZ77Decode()
        {
            var kodiraniPodaci = ReadFile();//ucitamo podatke iz fajla
            var dekodiranNiz = new StringBuilder();//izlazni niz karaktera
            var bafer = new StringBuilder();//pomocni buffer

            foreach (var unos in kodiraniPodaci)//za svaki unos 
            {
                if (unos.Bit == 0)//unos bez ponavljanja
                {
                    dekodiranNiz.Append(unos.Karakter);//samo dodamo karakter
                    bafer.Append(unos.Karakter);
                }
                else//unos sa ponavljanjem
                {
                    int indeksPocetka = bafer.Length - unos.Move;//indeks pocetka ponavljanja
                    for (int i = 0; i < unos.Length; i++)
                    {
                        dekodiranNiz.Append(bafer[indeksPocetka + i]);//dodamo karakter iz bafera
                        bafer.Append(bafer[indeksPocetka + i]);
                    }
                }
            }
            return dekodiranNiz.ToString();//vracamo dekodirani niz karaktera
        }

        public static List<LZ77Tuple> ReadFile()
        {
            var exit = new List<LZ77Tuple>();//izlaz je lista tuplova

            try
            {
                using (var fileInputStream = new FileStream(@"..\..\..\LZ77_compress.bin", FileMode.Open, FileAccess.Read))
                {
                    int marker;
                    while ((marker = fileInputStream.ReadByte()) != -1)//dok postoje bajtovi za citanje
                    {
                        if (marker == 0)
                        {
                            int karakterByte = fileInputStream.ReadByte();
                            char karakter = (char)karakterByte;
                            exit.Add(new LZ77Tuple(0, karakter));//upisujemo tuple sa 0 i karakter
                        }
                        else if (marker == 1)
                        {
                            byte[] moveBytes = new byte[2];
                            byte[] lengthBytes = new byte[2];
                            fileInputStream.Read(moveBytes, 0, 2);
                            fileInputStream.Read(lengthBytes, 0, 2);
                            int move = BitConverter.ToInt16(moveBytes, 0);//pretvaramo bajtove u int
                            int length = BitConverter.ToInt16(lengthBytes, 0);
                            exit.Add(new LZ77Tuple(1, move, length));//upisujemo tuple sa 1, indeks od koga je krenulo ponavljanje i duzinu ponavljanja
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return exit;//vracamo listu tuplova
        }
    }
}
