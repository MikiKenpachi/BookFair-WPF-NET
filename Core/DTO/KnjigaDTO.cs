using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Core.DTO
{
    public class KnjigaDTO : IDataErrorInfo
    {
        private string _isbn;
        private string _naziv;
        private string _cena;
        private string _brojStrana;
        private string _godinaIzdanja;
        private Knjiga.Zanrovi? _zanr;
        private Izdavac _izdavac;

        public string ISBN { get => _isbn; set { _isbn = value; OnPropertyChanged(); } }
        public string Naziv { get => _naziv; set { _naziv = value; OnPropertyChanged(); } }
        public string Cena { get => _cena; set { _cena = value; OnPropertyChanged(); } }
        public string BrojStrana { get => _brojStrana; set { _brojStrana = value; OnPropertyChanged(); } }
        public string GodinaIzdanja { get => _godinaIzdanja; set { _godinaIzdanja = value; OnPropertyChanged(); } }
        public Knjiga.Zanrovi? Zanr { get => _zanr; set { _zanr = value; OnPropertyChanged(); } }
        public Izdavac Izdavac { get => _izdavac; set { _izdavac = value; OnPropertyChanged(); } }
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
                        if (god > DateTime.Now.Year) return "X";
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}