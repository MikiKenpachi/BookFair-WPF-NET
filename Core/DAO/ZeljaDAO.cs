using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Core.DAO
{
    public class ZeljaDAO
    {
        private readonly Storage<Zelja> _storage;
        private List<Zelja> listaZelja;

        public ZeljaDAO()
        {
            _storage = new Storage<Zelja>("zelje.txt");
            listaZelja = _storage.Load();
        }

        public List<Zelja> GetAll()
        {
            return listaZelja;
        }

        // Sve želje jednog posjetioca
        public List<Zelja> GetByPosetilac(string brClanskeKarte)
        {
            return listaZelja
                .Where(z => z.Posetilac?.BrClanskeKarte == brClanskeKarte)
                .ToList();
        }

        // Da li posjetilac već ima tu knjigu na listi želja
        public bool PostojiZelja(string brClanskeKarte, string isbn)
        {
            return listaZelja.Any(z =>
                z.Posetilac?.BrClanskeKarte == brClanskeKarte &&
                z.Knjiga?.ISBN == isbn);
        }

        public void Add(Zelja z)
        {
            listaZelja.Add(z);
            _storage.Save(listaZelja);
        }

        // Uklanja željу po posetiocu i knjizi
        public void Remove(string brClanskeKarte, string isbn)
        {
            listaZelja = listaZelja
                .Where(z => !(z.Posetilac?.BrClanskeKarte == brClanskeKarte &&
                              z.Knjiga?.ISBN == isbn))
                .ToList();
            _storage.Save(listaZelja);
        }

        // Uklanja konkretan objekat
        public void Remove(Zelja zelja)
        {
            listaZelja.Remove(zelja);
            _storage.Save(listaZelja);
        }

        // Uklanja sve želje jednog posjetioca (korisno pri brisanju posjetioca)
        public void RemoveByPosetilac(string brClanskeKarte)
        {
            listaZelja = listaZelja
                .Where(z => z.Posetilac?.BrClanskeKarte != brClanskeKarte)
                .ToList();
            _storage.Save(listaZelja);
        }

        public void Save()
        {
            _storage.Save(listaZelja);
        }
    }
}