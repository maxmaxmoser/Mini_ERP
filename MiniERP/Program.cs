using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MiniERP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("========= Jeuxsuilpatron MiniERP project ========");
            Console.WriteLine("Quel cas voulez-vous traiter?");
            Console.WriteLine("\t1 - Cas 1 : 100% efficience, 3 projets");
            Console.WriteLine("\t2 - Cas 2 : 80% efficience, 3 projets");
            Console.WriteLine("\t3 - Cas 3 : 120% efficience, 4 projets");

            Console.Write("-->");
            int response = int.Parse(Console.ReadLine());
            using (StreamReader r = new StreamReader("..\\..\\config\\cas.json"))
            {
                string json = r.ReadToEnd();
                CasList items = JsonConvert.DeserializeObject<CasList>(json);
            }
        }
    }
}
