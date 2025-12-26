using Core.Storage.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Knjiga : ISerializable
    {
        public enum Zanrovi
        {
            Romantika,
            Naučna_fantastika,
            Misterija,
            Biografija,
            Istorijski_roman,
            Fantazija,
            Triler,
            Horor,
            Poezija,
            Drama
        }

        public string ISBN { get; set; }
        public string Naziv { get; set; }
        public Zanrovi Zanr { get; set; }
        public string Godina_izdanja { get; set; }
        public string Cena { get; set; }
        public string Broj_strana { get; set; }
        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public Izdavac Izdavac { get; set; }
        public List<Posetilac> Kupili { get; set; } = new List<Posetilac>();
        public List<Posetilac> Na_listi_zelja { get; set; } = new List<Posetilac>();

        
        public string[] ToCSV()
        {
            return new string[]
            {
                ISBN,
                Naziv,
                Zanr.ToString(),
                Godina_izdanja,
                Cena,
                Broj_strana,
                Izdavac != null ? Izdavac.Sifra : "", // Čuvaj šifru izdavača
                ListaAutora != null ? string.Join(";", ListaAutora.Select(a => a.Broj_lk)) : "",
                ListaAutora != null ? string.Join(";", ListaAutora) : "",
                Kupili != null ? string.Join(";", Kupili) : "",
                Na_listi_zelja != null ? string.Join(";", Na_listi_zelja) : ""
            };
        }

        public void FromCSV(string[] values)
        {
            ISBN = values[0];
            Naziv = values[1];
            Zanr = (Zanrovi)Enum.Parse(typeof(Zanrovi), values[2]);
            Godina_izdanja = values[3];
            Cena = values[4];
            Broj_strana = values[5];

        }

        public void DodajListuAutora(Autor autor)
        {
            if (!ListaAutora.Contains(autor))
                ListaAutora.Add(autor);
        }

        public void DodajuKupili(Posetilac posetilac)
        {
            if (!Kupili.Contains(posetilac))
                Kupili.Add(posetilac);
        }
        public void DodajuListuZelja(Posetilac posetilac)
        {
            if (!Na_listi_zelja.Contains(posetilac))
                Na_listi_zelja.Add(posetilac);
        }


    }
}
