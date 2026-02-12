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
            Roman,
            Triler,
            Kriminalistika,
            Fantastika,
            Naucna_Fantastika,
            Horor,
            Biografija,
            Istorijska,
            Drama,
            Poezija,
            Klasika,
            Psihologija,
            Decija_Literatura,
            Strucna_Literatura,
            Enciklopedija
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
        public List<string> ListaAutoraIDs { get; set; } = new List<string>();
        public string IzdavacID { get; set; }


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
                Izdavac != null ? Izdavac.Sifra : "", // samo ID izdavača
                ListaAutora != null ? string.Join(";", ListaAutora.Select(a => a.Broj_lk)) : "" // samo ID-evi autora
                // Kupci i lista želja se NE čuvaju
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

            // Ovde čuvamo samo identifikatore za povezivanje
            Izdavac = !string.IsNullOrEmpty(values[6]) ? new Izdavac { Sifra = values[6] } : null;

            if (!string.IsNullOrEmpty(values[7]))
            {
                ListaAutora = values[7]
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(broj => new Autor { Broj_lk = broj })
                    .ToList();
            }
            else
            {
                ListaAutora = new List<Autor>();
            }

            // Kupci i lista želja se popunjavaju kroz DataBinding
            Kupili = new List<Posetilac>();
            Na_listi_zelja = new List<Posetilac>();
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
