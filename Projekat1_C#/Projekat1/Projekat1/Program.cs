using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projekat1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("\t\t ************************************");
            Console.WriteLine("\t\t * Projekat 1 - Kompresija podataka *");
            Console.WriteLine("\t\t *  Aleksandar Milanović 62/2020    *");
            Console.WriteLine("\t\t ************************************");

            // Putanja do ulaznog fajla
            string putanja = Upis.GetFilePath("Sample-text-file.txt");
            string fileContent = File.ReadAllText(putanja);
            
            //==============================
            //1.Izracunati bajt-entropiju datog binarnog fajla.
            //Oznacimo sa Ni broj pojavljivanja bajta i = 0, 1, ... , 255 u datom binarnom fajlu, kao i pi = Ni/N, gde je N ukupna duzina fajla u bajtovima.
            //Bajt-entropija je definisana izrazom: H(p) = -p0 log2 p0 - p1 log2 p1 - ... - p255 log2 p255. (Pritom podrazumevamo da je 0 log2 0 = 0).
            //==============================

            Console.WriteLine("===============================================================");
            Console.WriteLine("1. Racunanje bajt-entropije...");
            Console.WriteLine("===============================================================");
            Entropija ent = new Entropija();
            double ukupnaEntropija = ent.RacunanjeBajtEntropije(putanja);//pozivanje funkcije (iz klase Entropija) za racunanje bajt-entropije fajla
            Console.WriteLine($"\nUkupna bajt-entropija ulaznog fajla je: {ukupnaEntropija:F4}");


            //==============================
            //2. Konstruisati Shannon-Fano i Huffmanov kod na osnovu vrednosti p0, p1, ... , p255 i primeniti ih na kodiranje(odnosno kompresovanje) datog binarnog fajla.
            //U kodiranom fajlu, potrebno je najpre zapamtiti sam kod, a zatim i kodirane podatke iz ulaznog fajla.
            //==============================

            //2.1 Konstrukcija Shannon-Fano koda
           
            //Algoritam:

            //Neka je A = {alfa_1, alfa_2, ..., alfa_a} skup simbola alfabeta izvora (skup vrednosti slucajne promenljive X)
            //i neka su p1, p2, ..., pa odgovarajuce verovatnoce pi = p(alfa_i).
            //1. Odredimo i tako da je apsolutna razlika suma p1 + p2 + ... + pi i pi+1 + pi+2 + ... + pa minimalna.
            //2. Slovima alfa_1, alfa_2, ..., alfa_i pridruzimo simbol 0 a slovima alfa_i+1, alfa_i+2, ...,  alfa_a simbol 1.
            //3. Primenimo isti postupak na svaki od skupova {alfa_1, alfa_2, ..., alfa_i} i {alfa_i+1, alfa_i+2, ..., alfa_n}.

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("2.1. Konstrukcija Shannon-Fano koda...");
            Console.WriteLine("===============================================================");

            List<Symbol> simboli = new List<Symbol>();//lista simbola

            for (int i = 0; i < Entropija.karakteriNiz.Count; i++)//uzimamo podatke iz public nizova koje smo definisali u klasi Entropija
            {
                Symbol s = new Symbol(Entropija.karakteriNiz[i], Entropija.verovatnocaSvakogKaraktera[i]);//pravimo objekat tipa Symbol i dodeljujemo vrednosti value i probability
                simboli.Add(s);//dodajemo u listu simbola
            }

            simboli = simboli.OrderByDescending(s => s.Probability).ToList(); // sortiramo niz (simboli ulaznog alfabeta) u opadajućem poretku po verovatnoci (po zahtevu iz teoreme)

            Shannon_Fano.ShannonFanoEncoding(simboli); // dodeljivanje koda svakom simbolu
            Console.WriteLine("Kodiranje:");
            Shannon_Fano.DisplayCodes(simboli);

            // Kompresija Shannon-Fano
            Console.WriteLine("\nEnkodiranje Shannon-Fano...");
            string kodiraniIzlazBit = Shannon_Fano.EncodeString(simboli, putanja);//bitovsko enkodiranje
            Console.WriteLine("Kompresija Shannon-Fano...");
            List<byte> bajtovi = Shannon_Fano.CompressShannonFano(kodiraniIzlazBit);//kompresija
            Console.WriteLine("Upisivanje u kompresovani fajl...");
            Shannon_Fano.WriteInFile(simboli, bajtovi);//upisivanje u kompresovani fajl

            // Dekompresija Shannon-Fano
            Console.WriteLine("Dekompresija Shannon-Fano...");
            string pathShannonFano = Upis.GetFilePath("shannon_fano_compress.bin");
            List<UcitaniSimbol> prviRedKodiranogFajla = Decode.uzmiPrviRedIzFajla(pathShannonFano);//prvi red koji sadrzi kodiranje oblika vrednost:kod

            string ostatakFajla = Decode.uzmiOstatakFajla(pathShannonFano);//ostatak fajla 

            int ukupnaDuzina = kodiraniIzlazBit.Length;
            int poModulu8 = ukupnaDuzina % 8;
            string stringSaIzbacenimEl = poModulu8 != 0
                ? Decode.UkloniCifre(ostatakFajla, 8 - poModulu8)
                : ostatakFajla;


            string decompressedSF = Decode.DecodeString(stringSaIzbacenimEl, prviRedKodiranogFajla);//dekompresovani fajl
            Console.WriteLine("\nShannon-Fano kompresija " + Decode.Compare(decompressedSF));//provera da li je uspesna kompresija

            Console.WriteLine("Prvih 20 simbola originalnog fajla:     " + fileContent.Substring(0, Math.Min(fileContent.Length, 20)));
            Console.WriteLine("Prvih 20 simbola dekompresovanog fajla: " + decompressedSF.Substring(0, Math.Min(decompressedSF.Length, 20)));

            // Stepen kompresije za Shannon-Fano
            StepenKompresije stepen = new StepenKompresije();
            double stepenKompresijeShannonFano = stepen.stepenKompresijeShannonFano();
            Console.WriteLine("Stepen kompresije ShannonFano: " + stepenKompresijeShannonFano);

            Console.WriteLine("Gotov Shannon-Fano");


            //2.2 Konstrukcija Huffman-ovog koda

            //Algoritam:

            //Definisemo strukturu jednog cvora za izgradnju stabla
            //Pravimo stablo
            //Sortiramo listu cvorova po verovatnoci u rastucem poretku
            //Trazimo 2 cvora sa najmanjom verovatnocom
            //Dodeljujemo leve i desne potomke i kodove (levi dobija kod 0, desni kod 1) 
            //Pravimo novi roditeljski cvor cija je vrednost verovatnoce zbir oba cvora sa najmanjom verovatnocom, i simbol je konkatenacija simbola oba cvora
            //Postupak ponavljamo dok ne dodjemo do jednog (krajnjeg) cvora

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("2.2. Konstrukcija Huffman-ovog koda...");
            Console.WriteLine("===============================================================");

            // Priprema niza karaktera
            string[] karakteriNizStr = Entropija.karakteriNiz.Select(c => c.ToString()).ToArray();//koristimo istu listu kao za Shannon-Fano iz klase Entropija, pretvaramo je u niz

            // Kreiranje Huffman-ovog stabla
            Console.WriteLine("Kodiranje:");
            Node koren = Huffman.BuildHuffmanTree(karakteriNizStr, Entropija.verovatnocaSvakogKaraktera);//kreiramo stablo (dobijamo referencu na koren stabla)
            Huffman.PrintNodes(koren, "");//ispisivanje čvorova

            // Kodiranje
            Console.WriteLine("\nEnkodiranje Huffman...");
            string stringHuffman = Huffman.EncodeString(putanja, koren);//enkodiranje
            Console.WriteLine("Kompresija Huffman...");
            List<byte> huffmanBytes = Huffman.CompressHuffman(stringHuffman);//kompresija
            Console.WriteLine("Upisivanje u kompresovani fajl...");
            Huffman.SaveToFile(koren, huffmanBytes);//cuvanje u fajl

            // Dekompresija 
            Console.WriteLine("Dekompresija Huffman...");
            string pathHuffman = Upis.GetFilePath("huffman_compress.bin");
            List<UcitaniSimbol> prviRedKodiranogFileaHuffman = Decode.uzmiPrviRedIzFajla(pathHuffman);

            string stringBezIzbacenihELHuffman = Decode.uzmiOstatakFajla(pathHuffman);

            ukupnaDuzina = stringHuffman.Length;
            poModulu8 = ukupnaDuzina % 8;
            string stringSaIzbacenimElHuffman = poModulu8 != 0
                ? Decode.UkloniCifre(stringBezIzbacenihELHuffman, 8 - poModulu8)
                : stringBezIzbacenihELHuffman;

            string decompressedHuff = Decode.DecodeString(stringSaIzbacenimElHuffman, prviRedKodiranogFileaHuffman);
            Console.WriteLine("\nHuffman-ova kompresija " + Decode.Compare(decompressedHuff));//provera uspesnosti kompresije

            Console.WriteLine("Prvih 20 simbola originalnog fajla:     " + fileContent.Substring(0, Math.Min(fileContent.Length, 20)));
            Console.WriteLine("Prvih 20 simbola dekompresovanog fajla: " + decompressedHuff.Substring(0, Math.Min(decompressedHuff.Length, 20)));

            double stepenKompresijeHuffman = stepen.stepenKompresijeHuffman();
            Console.WriteLine("Stepen kompresije Huffman: " + stepenKompresijeHuffman);

            Console.WriteLine("Gotov Huffman");


            //==============================
            //3. Implementirati algoritme LZ77 i LZW i primeniti ih na kompresiju datog binarnog fajla.
            //Pretpostaviti da je skup simbola ulaznog alfabeta A = {0, 1, ..., 255}.
            //==============================

            //==============================
            // 3.1 LZ77 algoritam

            //Prvo postavljamo window size za algoritam (5000). 
            //Napomena: povecanjem velicine prozora moze se povecati stepen kompresije ali se time povecava vreme kompresije fajla
            //(velicina ulaznog fajla je u mom slucaju 5MB sa 5.242.880 simbola pa je potrebno malo vise vremena za kompresiju)
            //Konstruisemo tuple sa tri atributa. Tuple za ovaj algoritam moze imati dva oblika: (x,y) ili (x,y,z).
            //Ukoliko je (x,y) to znaci da Y simbol nema ponavljanja i da je X = 0.
            //Dok kod (x,y,z) X=1 -> ima ponavljanja, y oznacava pre koliko simbola je krenulo ponavljanje, i z oznacava koliko je ponavljanje dugacko.
            //==============================

            Console.WriteLine("\n===============================================================");
            Console.WriteLine("3.1. Konstrukcija LZ77 (Lempel-Ziv) koda...");
            Console.WriteLine("===============================================================");
            
            string putanjaLZ77 = Upis.GetFilePath("Sample-text-file.txt");

            Console.WriteLine("Kompresija LZ77 (moze malo da potraje)...");
            int windowSize = 5000;
            var compressedOutput = LZ77.LZ77Compression(putanjaLZ77, windowSize);


            Console.WriteLine("Upisivanje u kompresovani fajl...");
            LZ77.WriteLZ77ToBinaryFile(compressedOutput);

            Console.WriteLine("Dekompresija LZ77...");
            string decompressedLZ77 = LZ77.LZ77Decode();
            Console.WriteLine("\nLZ77 kompresija " + Decode.Compare(decompressedLZ77));
            
            Console.WriteLine("Prvih 20 simbola originalnog fajla:     " + fileContent.Substring(0, Math.Min(fileContent.Length, 20)));
            Console.WriteLine("Prvih 20 simbola dekompresovanog fajla: " + decompressedLZ77.Substring(0, Math.Min(decompressedLZ77.Length, 20)));

            // Stepen kompresije za LZ77
            double stepenKompresijeLZ77 = stepen.stepenKompresijeLZ77();
            Console.WriteLine("Stepen kompresije LZ77: " + stepenKompresijeLZ77);

            Console.WriteLine("LZ77 gotov");



            //======================== 
            // 3.2 LZW algoritam

            //Čitamo liniju po liniju ulaznog fajla i vršimo kompresiju.
            //Kompresija se vrsi tako sto koristimo pomocnu strukturu-recnik.
            //Prednost ovog algoritma je ta sto se recnik nigde ne pamti pa ne zauzima dodatni memorijski prostor.
            //Krenuvsi od indeksa 1 pakujemo sve jedinstvene karaktere u recnik,
            //zatim od pocetka ulaznog niza proveravamo da li se trenutni + naredni element(i) nalaze u privremenom recniku.
            //Trazimo najduze ponavljanje i zapisujemo indeks na kom se mestu nalazi to ponavljanje u recniku i to zapisujemo u izlaz.
            //Zatim se u recnik doda podniz koji je bio ponovljen plus jedan karakter.
            //Upisujemo u izlazni fajl i nakon toga vršimo dekompresiju tog fajla.
            //Dekompresija se vrsi tako sto iteracija upisivanja u recnik kasni jedan korak (posto moramo imati pocetni karakter od koga krecemo)
            //======================== 


            Console.WriteLine("\n===============================================================");
            Console.WriteLine("3.1. Konstrukcija LZW (Lempel-Ziv-Welch) koda...");
            Console.WriteLine("===============================================================");

            string filePathLZW = Upis.GetFilePath("Sample-text-file.txt");

            LZW lzw = new LZW();
            Console.WriteLine("Enkodiranje LZW...");
            List<int> encodedLZWOutput = lzw.LzwEncoder(filePathLZW);
            Console.WriteLine("Upisivanje u kompresovani fajl...");
            lzw.SaveToFile(encodedLZWOutput); 

            List<int> encodedLZWFromFile = lzw.ReadFromFile();

            Console.WriteLine("Dekompresija LZW...");
            string decompressedLZW = lzw.LzwDecoder(encodedLZWFromFile);
            Console.WriteLine("\nLZW kompresija " + Decode.Compare(decompressedLZW));

            Console.WriteLine("Prvih 20 simbola originalnog fajla:     " + fileContent.Substring(0, Math.Min(fileContent.Length, 20)));
            Console.WriteLine("Prvih 20 simbola dekompresovanog fajla: " + decompressedLZW.Substring(0, Math.Min(decompressedLZW.Length, 20)));

            double stepenKompresijeLZW = stepen.stepenKompresijeLZW();
            Console.WriteLine("Stepen kompresije LZW: " + stepenKompresijeLZW);

            Console.WriteLine("LZW gotov");

            //========================


            // 4. Računanje stepena kompresije
            Console.WriteLine("\n===============================================================");
            Console.WriteLine("Poredjenje efikasnosti algoritama prema stepenu kompresije:\n");

            // Stepen kompresije za Shannon-Fano
            Console.WriteLine("Stepen kompresije ShannonFano: " + stepenKompresijeShannonFano);

            // Stepen kompresije za Huffman-a
            Console.WriteLine("Stepen kompresije Huffman:     " + stepenKompresijeHuffman);

            // Stepen kompresije za LZ77
            Console.WriteLine("Stepen kompresije LZ77:        " + stepenKompresijeLZ77);

            // Stepen kompresije za LZW
            Console.WriteLine("Stepen kompresije LZW:         " + stepenKompresijeLZW);

            Console.WriteLine("===============================================================\n");

            Console.WriteLine("KRAJ PROGRAMA!");
        }
    }
}
