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
    /// Interaction logic for IzmenaPosetioca.xaml
    /// </summary>
    public partial class IzmenaPosetioca : Window
    {
        public Posetilac SelektovaniPosetilac { get; set; }

        public IzmenaPosetioca(Posetilac posetilac)
        {
            InitializeComponent();
            this.SelektovaniPosetilac = posetilac;
            PopuniPolja();
        }

        private void PopuniPolja()
        {
            txtBrClanske.Text = SelektovaniPosetilac.BrClanskeKarte;
            txtIme.Text = SelektovaniPosetilac.Ime;
            txtPrezime.Text = SelektovaniPosetilac.Prezime;
            txtEmail.Text = SelektovaniPosetilac.Email;
            txtTelefon.Text = SelektovaniPosetilac.Telefon;
            dpDatumRodjenja.SelectedDate = SelektovaniPosetilac.DatumRodjenja;

            // Status
            if (SelektovaniPosetilac.Status == StatusPosetioca.R) cmbStatus.SelectedIndex = 0;
            else cmbStatus.SelectedIndex = 1;

            // POPUNJAVANJE ADRESE
            if (SelektovaniPosetilac.Adresa != null)
            {
                txtUlica.Text = SelektovaniPosetilac.Adresa.Ulica;
                txtBroj.Text = SelektovaniPosetilac.Adresa.Broj;
                txtGrad.Text = SelektovaniPosetilac.Adresa.Grad;
                txtDrzava.Text = SelektovaniPosetilac.Adresa.Drzava;
            }
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Ažuriranje podataka o posetiocu
            SelektovaniPosetilac.Ime = txtIme.Text;
            SelektovaniPosetilac.Prezime = txtPrezime.Text;
            SelektovaniPosetilac.Email = txtEmail.Text;
            SelektovaniPosetilac.Telefon = txtTelefon.Text;
            SelektovaniPosetilac.DatumRodjenja = dpDatumRodjenja.SelectedDate ?? SelektovaniPosetilac.DatumRodjenja;
            SelektovaniPosetilac.Status = (cmbStatus.SelectedIndex == 0) ? StatusPosetioca.R : StatusPosetioca.V;

            // Ažuriranje polja ADRESE
            if (SelektovaniPosetilac.Adresa != null)
            {
                SelektovaniPosetilac.Adresa.Ulica = txtUlica.Text;
                SelektovaniPosetilac.Adresa.Broj = txtBroj.Text;
                SelektovaniPosetilac.Adresa.Grad = txtGrad.Text;
                SelektovaniPosetilac.Adresa.Drzava = txtDrzava.Text;
                // VlasnikID ostaje isti jer se osoba nije promenila
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
