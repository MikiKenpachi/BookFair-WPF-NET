using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Core.DTO
{
    public class KnjigaDTO : IDataErrorInfo
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
                        if (!Regex.IsMatch(ISBN.Trim(), @"^\d{6}$")) return "X";
                        break;

                    case nameof(Naziv):
                        if (string.IsNullOrWhiteSpace(Naziv)) return "X";
                        if (Naziv.Trim().Length < 2) return "X";
                        break;

                    case nameof(Cena):
                        if (string.IsNullOrWhiteSpace(Cena)) return "X";
                        if (!double.TryParse(Cena, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out double cenaVal)) return "X";
                        if (cenaVal <= 0) return "X";
                        break;

                    case nameof(BrojStrana):
                        if (string.IsNullOrWhiteSpace(BrojStrana)) return "X";
                        if (!int.TryParse(BrojStrana, out int straneVal) || straneVal <= 0) return "X";
                        break;

                    case nameof(GodinaIzdanja):
                        if (string.IsNullOrWhiteSpace(GodinaIzdanja)) return "X";
                        if (!int.TryParse(GodinaIzdanja, out int god)) return "X";
                        if (god < 1400 || god > DateTime.Now.Year) return "X";
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