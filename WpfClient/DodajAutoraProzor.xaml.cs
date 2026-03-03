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

        public DodajAutoraProzor(Autor postojeciAutor = null)
        {
            InitializeComponent();

            if (postojeciAutor != null)
            {
                // Režim IZMENE
                NoviAutor = postojeciAutor;
                _dto = new AutorDTO
                {
                    Ime = postojeciAutor.Ime,
                    Prezime = postojeciAutor.Prezime,
                    DatumRodjenja = postojeciAutor.Datum_rodjenja,
                    Telefon = postojeciAutor.Telefon,
                    Email = postojeciAutor.Email,
                    GodineIskustva = postojeciAutor.Godine_iskustva.ToString(),
                    BrojLk = postojeciAutor.Broj_lk,
                    Ulica = postojeciAutor.Adresa?.Ulica,
                    Broj = postojeciAutor.Adresa?.Broj,
                    Grad = postojeciAutor.Adresa?.Grad,
                    Drzava = postojeciAutor.Adresa?.Drzava
                };
                this.Title = Application.Current.FindResource("titleIzmeniAutora").ToString();
            }
            else
            {
                // Režim DODAVANJA
                _dto = new AutorDTO();
                this.Title = Application.Current.FindResource("titleDodajNovogAutora").ToString();
            }

            this.DataContext = _dto;
            btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private void TxtPolje_Changed(object sender, EventArgs e)
        {
            // WPF automatski ažurira DTO preko Bindinga, mi samo proveravamo stanje dugmeta
            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private bool FormaJeValidna()
        {
            // Spisak polja koja moraju biti bez greške
            string[] poljaZaProveru = {
            nameof(_dto.Ime), nameof(_dto.Prezime), nameof(_dto.Email),
            nameof(_dto.BrojLk), nameof(_dto.GodineIskustva), nameof(_dto.Grad),
            nameof(_dto.DatumRodjenja)
        };

            // Proveravamo da li bilo koje polje vraća poruku o grešci
            foreach (var p in poljaZaProveru)
            {
                if (!string.IsNullOrEmpty(_dto[p])) return false;
            }

            // Dodatna provera da polja nisu ostala null (inicijalno stanje)
            return !string.IsNullOrWhiteSpace(_dto.Ime) &&
                   !string.IsNullOrWhiteSpace(_dto.Prezime) &&
                   _dto.DatumRodjenja.HasValue;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Dupla provera pre snimanja
            if (!FormaJeValidna())
            {
                string poruka = Application.Current.FindResource("msgIspraviGreske").ToString();
                MessageBox.Show(poruka, "Validacija", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NoviAutor == null)
            {
                NoviAutor = new Autor { Adresa = new Adresa() };
            }

            // Mapiranje sa DTO na Model
            NoviAutor.Ime = _dto.Ime.Trim();
            NoviAutor.Prezime = _dto.Prezime.Trim();
            NoviAutor.Datum_rodjenja = _dto.DatumRodjenja.Value;
            NoviAutor.Telefon = _dto.Telefon?.Trim();
            NoviAutor.Email = _dto.Email.Trim();
            NoviAutor.Godine_iskustva = int.Parse(_dto.GodineIskustva);
            NoviAutor.Broj_lk = _dto.BrojLk.Trim();

            if (NoviAutor.Adresa == null) NoviAutor.Adresa = new Adresa();
            NoviAutor.Adresa.Ulica = _dto.Ulica?.Trim();
            NoviAutor.Adresa.Broj = _dto.Broj?.Trim();
            NoviAutor.Adresa.Grad = _dto.Grad?.Trim();
            NoviAutor.Adresa.Drzava = _dto.Drzava?.Trim();

            this.DialogResult = true;
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}