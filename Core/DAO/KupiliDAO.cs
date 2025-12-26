using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class KupiliDAO
    {
        private readonly Storage<Kupovina> _storage;
        private List<Kupovina> listaKupovina;

        public KupiliDAO()
        {
            _storage = new Storage<Kupovina>("kupovina.txt");
            listaKupovina = _storage.Load();
        }

        public List<Kupovina> GetAll()
        {
            return listaKupovina;
        }

        public void Save()
        {
            _storage.Save(listaKupovina);
        }

    }
}
