using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class IzdavacDTO : IDataErrorInfo
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }
        public Autor SefIzdavaca { get; set; } // Pretpostavljamo da Autor ima property 'GodineIskustva'

        // Ove liste mogu ostati radi prikaza u DataGrid-u
        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public List<Knjiga> ListaKnjiga { get; set; } = new List<Knjiga>();

        // --- IDataErrorInfo Implementacija ---

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string rezultat = string.Empty;

                switch (columnName)
                {
                    case nameof(Sifra):
                        if (string.IsNullOrWhiteSpace(Sifra))
                            rezultat = "Šifra izdavaca je obavezna.";
                        break;

                    case nameof(Naziv):
                        if (string.IsNullOrWhiteSpace(Naziv))
                            rezultat = "Naziv ne sme biti prazan.";
                        else if (Naziv.Length < 3)
                            rezultat = "Naziv mora imati bar 3 karaktera.";
                        break;

                    case nameof(SefIzdavaca):
                        if (SefIzdavaca == null)
                        {
                            rezultat = "Morate dodeliti šefa izdavača.";
                        }
                        // Ovde proveravamo tvoj uslov za > 5 godina iskustva
                        else if (SefIzdavaca.Godine_iskustva <= 5)
                        {
                            rezultat = $"Šef mora imati više od 5 godina iskustva (Trenutno: {SefIzdavaca.Godine_iskustva}).";
                        }
                        break;
                }

                return rezultat;
            }
        }
    }
}
