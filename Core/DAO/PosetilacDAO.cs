using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajamKnjigaProjekat.Core.DAO
{
    public class PosetilacDAO
    {
        private List<Posetilac> listaPosetilaca = new List<Posetilac>();

        public List<Posetilac> GetALL()
        {
            return listaPosetilaca;
        }

        public void Add(Posetilac p)
        {
            listaPosetilaca.Add(p);
        }

        public void Remove(Posetilac p)
        {
            listaPosetilaca.Remove(p);
        }

        public Posetilac GetByClanskaKarta(string brKarte)
        {
            foreach (var p in listaPosetilaca)
            {
                if (p.BrClanskeKarte == brKarte)
                    return p;
            }
            return null;
        }
    }
}
