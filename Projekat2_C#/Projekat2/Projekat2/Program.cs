using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using System.Reflection;
using Microsoft.VisualBasic;

class Program
{
    static void Main()
    {
        Console.WriteLine("  ************************************");
        Console.WriteLine("  * Projekat 2 - Zaštita podataka    *");
        Console.WriteLine("  *  Aleksandar Milanović 62/2020    *");
        Console.WriteLine("  ************************************");

        //Potrebno je generisati LDPC (Low Density Parity Check) kod, implementirati algoritam dekodiranja pomocu sindroma
        //kao i Gallager B algoritam:

        //============================
        //1. Konstruisati matricu H LDPC koda sa parametrima n = 15, n - k = 9, wr = 5, wc =3 (odeljak 8.8.1).
        //Za generisanje 2.i 3.grupe redova koristiti standardni generator pseudoslucajnih brojeva sa fiksiranim seed - om,
        //jednakim vasem broju indeksa.
        //============================

        Console.WriteLine("=========================================");
        Console.WriteLine("1. Konstruisanje matrice H LDPC koda...");
        Console.WriteLine("=========================================");

        Random rand = new Random(62);//fiksiramo seed (jednak broju indeksa)

        int n = 15; //broj kolona u matrici H
        int k = 6; //n-k = 9 => k=6
        int wr = 5; //sirina bloka (kolona) u prvom setu redova
        int wc = 3; //broj redova u svakom setu

        int[,] H = new int[n - k, n];  // Inicijalizacija matrice H

        // Popunjavanje prvog seta redova
        // Prvi blok popunjavamo tako sto jedinice stavljamo po glavnoj dijagonali prvog bloka matrice,
        // tako da svaki red ima po wr jedinica
        Console.WriteLine("Popunjavanje prvog seta redova:");
        for (int i = 0; i < wc; i++)
        {
            int startCol = i * wr;
            int endCol = startCol + wr;
            for (int j = startCol; j < endCol; j++)
            {
                H[i, j] = 1;
            }
        }

        stampajMatricu(H);

        // Popunjavanje drugog seta redova
        //drugi blok popunjavamo random postavljanjem jedinica,
        //ali tako da postoji iskljucivo samo jedna jedinica u jednoj koloni tog bloka
        Console.WriteLine("\nPopunjavanje drugog seta redova:");
        for (int j = 0; j < n; j++)
        {
            int rbr = rand.Next(wc, wc + 3);  // Random int u opsegu [wc, wc+2]
            for (int i = wc; i < wc * 2; i++)
            {
                H[i, j] = (rbr == i) ? 1 : 0;
            }
        }

        stampajMatricu(H);

        // Popunjavanje trećeg seta redova
        //treci blok popunjavamo random postavljanjem jedinica,
        //ali tako da postoji iskljucivo samo jedna jedinica u jednoj koloni tog bloka
        Console.WriteLine("\nPopunjavanje trećeg seta redova:");
        for (int j = 0; j < n; j++)
        {
            int rbr = rand.Next(wc * 2, wc * 2 + 3);  // Random int u opsegu [wc*2, wc*2+2]
            for (int i = wc * 2; i < wc * 3; i++)
            {
                H[i, j] = (rbr == i) ? 1 : 0;
            }
        }

        stampajMatricu(H);


        //===========================
        void stampajMatricu(int[,] H)
        {
            // Ispis matrice H
            Console.WriteLine("Matrica H:");
            for (int i = 0; i < H.GetLength(0); i++)
            {
                for (int j = 0; j < H.GetLength(1); j++)
                {
                    Console.Write(H[i, j] + " ");
                }
                Console.WriteLine();
            }
        }



        //==============================
        //2. Na osnovu ovako konstruisane matrice H, generisati tabelu sindroma i korektora i odrediti
        //kodno rastojanje ovog koda.
        //==============================

        //2.1. Generisanje tabele sindroma i korektora

        //Na pocetku je potrebno generisati sve binarne kombinacije za n bitova - to nam predstavlja sve korektore e.
        //Zatim ih sortiramo prema broju jedinica da prvo idu sve kombinacije koje imaju jednu jedinicu, pa dve itd.
        //Tako obezbedimo wH(e) minimalnu tezinu
        //Racunamo sve sindrome pomocu matrice H i vektora korektora e po obrascu: s = H e^T
        //Konacnu tabelu sindroma i korektora formiramo tako sto pronadjemo prvu pojavu sindroma s, i taj korektor e zabelezimo.

        Console.WriteLine("\n================================================");
        Console.WriteLine("2.1. Generisanje tabele sindroma i korektora...");
        Console.WriteLine("================================================");

        // Generisanje svih binarnih kombinacija za n bitova
        var binarneKombinacije = GetBinaryCombinations(n);

        // Sortiranje binarnih kombinacija prema ukupnom broju jedinica
        var binarneKombinacijeSortirane = binarneKombinacije
            .OrderBy(x => x.Count(b => b == 1))
            .ToArray();

        // Konvertovanje binarnih kombinacija u matricu
        int[,] matricaBinarnihKombinacija = new int[binarneKombinacijeSortirane.Length, n];
        for (int i = 0; i < binarneKombinacijeSortirane.Length; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matricaBinarnihKombinacija[i, j] = binarneKombinacijeSortirane[i][j];
            }
        }

