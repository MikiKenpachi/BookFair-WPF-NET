using System;
using System.Collections.Generic;
using Core.Storage.Serialization;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Knjiga : ISerializable
    {

        public string ISBN { get; set; }
        public string Naziv { get; set; }
        public string Zanr { get; set; }
        public string Godina_izdanja { get; set; }
        public string Cena { get; set; }
        public string Broj_strana { get; set; }
        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public Izdavac Izdavac { get; set; }
        public List<Posetilac> Kupili { get; set; } = new List<Posetilac>();
        public List<Posetilac> Na_listi_zelja { get; set; } = new List<Posetilac>();

        // Implement ISerializable members here
        public string[] ToCSV()
        {
            return new string[]
            {
                ISBN,
                Naziv,
                Zanr,
                Godina_izdanja,
                Cena,
                Broj_strana,
                Izdavac.Sifra,
                ListaAutora != null ? string.Join(";", ListaAutora) : "",
                Kupili != null ? string.Join(";", Kupili) : "",
                Na_listi_zelja != null ? string.Join(";", Na_listi_zelja) : ""
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 10)
                throw new ArgumentException("Invalid CSV data for Knjiga.");

            ISBN = values[0];
            Naziv = values[1];
            Zanr = values[2];
            Godina_izdanja = values[3];
            Cena = values[4];
            Broj_strana = values[5];
            Izdavac.Sifra = values[6];
            // ListaAutora deserialization would require more info about Autor
            //Kupili = new List<Posetilac>(values[8].Split(';'));
            //Na_listi_zelja = new List<Posetilac>(values[9].Split(';'));

       
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
