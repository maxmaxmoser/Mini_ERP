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
    }

    public class ProjetsList
    {
        public List<Projet> projets { get; set; }
    }
}