        // Izračunavanje rezultata matričnog proizvoda
        var rezultat = MultiplyMatrices(H, TransposeMatrix(matricaBinarnihKombinacija));

        // Primena modulo 2 na rezultat
        var rezultatMod2 = Modulo2(rezultat);

        // Transponovanje rezultata za lepši prikaz
        var finalniRezultati = TransposeMatrix(rezultatMod2);

        // Konverzija matrice u listu nizova
        var listaFinalnihRezultata = new List<int[]>();
        for (int i = 0; i < finalniRezultati.GetLength(0); i++)
        {
            var red = new int[finalniRezultati.GetLength(1)];
            for (int j = 0; j < finalniRezultati.GetLength(1); j++)
            {
                red[j] = finalniRezultati[i, j];
            }
            listaFinalnihRezultata.Add(red);
        }

        // Pronalaženje jedinstvenih elemenata i njihovih indeksa
        var jedinstveniElementi = listaFinalnihRezultata
            .GroupBy(row => string.Join(",", row))
            .Select(g => g.First())
            .ToArray();

        var indeksi = listaFinalnihRezultata
            .Select(row => Array.IndexOf(listaFinalnihRezultata.ToArray(), row))
            .Distinct()
            .ToArray();

        // Izdvajanje binarnih kombinacija na osnovu indeksa
        var izdvojeneKombinacije = indeksi
            .Select(i => binarneKombinacijeSortirane[i])
            .ToArray();

        // Ispis rezultata
        Console.WriteLine("\nKOREKTOR:           SINDROM:");
        for (int i = 0; i < jedinstveniElementi.Length; i++)
        {
            Console.WriteLine($"{string.Join(" ", jedinstveniElementi[i])} : {string.Join(" ", izdvojeneKombinacije[i])}");
        }


        //===========================
        //2.2. Kodno rastojanje
        //Kodno rastojanje d(C) je minimalni broj linearno zavisnih kolona matrice H.
        //Matrice su linearno zavisne ako postoji neka nenula linearna kombinacija tih matrica koja je jednaka nuli.
        //Da bi smo pronasli minimalni broj zavisnih kolona, potrebno je da izracunamo sumu redova za sve moguce kombinacije kolona.
        //Rezultat tj.kodno rastojanje predstavlja minalni broj kolona koje su linerano zavisne tj. rezultat sabiranja redova im je 0.
        //===========================

        Console.WriteLine("\n=========================================");
        Console.WriteLine("2.2. Racunanje kodnog rastojanja...");
        Console.WriteLine("=========================================");

        var noveMatrice = new List<int[,]>();
        var brojeviKolona = new List<int>();
        var broj = new List<int>();

        // Funkcija za računanje zbira redova
        List<int> CalculateRowSums(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var zbirReda = new List<int>();
            for (int i = 0; i < rows; i++)
            {
                int sum = 0;
                for (int j = 0; j < cols; j++)
                {
                    sum += matrix[i, j];
                }
                zbirReda.Add(sum % 2);//po modulu 2
            }
            if (zbirReda.All(v => v == 0))//ako je vektor zbira redova [0,0,0...0] tj. zbirovi su 0 za svaki red
            {
                broj.Add(cols);
            }
            return zbirReda;
        }

