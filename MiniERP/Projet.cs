using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniERP
{
    public class Projet
    {
        public string nom { get; set; }
        public int nb_dev_days { get; set; }
        public int nb_mgt_days { get; set; }
        public string deadline { get; set; }

        public override string ToString()
        {
            return nom;
        }
    }

    public class ProjetsList
    {
        public List<Projet> Projets { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Projet proj in Projets)
            {
                sb.Append("[" + proj.ToString() + "]");
            }

            return sb.ToString();
        }
    }
}
