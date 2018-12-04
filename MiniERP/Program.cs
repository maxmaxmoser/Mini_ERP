using FluentDateTime;
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

                foreach(Cas cas in items.cas)
                {
                    if(cas.id == response)
                    {
                        CheckLivraisons(cas);
                        Console.Read();
                    }
                }
            }
        }

        private static string CheckLivraisons(Cas cas)
        {
            DateTime dateDebut = DateTime.Parse(cas.date_depart);
            List<Projet> projetsCas = new List<Projet>();

            using (StreamReader r = new StreamReader("..\\..\\config\\projets.json"))
            {
                string json = r.ReadToEnd();
                ProjetsList items = JsonConvert.DeserializeObject<ProjetsList>(json);
            
                var nomProjetsCas = new List<string>(cas.projects.Split(','));
                
                foreach (var projet in items.projets)
                {
                    if(nomProjetsCas.Contains(projet.nom))
                    {
                        projetsCas.Add(projet);
                    }
                }

                foreach (Projet proj in projetsCas)
                {
                    DateTime finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / cas.nb_dev).AddDays(-1);
                    DateTime finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / cas.nb_chef_proj).AddDays(-1);

                    if (finPrevueDev > finPrevueMgt)
                    {
                        Console.WriteLine(finPrevueDev);
                        dateDebut = finPrevueDev.AddDays(1);
                    }
                    else
                    {
                        Console.WriteLine(finPrevueMgt);
                        dateDebut = finPrevueMgt.AddDays(1);
                    }

                }
            }
            return "";
        }
    }
}

// Quand deadline égale, on priorise le projet qui prend le moins de temps