        // Za svaki član niza
        foreach (var clan in binarneKombinacijeSortirane)
        {
            var novaMatricaList = new List<int[]>();

            // Izgradnja liste kolona na osnovu članova
            for (int i = 0; i < clan.Length; i++)
            {
                if (clan[i] == 1)
                {
                    var kolona = new int[H.GetLength(0)]; 
                    for (int j = 0; j < H.GetLength(0); j++)
                    {
                        kolona[j] = H[j, i];//Uzimamo kolonu originalne matrice
                    }
                    novaMatricaList.Add(kolona);//Dodajemo tu kolonu u novu matricu
                }
            }

            // Proveri da li je lista prazna pre nego što nastaviš
            if (novaMatricaList.Count == 0)
            {
                //Console.WriteLine("Upozorenje: Lista novaMatricaList je prazna.");
                continue; // Preskoči ovu iteraciju
            }

            var brojRedova = novaMatricaList[0].Length;
            var brojKolona = novaMatricaList.Count;

            // Inicijalizuj novu matricu sa pravim dimenzijama
            var novaMatrica = new int[brojRedova, brojKolona];

            for (int i = 0; i < brojKolona; i++)
            {
                for (int j = 0; j < brojRedova; j++)
                {
                    novaMatrica[j, i] = novaMatricaList[i][j];
                }
            }

            noveMatrice.Add(novaMatrica);
            brojeviKolona.Add(novaMatrica.GetLength(1));
        }


        // Ispisujemo nove matrice, zbir redova, broj kolona i listu brojeva kolona
        for (int i = 0; i < noveMatrice.Count; i++)
        {
            var m = noveMatrice[i];
            var clan = binarneKombinacijeSortirane[i];
            var brojKolona = brojeviKolona[i];

            // Ispis matrica
            //Console.WriteLine($"Matrica {i + 1} (za član niza {string.Join(" ", clan)}):");
            //for (int row = 0; row < m.GetLength(0); row++)
            //{
            //    for (int col = 0; col < m.GetLength(1); col++)
            //    {
            //        //Console.Write(m[row, col] + " ");
            //    }
            //    //Console.WriteLine();
            //}

            //Zbir redova
            var zbirRedova = CalculateRowSums(m);
            //Console.WriteLine($"Zbir redova: {string.Join(" ", zbirRedova)}");
            //Console.WriteLine($"Broj kolona: {brojKolona}");
            //Console.WriteLine("----------");
        }

        var kodnoRastojanje = broj.Min();
        Console.WriteLine($"\nKodno rastojanje: {kodnoRastojanje}\n");


        //===========================
        //3. Implementirati Gallager B algoritam.
        //Odrediti n-torku greske e sa najmanje jedinica tako da Gallager B algoritam
        //sa pragovima odlucivanja th0 = th1 = 0.5 ne uspeva da ispravi sve greske.
        //Uporediti ovaj broj sa kodnim rastojanjem odredjenim u delu 2.
        //===========================


        //Opis algoritma:

        //Pocinje se sa inicijalizacijom vrednosti informacionih bitova x(0) koje se postavljaju na vrednosti primljenog vektora y.

        //Iterativni koraci:

        //Svaki informacioni bit xj salje svoju vrednost svim bitovima ci.
        //Svaki bit ci racuna sumu ω(k)i→j koja predstavlja zbir vrednosti svih bitova osim xj.
        //Informacioni bit xj prima vrednosti ω(k)i→j od ostalih bitova.

        //Na osnovu primljenih vrednosti, informacioni bit xj ažurira svoju vrednost u sledecoj iteraciji koristeci "vecinsko glasanje".
        //Azuriranje se vrsi na osnovu broja pristiglih nula i jedinica:

        //Ako je broj pristiglih nula za informacioni bit xj veći ili jednak od th0 * (n - 1), tada se vrednost xj postavlja na 0.
        //Ako je broj pristiglih jedinica za informacioni bit xj veci ili jednak th1 *(n - 1), tada se vrednost xj postavlja na 1.
        //Ovde(n - 1) predstavlja broj bitova za svaki informacioni bit, a th0 i th1 odredjuju koliki procenat bitova koji mora podrzavati promenu vrednosti da bi doslo do azuriranja.

        //Krajnji rezultat algoritma je dekodirana rec x(k), koja bi trebalo da predstavlja ispravnu kodnu rec.

        Console.WriteLine("===========================================");
        Console.WriteLine("3. Implementacija Gallager B algoritma....");
        Console.WriteLine("===========================================\n");

