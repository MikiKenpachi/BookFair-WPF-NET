using Core.DAO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DodajIzdavacaProzor.xaml
    /// </summary>
    public partial class DodajIzdavacaProzor : Window
    {
        public Izdavac Izdavac { get; set; }

        // Konstruktor prima parametar (ako je null, znači dodajemo novog)

        public ObservableCollection<Izdavac> ListaIzdavaca { get; set; }

        public Izdavac? NoviIzdavac { get; private set; }
        public DodajIzdavacaProzor(Izdavac postojeciIzdavac = null)
        {
            InitializeComponent();

            // Učitavanje iz fajla preko DAO klase
            var dao = new IzdavacDAO();
            ListaIzdavaca = new ObservableCollection<Izdavac>(dao.GetAll());



            if (postojeciIzdavac != null)
            {
                // REŽIM IZMENE
                Izdavac = postojeciIzdavac;
                txtSifra.Text = Izdavac.Sifra;
                txtSifra.IsEnabled = false; // Šifru ne damo da menja
                txtNaziv.Text = Izdavac.Naziv;

                // Postavljamo selektovanog šefa
                // (Moraš imati ispravno implementiran Equals u klasi Autor ili da se reference poklapaju)
                // Ako ovo ne radi odmah, koristi: cbSefovi.SelectedValue = Izdavac.SefIzdavaca.Broj_lk;
                cbSefovi.SelectedItem = Izdavac.SefIzdavaca;

                this.Title = "Izmena izdavača";
            }
            else
            {
                // REŽIM DODAVANJA
                Izdavac = new Izdavac();
                this.Title = "Dodaj novog izdavača";
            }
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSifra.Text) || string.IsNullOrWhiteSpace(txtNaziv.Text))
            {
                MessageBox.Show("Popunite sva polja!", "Greška");
                return;
            }

            if (cbSefovi.SelectedItem == null)
            {
                MessageBox.Show("Morate izabrati šefa!", "Greška");
                return;
            }

            // Popunjavanje objekta podacima sa forme
            Izdavac.Sifra = txtSifra.Text;
            Izdavac.Naziv = txtNaziv.Text;
            Izdavac.SefIzdavaca = (Autor)cbSefovi.SelectedItem;

            this.DialogResult = true; // Ovo zatvara prozor i vraća 'true' u glavni prozor
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
