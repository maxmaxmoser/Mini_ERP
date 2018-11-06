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
        public int efficience { get; set; }
        public int duree_embauche { get; set; }
        public int duree_avt_efficacite { get; set; }
        public string projects { get; set; }
    }

    class CasList
    {

        public List<Cas> cas { get; set; }

    }
}
