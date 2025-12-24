using System;
using System.Collections.Generic;
namespace SajamKnjigaProjekat.Core.Models
{
    public class Izdavac
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }
        public Autor SefIzdavaca { get; set; }

        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public List<Knjiga> ListaKnjiga { get; set; } = new List<Knjiga>();


        public Izdavac() { }

        public void DodajAutora(Autor autor)
        {
            if (!ListaAutora.Contains(autor))
                ListaAutora.Add(autor);
        }


        public void DodajKnjigu(Knjiga knjiga)
        {
            if(!ListaKnjiga.Contains(knjiga))
                ListaKnjiga.Add(knjiga);    
        }
    }
}