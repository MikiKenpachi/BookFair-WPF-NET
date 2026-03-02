using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class KnjigaDTO
    {
        public string ISBN { get; set; }
        public string Naziv { get; set; }
        public string Cena { get; set; }
        public string BrojStrana { get; set; }
        public string GodinaIzdanja { get; set; }
        public Knjiga.Zanrovi? Zanr { get; set; }
        public Izdavac Izdavac { get; set; }
        public List<Autor> Autori { get; set; } = new List<Autor>();

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(ISBN):
                        if (string.IsNullOrWhiteSpace(ISBN)) return "X";
                        // ISBN obično ima 10 ili 13 cifara (opciona dodatna provera)
                        if (ISBN.Length < 10) return "X";
                        break;

                    case nameof(Naziv):
                        if (string.IsNullOrWhiteSpace(Naziv)) return "X";
                        break;

                    case nameof(Cena):
                        if (string.IsNullOrWhiteSpace(Cena) || !double.TryParse(Cena, out _)) return "X";
                        break;

                    case nameof(BrojStrana):
                        if (string.IsNullOrWhiteSpace(BrojStrana) || !int.TryParse(BrojStrana, out _)) return "X";
                        break;

                    case nameof(GodinaIzdanja):
                        if (string.IsNullOrWhiteSpace(GodinaIzdanja)) return "X";
                        if (!int.TryParse(GodinaIzdanja, out int god) || god > DateTime.Now.Year) return "X";
                        break;

                    case nameof(Zanr):
                        if (Zanr == null) return "X";
                        break;

                    case nameof(Izdavac):
                        if (Izdavac == null) return "X";
                        break;
                }
                return string.Empty;
            }
        }
    }
}
