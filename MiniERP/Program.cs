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
                    Console.WriteLine("\nQuel cas voulez-vous traiter?");
                    Console.Write("-->");
                    int response;
                    while (!int.TryParse(Console.ReadLine(), out response)
                        || response > items.Count()
                        || response < 1)
                    {
                        Console.WriteLine("Veuillez saisir l'identifiant numérique d'un des cas");
                        Console.Write("-->");
                    }

                    Console.WriteLine("\nSouhaitez-vous prendre en compte les délais d'embauche et de préparation des nouveaux employés ?  (Question E)");
                    Console.WriteLine("true / false");
                    Console.Write("-->");

                    bool delais_embauche_actif;

                    while (!bool.TryParse(Console.ReadLine(), out delais_embauche_actif))
                    {
                        Console.WriteLine("Veuillez saisir 'true' ou 'false'");
                        Console.Write("-->");
                    }
                    
                    
                
                    foreach (Cas cas in items.Cas)
                    {
                        if (cas.id == response)
                        {
                            CheckLivraisons(cas, (bool)delais_embauche_actif);
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

        private static string CheckLivraisons(Cas cas, bool delais_embauche_actif)
        {
            DateTime dateDebut = DateTime.Parse(cas.date_depart);
            List<Projet> projetsCas = new List<Projet>();
            float coeffEfficience = getCoeffEfficience(cas);

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
                
                bool retard = false;
                bool retardDev = false;
                bool retardMgt = false;
                foreach (Projet proj in projetsCas)
                {
                    // Application du coefficient d'efficience
                    decimal nb_dev_days = (decimal)(proj.nb_dev_days * coeffEfficience);
                    decimal nb_mgt_days = (decimal)(proj.nb_mgt_days * coeffEfficience);

                    if((nb_dev_days - Math.Truncate(nb_dev_days) > 0))
                    {
                        nb_dev_days = (int)nb_dev_days + 1 ;
                    }

                    if ((nb_mgt_days - Math.Truncate(nb_mgt_days) > 0))
                    {
                        nb_mgt_days = (int)nb_mgt_days + 1;
                    }
                    
                    proj.nb_dev_days = int.Parse(nb_dev_days.ToString());
                    proj.nb_mgt_days = int.Parse(nb_mgt_days.ToString());


                    // Deadline du projet
                    DateTime deadline = DateTime.Parse(proj.deadline);

                    // Calcul des dates prévues de fin de projet
                    DateTime finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / cas.nb_dev).AddDays(-1);
                    DateTime finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / cas.nb_chef_proj).AddDays(-1);

                    Console.Write("Date de fin prévue - projet " + proj.nom + ": ");
                    if (finPrevueDev > finPrevueMgt)
                    {
                        Console.WriteLine(finPrevueDev);
                        dateDebut = finPrevueDev;
                    }
                    else
                    {
                        Console.WriteLine(finPrevueMgt);
                        dateDebut = finPrevueMgt;
                    }

                    // Check si retard de livraison
                    if(deadline < finPrevueDev || deadline < finPrevueMgt)
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

                    dateDebut = dateDebut.AddBusinessDays(1);
                }

                if (retard)
                {
                    int nb_jours_avant_activite = (cas.duree_avt_efficacite + cas.duree_embauche) * 4 * 5;  //80
                    int nbDevSupp = 0;
                    int nbMgtSupp = 0;

                    if(delais_embauche_actif)
                    {
                        
                        Console.WriteLine("Révision des RH avec prise en compte des délais d'embauche");
                    }
                    // Tant que retard, on recalcule
                    while (retard)
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
                        dateDebut = DateTime.Parse(cas.date_depart);
                        retard = false;
                        retardDev = false;
                        retardMgt = false;

                        int nb_jours_tot_dev = 0;
                        int nb_jours_prevu_dev;
                        int nb_jours_tot_mgt = 0;
                        int nb_jours_prevu_mgt;
                        int nb_jours_avec_aide = 0;
                        int nb_jours_sans_aide = 0;
                        DateTime finPrevueDev;
                        DateTime finPrevueMgt;

                        foreach (Projet proj in projetsCas)
                        {
                            // Deadline du projet
                            DateTime deadline = DateTime.Parse(proj.deadline);

                            // cas avec respect des délais d'embauche + efficience
                            if(delais_embauche_actif)
                            {
                                nb_jours_prevu_dev = nb_jours_tot_dev + proj.nb_dev_days;  // 60 + 30
                                nb_jours_prevu_mgt = nb_jours_tot_mgt + proj.nb_mgt_days;

                                // =< 80; on incrémente la date sans compter les nouveaux
                                if (nb_jours_prevu_dev <= nb_jours_avant_activite)
                                {
                                    finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / cas.nb_dev).AddDays(-1);
                                }
                                else
                                {
                                    // si on vient de passer dans les 80 jours    (90 > 80)
                                    if (nb_jours_tot_dev <= nb_jours_avant_activite)
                                    {
                                        nb_jours_avec_aide = nb_jours_prevu_dev - nb_jours_avant_activite;   // ex ou on est a un total de 90 jours avec un projet de 30 jours (60)  :  nb_jours avec aide = 90 - 80 = 10
                                        nb_jours_sans_aide = nb_jours_prevu_dev - nb_jours_tot_dev - nb_jours_avec_aide; // nb_jours sans aide : 90 - 60 -10 = 20

                                        finPrevueDev = dateDebut.AddBusinessDays((nb_jours_sans_aide / (cas.nb_dev)) + (nb_jours_sans_aide / (cas.nb_dev + nbDevSupp))).AddDays(-1);  // ajoute 20 / dev std   + 10/ devstd + news
                                    }
                                    else
                                    {
                                        finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / (cas.nb_dev + nbDevSupp)).AddDays(-1);
                                    }

                                }
                                // =< 80; on incrémente sans compter les nouveaux
                                if (nb_jours_tot_mgt <= nb_jours_avant_activite)
                                {
                                    finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / cas.nb_chef_proj).AddDays(-1);
                                }
                                else
                                {
                                    if (nb_jours_tot_mgt <= nb_jours_avant_activite)
                                    {
                                        nb_jours_avec_aide = nb_jours_prevu_mgt - nb_jours_avant_activite;   // ex ou on est a un total de 90 jours avec un projet de 30 jours (60)  :  nb_jours avec aide = 90 - 80 = 10
                                        nb_jours_sans_aide = nb_jours_prevu_mgt - nb_jours_tot_mgt - nb_jours_avec_aide; // nb_jours sans aide : 90 - 60 -10 = 20

                                        finPrevueMgt = dateDebut.AddBusinessDays((nb_jours_sans_aide / (cas.nb_chef_proj)) + (nb_jours_sans_aide / (cas.nb_dev + nbMgtSupp))).AddDays(-1);
                                    }
                                    else
                                    {
                                        finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / (cas.nb_chef_proj + nbMgtSupp)).AddDays(-1);
                                    }
                                }
                                nb_jours_prevu_dev = nb_jours_tot_dev + proj.nb_dev_days;  // 60 + 30
                                nb_jours_prevu_mgt = nb_jours_tot_mgt + proj.nb_mgt_days;

                                // =< 80; on incrémente la date sans compter les nouveaux
                                if (nb_jours_prevu_dev <= nb_jours_avant_activite)
                                {
                                    finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / cas.nb_dev).AddDays(-1);
                                }
                                else
                                {
                                    // si on vient de passer dans les 80 jours    (90 > 80)
                                    if (nb_jours_tot_dev <= nb_jours_avant_activite)
                                    {
                                        nb_jours_avec_aide = nb_jours_prevu_dev - nb_jours_avant_activite;   // ex ou on est a un total de 90 jours avec un projet de 30 jours (60)  :  nb_jours avec aide = 90 - 80 = 10
                                        nb_jours_sans_aide = nb_jours_prevu_dev - nb_jours_tot_dev - nb_jours_avec_aide; // nb_jours sans aide : 90 - 60 -10 = 20

                                        finPrevueDev = dateDebut.AddBusinessDays((nb_jours_sans_aide / (cas.nb_dev)) + (nb_jours_sans_aide / (cas.nb_dev + nbDevSupp))).AddDays(-1);  // ajoute 20 / dev std   + 10/ devstd + news
                                    }
                                    else
                                    {
                                        finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / (cas.nb_dev + nbDevSupp)).AddDays(-1);
                                    }

                                }
                                // =< 80; on incrémente sans compter les nouveaux
                                if (nb_jours_tot_mgt <= nb_jours_avant_activite)
                                {
                                    finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / cas.nb_chef_proj).AddDays(-1);
                                }
                                else
                                {
                                    if (nb_jours_tot_mgt <= nb_jours_avant_activite)
                                    {
                                        nb_jours_avec_aide = nb_jours_prevu_mgt - nb_jours_avant_activite;   // ex ou on est a un total de 90 jours avec un projet de 30 jours (60)  :  nb_jours avec aide = 90 - 80 = 10
                                        nb_jours_sans_aide = nb_jours_prevu_mgt - nb_jours_tot_mgt - nb_jours_avec_aide; // nb_jours sans aide : 90 - 60 -10 = 20

                                        finPrevueMgt = dateDebut.AddBusinessDays((nb_jours_sans_aide / (cas.nb_chef_proj)) + (nb_jours_sans_aide / (cas.nb_dev + nbMgtSupp))).AddDays(-1);
                                    }
                                    else
                                    {
                                        finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / (cas.nb_chef_proj + nbMgtSupp)).AddDays(-1);
                                    }
                                }
                                nb_jours_tot_dev = nb_jours_prevu_dev;
                                nb_jours_tot_mgt = nb_jours_prevu_mgt;
                            }
                            // cas sans respect du temps d'embauche + efficience
                            else
                            {
                                // Calcul des dates prévues de fin de projet
                                finPrevueDev = dateDebut.AddBusinessDays(proj.nb_dev_days / (cas.nb_dev + nbDevSupp)).AddDays(-1);
                                finPrevueMgt = dateDebut.AddBusinessDays(proj.nb_mgt_days / (cas.nb_chef_proj + nbMgtSupp)).AddDays(-1);
                            }
                            

                            if (finPrevueDev > finPrevueMgt)
                            {
                                dateDebut = finPrevueDev;
                            }
                            else
                            {
                                dateDebut = finPrevueMgt;
                            }

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

                            dateDebut.AddDays(1);
                        }
                    }

                    Console.WriteLine("Cas réalisable avec ressources supplémentaires suivantes: " + nbDevSupp + " développeur(s); " + nbMgtSupp + " chefs de projet(s).");
                }
            }
            return "";
        }

        private static float getCoeffEfficience(Cas cas)
        {
            float f = (100 - (cas.efficience - 100)) / 100;
            return ((100-(cas.efficience-100))/100);
        }
    }
}

// Quand deadline égale, on priorise le projet qui prend le moins de temps
