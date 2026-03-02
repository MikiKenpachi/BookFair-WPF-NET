using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfClient
{
    
public partial class DodajAutoraProzor : Window
    {
  
            public Autor? NoviAutor { get; private set; }

            // Instanca DTO-a koja sadrži svu logiku validacije ("X" pravila)
            private AutorDTO _validator = new AutorDTO();

            public DodajAutoraProzor(Autor a = null)
            {
                InitializeComponent();

                if (a != null)
                {
                    NoviAutor = a;
                    PopuniPolja();
                    this.Title = Application.Current.FindResource("titleIzmeniAutora").ToString();
                }
                else
                {
                    this.Title = Application.Current.FindResource("titleDodajNovogAutora").ToString();
                }
            }

            private void PopuniPolja()
            {
                if (NoviAutor == null) return;

                txtIme.Text = NoviAutor.Ime;
                txtPrezime.Text = NoviAutor.Prezime;
                dpDatum.SelectedDate = NoviAutor.Datum_rodjenja;
                txtTelefon.Text = NoviAutor.Telefon;
                txtEmail.Text = NoviAutor.Email;
                txtBrojLk.Text = NoviAutor.Broj_lk;
                txtIskustvo.Text = NoviAutor.Godine_iskustva.ToString();

                if (NoviAutor.Adresa != null)
                {
                    txtUlica.Text = NoviAutor.Adresa.Ulica;
                    txtBroj.Text = NoviAutor.Adresa.Broj;
                    txtGrad.Text = NoviAutor.Adresa.Grad;
                    txtDrzava.Text = NoviAutor.Adresa.Drzava;
                }
            }

            private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
            {
                // 1. Prenosimo podatke iz UI kontrola u DTO validator
                SinhronizujValidatorIzForme();

                // 2. Provera validnosti oslanjanjem isključivo na DTO (tražimo "X")
                if (!JeLiValidno())
                {
                    string poruka = Application.Current.FindResource("msgPopuniteSvaPolja").ToString();
                    string naslovGreska = Application.Current.FindResource("errorTitle").ToString();
                    MessageBox.Show(poruka, naslovGreska, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 3. Ako je validacija prošla, kreiramo/ažuriramo realni model
                if (NoviAutor == null)
                {
                    NoviAutor = new Autor { Adresa = new Adresa() };
                }

                MapirajValidatorUModel();

                this.DialogResult = true;
                this.Close();
            }

            private void SinhronizujValidatorIzForme()
            {
                _validator.Ime = txtIme.Text;
                _validator.Prezime = txtPrezime.Text;
                _validator.Email = txtEmail.Text;
                _validator.Telefon = txtTelefon.Text;
                _validator.BrojLk = txtBrojLk.Text;
                _validator.DatumRodjenja = dpDatum.SelectedDate;
                _validator.Ulica = txtUlica.Text;
                _validator.Broj = txtBroj.Text;
                _validator.Grad = txtGrad.Text;

                if (int.TryParse(txtIskustvo.Text, out int iskustvo))
                    _validator.GodineIskustva = iskustvo;
                else
                    _validator.GodineIskustva = -1; // Izaziva "X" u validatoru ako nije broj
            }

            private bool JeLiValidno()
            {
                // Proveravamo samo ključna polja definisana u AutorDTO
                string[] polja = {
                nameof(AutorDTO.Ime),
                nameof(AutorDTO.Prezime),
                nameof(AutorDTO.Email),
                nameof(AutorDTO.BrojLk),
                nameof(AutorDTO.Ulica),
                nameof(AutorDTO.Grad),
                nameof(AutorDTO.DatumRodjenja),
                nameof(AutorDTO.GodineIskustva)
            };

                foreach (var p in polja)
                {
                    if (_validator[p] == "X") return false;
                }
                return true;
            }

            private void MapirajValidatorUModel()
            {
                NoviAutor.Ime = _validator.Ime.Trim();
                NoviAutor.Prezime = _validator.Prezime.Trim();
                NoviAutor.Email = _validator.Email.Trim();
                NoviAutor.Telefon = _validator.Telefon.Trim();
                NoviAutor.Broj_lk = _validator.BrojLk.Trim();
                NoviAutor.Datum_rodjenja = _validator.DatumRodjenja ?? DateTime.Now;
                NoviAutor.Godine_iskustva = _validator.GodineIskustva;

                if (NoviAutor.Adresa != null)
                {
                    NoviAutor.Adresa.Ulica = _validator.Ulica.Trim();
                    NoviAutor.Adresa.Broj = _validator.Broj.Trim();
                    NoviAutor.Adresa.Grad = _validator.Grad.Trim();
                    NoviAutor.Adresa.Drzava = txtDrzava.Text.Trim();
                }
            }

            private void BtnOdustani_Click(object sender, RoutedEventArgs e)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    
}
