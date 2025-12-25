using System;
using System.Collections.Generic;
using Core.Storage.Serialization;
namespace SajamKnjigaProjekat.Core.Models
{   
    public class Adresa : ISerializable
    {

        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }


        public Adresa(){ }

        public Adresa(string ulica, string grad, string broj, string drzava)
        {
            Ulica = ulica;
            Broj = broj;
            Grad = grad;
            Drzava = drzava;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
          Ulica,
          Grad,
          Broj,
          Drzava
                
                };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 4)
                throw new ArgumentException("Invalid CSV data for Autor.");

            Ulica = values[0];
            Grad = values[1];
            Broj = values[2];
            Drzava= values[3];
         
        }
    }
}