        Console.WriteLine("Proizvoljni vektor:  1, 0, 0, 0, 0, 0, 0 (primljena kodna rec)");//primer 8.8.3 iz skripte
        var primljeniVektor = new List<int> { 1, 0, 0, 0, 0, 0, 0 };
        var dekodiraniRezultat1 = GallagerBAlgoritam(primljeniVektor);
        Console.WriteLine($"Dekodirani rezultat: {string.Join(", ", dekodiraniRezultat1)} (ispravna kodna rec)");

        Console.WriteLine("\nTestiranje nad redovima matrice:");
        stampajMatricu(H);
        Console.WriteLine("\nIspis dekodiranih rezultata:");
        // Ispis dekodiranih rezultata
        for (int i = 0; i < H.GetLength(0); i++)
        {
            var red = new List<int>();
            for (int j = 0; j < H.GetLength(1); j++)
            {
                red.Add(H[i, j]);
            }
            var dekodiraniRezultat = GallagerBAlgoritam(red);
            Console.WriteLine("Dekodirani rezultat: " + string.Join(", ", dekodiraniRezultat));
        }


        Console.WriteLine("\nKRAJ PROGRAMA!");
    }

    static int[][] GetBinaryCombinations(int n)
    {
        var combinations = Enumerable.Range(0, (int)Math.Pow(2, n))
            .Select(i => Enumerable.Range(0, n)
            .Select(bit => (i >> bit) & 1)
            .ToArray())
            .ToArray();
        return combinations;
    }

    static int[,] TransposeMatrix(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[,] transposed = new int[cols, rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                transposed[j, i] = matrix[i, j];
            }
        }
        return transposed;
    }

    static int[,] MultiplyMatrices(int[,] A, int[,] B)
    {
        int aRows = A.GetLength(0);
        int aCols = A.GetLength(1);
        int bCols = B.GetLength(1);
        int[,] result = new int[aRows, bCols];

        for (int i = 0; i < aRows; i++)
        {
            for (int j = 0; j < bCols; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < aCols; k++)
                {
                    result[i, j] += A[i, k] * B[k, j];
                }
            }
        }
        return result;
    }

    static int[,] Modulo2(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[,] result = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = matrix[i, j] % 2;
            }
        }
        return result;
    }

    static List<int> GallagerBAlgoritam(List<int> y, double th0 = 0.5, double th1 = 0.5, int maxIteracija = 100)
    {
        int n = y.Count;
        var x = new List<int>(y);  // Početna vrednost informacionih bitova

        for (int iteracija = 0; iteracija < maxIteracija; iteracija++)
        {
            // Iterativni koraci
            for (int j = 0; j < n; j++)
            {
                // Korak 1: Svaki xj šalje svoju vrednost ostalim bitovima
                var poslateVrednosti = x.Where((val, index) => index != j).ToList();

                // Korak 2: Računanje sume ωi→j
                int omegaIDoJ = poslateVrednosti.Sum();

                // Korak 3: Slanje ωi→j susedima
                for (int i = 0; i < n; i++)
                {
                    if (i != j)
                    {
                        // Korak 2a: Efikasnije računanje ωi→j
                        int omegaI = x.Sum() - x[j];
                        omegaIDoJ = omegaI;

                        // Korak 3: Slanje ωi→j
                        int poslataVrednost = omegaIDoJ;

                        // Korak 4: Pristizanje vrednosti ωi→j
                        var primljeneVrednosti = Enumerable.Range(0, n)
                            .Where(index => index != j)
                            .Select(_ => poslataVrednost)
                            .ToList();

                        // Korak 5: Ažuriranje vrednosti xj većinskim glasanjem
                        int brojNula = primljeneVrednosti.Count(v => v == 0);
                        int brojJedinca = primljeneVrednosti.Count(v => v == 1);

                        if (brojNula >= th0 * (n - 1))
                        {
                            x[j] = 0;
                        }
                        else if (brojJedinca >= th1 * (n - 1))
                        {
                            x[j] = 1;
                        }
                        else
                        {
                            x[j] = y[j];  // Ako pragovi nisu zadovoljeni, zadržavamo vrednost iz kanala
                        }
                    }
                }
            }

            // Provera uslova zaustavljanja
            if (x.SequenceEqual(y))
            {
                Console.WriteLine($"Algoritam je dostigao stacionarno stanje posle {iteracija + 1} iteracija.");
                break;
            }
        }

        return x;
    }
}



