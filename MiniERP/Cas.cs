using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniERP
{
    class Cas
    {
        public int id { get; set; }
        public string date_depart { get; set; }
        public int nb_dev { get; set; }
        public int nb_chef_proj { get; set; }
        public float efficience { get; set; }
        public int duree_embauche { get; set; }
        public int duree_avt_efficacite { get; set; }
        public string projects { get; set; }

        public override string ToString()
        {
            return "Cas " + id + " - Date de départ: " + date_depart + "\n\tEfficicence " + efficience + "%\n\t" + projects.Split(',').Count() + " projets: " + projects.ToString() + "\n\t" + nb_dev + " développeurs, " + nb_chef_proj + " chefs de projet";
        }
    }

    class CasList
    {
        public List<Cas> Cas { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach(Cas cas in Cas)
            {
                sb.AppendLine(cas.ToString());
            }

            return sb.ToString();
        }

    }
}
