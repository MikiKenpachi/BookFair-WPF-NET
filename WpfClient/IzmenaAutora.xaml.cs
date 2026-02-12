using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for IzmenaAutora.xaml
    /// </summary>
    public partial class IzmenaAutora : Window
    {
        public Autor SelektovaniAutor { get; set; }

        public IzmenaAutora(Autor autor)
        {
            InitializeComponent();
            this.SelektovaniAutor = autor;
            PopuniPolja();
        }

        private void PopuniPolja()
        {
            if (SelektovaniAutor != null)
            {
                txtBrojLK.Text = SelektovaniAutor.Broj_lk.ToString();
                txtIme.Text = SelektovaniAutor.Ime;
                txtPrezime.Text = SelektovaniAutor.Prezime;
                txtEmail.Text = SelektovaniAutor.Email;
                txtTelefon.Text = SelektovaniAutor.Telefon;
                txtIskustvo.Text = SelektovaniAutor.Godine_iskustva.ToString();
                dpDatumRodjenja.SelectedDate = SelektovaniAutor.Datum_rodjenja;

                if (SelektovaniAutor.Adresa != null)
                {
                    txtUlica.Text = SelektovaniAutor.Adresa.Ulica;
                    txtBroj.Text = SelektovaniAutor.Adresa.Broj;
                    txtGrad.Text = SelektovaniAutor.Adresa.Grad;
                    txtDrzava.Text = SelektovaniAutor.Adresa.Drzava;
                }
            }
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Ažuriranje podataka u objektu u memoriji
            SelektovaniAutor.Ime = txtIme.Text;
            SelektovaniAutor.Prezime = txtPrezime.Text;
            SelektovaniAutor.Email = txtEmail.Text;
            SelektovaniAutor.Telefon = txtTelefon.Text;
            SelektovaniAutor.Datum_rodjenja = dpDatumRodjenja.SelectedDate ?? SelektovaniAutor.Datum_rodjenja;

            if (int.TryParse(txtIskustvo.Text, out int iskustvo))
            {
                SelektovaniAutor.Godine_iskustva = iskustvo;
            }

            // Ažuriranje adrese
            if (SelektovaniAutor.Adresa != null)
            {
                SelektovaniAutor.Adresa.Ulica = txtUlica.Text;
                SelektovaniAutor.Adresa.Broj = txtBroj.Text;
                SelektovaniAutor.Adresa.Grad = txtGrad.Text;
                SelektovaniAutor.Adresa.Drzava = txtDrzava.Text;
            }

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
