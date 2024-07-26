using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class StepenKompresije
    {
        private static readonly string currentDirectory = Directory.GetCurrentDirectory();

        public double stepenKompresijeShannonFano()
        {
            string inputFile = Path.Combine(currentDirectory, @"..\..\..\Sample-text-file.txt");
            string exitFile = Path.Combine(currentDirectory, @"..\..\..\shannon_fano_compress.bin");

            long sizeInputFile = 0;
            long sizeShannonFano = 0;
            try
            {
                sizeInputFile = new FileInfo(inputFile).Length;
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Velicina ulaznog fajla:      {sizeInputFile/1024.0:F2} KB");
                sizeShannonFano = new FileInfo(exitFile).Length;
                Console.WriteLine($"Velicina kompresovanog fajla: {sizeShannonFano/1024.0:F2} KB");
                Console.WriteLine("----------------------------------------");
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return getStepenKompresije(sizeInputFile, sizeShannonFano);
        }

        public double stepenKompresijeHuffman()
        {
            string inputFile = Path.Combine(currentDirectory, @"..\..\..\Sample-text-file.txt");
            string exitFile = Path.Combine(currentDirectory, @"..\..\..\huffman_compress.bin");

            long sizeInputFile = 0;
            long sizeHuffman = 0;
            try
            {
                sizeInputFile = new FileInfo(inputFile).Length;
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Velicina ulaznog fajla:      {sizeInputFile / 1024.0:F2} KB");
                sizeHuffman = new FileInfo(exitFile).Length;
                Console.WriteLine($"Velicina kompresovanog fajla: {sizeHuffman / 1024.0:F2} KB");
                Console.WriteLine("----------------------------------------");
                
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return getStepenKompresije(sizeInputFile, sizeHuffman);
        }

        public double stepenKompresijeLZ77()
        {
            string inputFile = Path.Combine(currentDirectory, @"..\..\..\Sample-text-file.txt");
            string exitFile = Path.Combine(currentDirectory, @"..\..\..\LZ77_compress.bin");

            long sizeInputFile = 0;
            long sizeLZ77 = 0;
            try
            {
                sizeInputFile = new FileInfo(inputFile).Length;
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Velicina ulaznog fajla:       {sizeInputFile / 1024.0:F2} KB");
                sizeLZ77 = new FileInfo(exitFile).Length;
                Console.WriteLine($"Velicina kompresovanog fajla: {sizeLZ77 / 1024.0:F2} KB");
                Console.WriteLine("----------------------------------------");
                
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return getStepenKompresije(sizeInputFile, sizeLZ77);
        }

        public double stepenKompresijeLZW()
        {
            string inputFile = Path.Combine(currentDirectory, @"..\..\..\Sample-text-file.txt");
            string exitFile = Path.Combine(currentDirectory, @"..\..\..\LZW_compress.bin");

            long sizeInputFile = 0;
            long sizeLZW = 0;
            try
            {
                sizeInputFile = new FileInfo(inputFile).Length;
                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Velicina ulaznog fajla:       {sizeInputFile / 1024.0:F2} KB");
                sizeLZW = new FileInfo(exitFile).Length;
                Console.WriteLine($"Velicina kompresovanog fajla: {sizeLZW / 1024.0:F2} KB");
                Console.WriteLine("----------------------------------------");
                
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return getStepenKompresije(sizeInputFile, sizeLZW);
        }

        public double getStepenKompresije(long inputNumber, long outputNumber)
        {
            double compressionRatio = (double)inputNumber / outputNumber;
            compressionRatio = Math.Round(compressionRatio * 10000) / 10000.0;
            return compressionRatio;
        }
    }
}
