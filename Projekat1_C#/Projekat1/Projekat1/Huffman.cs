using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Projekat1
{
    public class Huffman
    {
        public static Node BuildHuffmanTree(string[] characters, List<double> probabilities)
        {
            List<Node> nodes = new List<Node>();//lista cvorova

            for (int i = 0; i < characters.Length; i++)
            {
                Node n = new Node(probabilities[i], characters[i], null, null);//kreiramo objekat tipa Node i dodeljujemo vrednosti: verovatnoca, simbol, levi i desni potomak
                nodes.Add(n);//dodajemo cvor u listu
            }

            while (nodes.Count > 1)//sve dok ne dodjemo do jednog cvora
            {
                nodes = nodes.OrderBy(node => node.Probability).ToList();//sortiramo cvorove po verovatnoci u rastucem poretku

                Node left = nodes[0];//levi potomak
                Node right = nodes[1];//desni potomak

                left.Code = "0";//kod za levog potomka je 0
                right.Code = "1";//kod za desnog potomka je 1

                Node newNode = new Node(left.Probability + right.Probability, left.Symbol + right.Symbol, left, right);//kreiramo novi cvor cija je verovatnoca jednaka zbiru verovatnoca dva cvora, a simbol novog cvora je konkatenacija dva cvora
                nodes.Remove(left);//uklanjamo stare cvorove
                nodes.Remove(right);
                nodes.Add(newNode);//dodajemo novi cvor
            }

            return nodes[0];//vracamo koren stabla
        }

        public static void PrintNodes(Node node, string value)//stampanje cvorova stabla
        {
            string newValue = value + node.Code;// konkatenira trenutni kod čvora sa kodom koji je do sada prikupljen (value)

            if (node.Left != null)//ako postoji levi 
            {
                PrintNodes(node.Left, newValue);//pozivamo rekurzivno ispis dodajuci trenutni kod u newValue
            }
            if (node.Right != null)//ako postoji desni sused 
            {
                PrintNodes(node.Right, newValue);//pozivamo rekurzivno ispis dodajuci trenutni kod u newValue
            }
            if (node.Left == null && node.Right == null)//ako ne postoje ni levi ni desni sused dosli smo do lista
            {
                Console.WriteLine($"Simbol {node.Symbol} => kod: {newValue}");//ispisujemo simbol i kod
            }
        }

        public static string EncodeString(string path, Node root)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);//citamo bajtove iz ulaznog fajla (alfabeta)
                string fileContent = Encoding.UTF8.GetString(data);//enkodiramo podatke iz bajtova u tip string

                StringBuilder encodedString = new StringBuilder();
                foreach (char character in fileContent)//za svaki karakter u sadrzaju fajla
                {
                    encodedString.Append(FindCode(character, root, ""));//pronađi njegov kod i dodaj ga u enkodirani string
                }
                return encodedString.ToString();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

        private static string FindCode(char character, Node node, string value)//pronalazi kod za zadati karakter u Huffman-ovom stablu
        {
            // Ako je čvor null, vraćamo null (karakter nije pronađen u ovom podstablu)
            if (node == null)
            {
                return null;
            }
            // Ako je čvor list (nema leve i desne potomke) i simbol čvora je jednak zadatom karakteru
            if (node.Left == null && node.Right == null && node.Symbol.Equals(character.ToString()))
            {
                // Vraćamo trenutnu vrednost (koja predstavlja Huffman-ov kod) zajedno sa kodom čvora
                return value + node.Code;
            }
            // Pretražujemo levo podstablo, dodajući kod čvora trenutnoj vrednosti
            string leftCode = FindCode(character, node.Left, value + node.Code);
            // Ako je kod pronađen u levom podstablu, vraćamo ga
            if (leftCode != null)
            {
                return leftCode;
            }
            // Ako kod nije pronađen u levom podstablu, pretražujemo desno podstablo, dodajući kod čvora trenutnoj vrednosti
            return FindCode(character, node.Right, value + node.Code);
        }

        public static string SaveNodesToString(Node node, string value)
        {
            string newValue = value + node.Code;// Dodajemo trenutni kod čvora u trenutnu vrednost

            StringBuilder nodeInfo = new StringBuilder();

            // Rekurzivno pozovi funkciju za levi podstablo ako postoji
            if (node.Left != null)
            {
                nodeInfo.Append(SaveNodesToString(node.Left, newValue));
            }
            // Rekurzivno pozovi funkciju za desni podstablo ako postoji
            if (node.Right != null)
            {
                nodeInfo.Append(SaveNodesToString(node.Right, newValue));
            }
            // Ako je čvor list (nema levo ni desno dete), dodaj informaciju o simbolu i njegovom kodu
            if (node.Left == null && node.Right == null)
            {
                nodeInfo.Append($"{node.Symbol}:{newValue} ");
            }
            // Vrati string sa informacijama o čvorovima
            return nodeInfo.ToString();
        }

        public static List<byte> CompressHuffman(string encodedString)
        {
            List<byte> bytes = new List<byte>();
            // Prolazi kroz kodirani string po blokovima od 8 karaktera
            for (int i = 0; i < encodedString.Length; i += 8)
            {
                // Uzimanje sledećih 8 karaktera ili manje ako je preostalo manje od 8
                string byteString = encodedString.Substring(i, Math.Min(8, encodedString.Length - i));
                byte b = Convert.ToByte(byteString, 2);// Konvertuje string binarne reprezentacije u bajt
                bytes.Add(b);
            }
            return bytes;
        }

        public static void SaveToFile(Node root, List<byte> compressedData)
        {
            try
            {
                using (FileStream file = new FileStream(@"..\..\..\huffman_compress.bin", FileMode.Create))
                {
                    string nodeInfo = SaveNodesToString(root, ""); // Dobijanje string reprezentacije Huffmanovih čvorova
                    byte[] nodeInfoBytes = Encoding.UTF8.GetBytes(nodeInfo);// Pretvaranje stringa čvorova u bajtove
                    file.Write(nodeInfoBytes, 0, nodeInfoBytes.Length);
                    file.WriteByte((byte)'\n'); // Upisivanje novog reda
                    foreach (byte b in compressedData)// Pisanje kompresovanih podataka (bajtova) u fajl
                    {
                        file.WriteByte(b);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
