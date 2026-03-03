using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class DodajAutoraProzor : Window
    {
        public Autor NoviAutor { get; private set; }
        private AutorDTO _dto;

        public DodajAutoraProzor(Autor a = null)
        {
            InitializeComponent();

            if (a != null)
            {
                NoviAutor = a;
                _dto = new AutorDTO
                {
                    Ime = a.Ime,
                    Prezime = a.Prezime,
                    DatumRodjenja = a.Datum_rodjenja,
                    Telefon = a.Telefon,
                    Email = a.Email,
                    GodineIskustva = a.Godine_iskustva.ToString(),
                    BrojLk = a.Broj_lk,
                    Ulica = a.Adresa?.Ulica,
                    Broj = a.Adresa?.Broj,
                    Grad = a.Adresa?.Grad,
                    Drzava = a.Adresa?.Drzava
                };
                this.Title = Application.Current.FindResource("titleIzmeniAutora").ToString();
            }
            else
            {
                _dto = new AutorDTO();
                this.Title = Application.Current.FindResource("titleDodajNovogAutora").ToString();
            }

            this.DataContext = _dto;
            btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private void TxtPolje_Changed(object sender, EventArgs e)
        {
            if (_dto == null) return;
            _dto.DatumRodjenja = dpDatum.SelectedDate;  // ← ključna linija
            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private bool FormaJeValidna()
        {
            return string.IsNullOrEmpty(_dto[nameof(_dto.Ime)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Prezime)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Telefon)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Email)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.GodineIskustva)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.BrojLk)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Ulica)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Broj)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Grad)]) &&
                   string.IsNullOrEmpty(_dto[nameof(_dto.Drzava)]) &&
                   dpDatum.SelectedDate.HasValue;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (NoviAutor == null)
            {
                NoviAutor = new Autor();
                NoviAutor.Adresa = new Adresa();
            }

            NoviAutor.Ime = _dto.Ime;
            NoviAutor.Prezime = _dto.Prezime;
            NoviAutor.Datum_rodjenja = dpDatum.SelectedDate ?? DateTime.Now;
            NoviAutor.Telefon = _dto.Telefon;
            NoviAutor.Email = _dto.Email;
            NoviAutor.Godine_iskustva = int.Parse(_dto.GodineIskustva);
            NoviAutor.Broj_lk = _dto.BrojLk;
            NoviAutor.Adresa.Ulica = _dto.Ulica;
            NoviAutor.Adresa.Broj = _dto.Broj;
            NoviAutor.Adresa.Grad = _dto.Grad;
            NoviAutor.Adresa.Drzava = _dto.Drzava;

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}