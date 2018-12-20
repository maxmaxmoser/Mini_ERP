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
            Console.WriteLine("=================================================");
            Console.WriteLine("========= Jeuxsuilpatron MiniERP project ========");
            Console.WriteLine("=================================================");

            using (StreamReader r = new StreamReader("..\\..\\config\\cas.json"))
            {
                string json = r.ReadToEnd();
                CasList items = JsonConvert.DeserializeObject<CasList>(json);

                Console.Write(items.ToString());

                string continuer = "y";
                while(continuer.Equals("y"))
                {
                    Console.WriteLine("\nQuel cas voulez-vous traiter? (id)");
                    Console.Write("-->");
                    int response;
                    while (!int.TryParse(Console.ReadLine(), out response)
                        || response > items.Count()
                        || response < 1)
                    {
                        Console.WriteLine("Veuillez saisir l'identifiant numérique d'un des cas");
                        Console.Write("-->");
                    }

                    foreach (Cas cas in items.Cas)
                    {
                        if (cas.id == response)
                        {
                            CheckLivraisons(cas);
                        }
                    }

                    Console.Write("Voulez-vous continuer?(y/n) ");
                    continuer = Console.ReadLine();
                    while (!continuer.Equals("y") && !continuer.Equals("n"))
                    {
                        Console.Write("Voulez-vous continuer?(y/n) ");
                        continuer = Console.ReadLine();
                    }

                    if(continuer.Equals("n"))
                    {
                        Environment.Exit(0);
                    }
                }                
            }
        }

        private static string CheckLivraisons(Cas cas)
        {
            DateTime dateDebutDev = DateTime.Parse(cas.date_depart);
            DateTime dateDebutMgt = DateTime.Parse(cas.date_depart);

            List<Projet> projetsCas = new List<Projet>();
            float coeffEfficience = getCoeffEfficience(cas);

            // On commence par récupérer les informations des projets du cas traité
            using (StreamReader r = new StreamReader("..\\..\\config\\projets.json"))
            {
                string json = r.ReadToEnd();
                ProjetsList items = JsonConvert.DeserializeObject<ProjetsList>(json);
            
                var nomProjetsCas = new List<string>(cas.projects.Split(','));
                
                foreach (var projet in items.Projets)
                {
                    if(nomProjetsCas.Contains(projet.nom))
                    {
                        projetsCas.Add(projet);
                    }
                }
                
                // Ensuite pour chaque projet on calcule les dates prévues de fin (dev & mgt) et on vérifie si il y a retard de livraison
                bool retard = false;
                bool retardDev = false;
                bool retardMgt = false;
                foreach (Projet proj in projetsCas)
                {
                    // Application du coefficient d'efficience
                    decimal nb_dev_days = (decimal)(proj.nb_dev_days * coeffEfficience);
                    decimal nb_mgt_days = (decimal)(proj.nb_mgt_days * coeffEfficience);

                    if ((nb_dev_days - Math.Truncate(nb_dev_days) > 0))
                    {
                        nb_dev_days = (int)nb_dev_days + 1;
                    }

                    if ((nb_mgt_days - Math.Truncate(nb_mgt_days) > 0))
                    {
                        nb_mgt_days = (int)nb_mgt_days + 1;
                    }

                    proj.nb_dev_days = int.Parse(nb_dev_days.ToString());
                    proj.nb_mgt_days = int.Parse(nb_mgt_days.ToString());

                    // Deadline du projet
                    DateTime deadline = DateTime.Parse(proj.deadline);

                    // Calcul des dates prévues de fin de projet (dev & mgt)
                    DateTime finPrevueDev = dateDebutDev.AddBusinessDays(proj.nb_dev_days / cas.nb_dev).AddDays(-1);
                    DateTime finPrevueMgt = dateDebutMgt.AddBusinessDays(proj.nb_mgt_days / cas.nb_chef_proj).AddDays(-1);

                    Console.Write("Date de fin prévue - projet " + proj.nom + ": ");
                    if (finPrevueDev > finPrevueMgt)
                    {
                        Console.WriteLine(finPrevueDev);                       
                    }
                    else
                    {
                        Console.WriteLine(finPrevueMgt);                    
                    }

                    // Pour le prochain projet, les dates de débuts de dev & mgt
                    dateDebutDev = finPrevueDev;
                    dateDebutMgt = finPrevueMgt;

                    // Check si retard de livraison
                    if (deadline < finPrevueDev || deadline < finPrevueMgt)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("/!\\ Projet " + proj.nom + " : RETARD DE LIVRAISON /!\\ (deadline le " + proj.deadline + ")");
                        Console.BackgroundColor = ConsoleColor.Black;

                        retard = true;

                        if(deadline < finPrevueDev)
                        {
                            retardDev = true;
                        }

                        if(deadline < finPrevueMgt)
                        {
                            retardMgt = true;
                        }
                    }

                    dateDebutDev = dateDebutDev.AddBusinessDays(1);
                    dateDebutMgt = dateDebutMgt.AddBusinessDays(1);
                }

                // On recalcule les dates de fin en ajoutant progressivement des ressources (selon le type de retard)
                // Et on affiche les ressources nécessaires pour une livvraison dans les temps
                if (retard)
                {
                    int nbDevSupp = 0;
                    int nbMgtSupp = 0;

                    // Tant que retard, on recalcule
                    while(retard)
                    {
                        if(retardDev && retardMgt)
                        {
                            nbDevSupp++;
                            nbMgtSupp++;
                        }
                        else if(retardDev)
                        {
                            nbDevSupp++;
                        }
                        else if(retardMgt)
                        {
                            nbMgtSupp++;
                        }

                        // on remet les compteurs à zéro
                        dateDebutDev = DateTime.Parse(cas.date_depart);
                        dateDebutMgt = DateTime.Parse(cas.date_depart);
                        retard = false;
                        retardDev = false;
                        retardMgt = false;
                        foreach (Projet proj in projetsCas)
                        {
                            // Deadline du projet
                            DateTime deadline = DateTime.Parse(proj.deadline);

                            // Calcul des dates prévues de fin de projet
                            DateTime finPrevueDev = dateDebutDev.AddBusinessDays(proj.nb_dev_days / (cas.nb_dev + nbDevSupp)).AddDays(-1);
                            DateTime finPrevueMgt = dateDebutMgt.AddBusinessDays(proj.nb_mgt_days / (cas.nb_chef_proj + nbMgtSupp)).AddDays(-1);

                            // Pour le prochain projet, les dates de débuts de dev & mgt
                            dateDebutDev = finPrevueDev;
                            dateDebutMgt = finPrevueMgt;

                            // Check si retard de livraison
                            if (deadline < finPrevueDev || deadline < finPrevueMgt)
                            {
                                retard = true;

                                if (deadline < finPrevueDev)
                                {
                                    retardDev = true;
                                }

                                if (deadline < finPrevueMgt)
                                {
                                    retardMgt = true;
                                }
                            }
                                                        
                            dateDebutDev.AddDays(1);
                            dateDebutMgt.AddDays(1);
                        }
                    }

                    Console.WriteLine("Cas réalisable avec ressources supplémentaires suivantes: " + nbDevSupp + " développeur(s); " + nbMgtSupp + " chefs de projet(s).");
                }
            }
            return "";
        }

        private static float getCoeffEfficience(Cas cas)
        {
            return ((100-(cas.efficience-100))/100);
        }
    }
}
