using SajamKnjigaProjekat.Core.DAO;
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
    /// Interaction logic for DodajPosetiocaProzor.xaml
    /// </summary>
    public partial class DodajPosetiocaProzor : Window
    {
        public Posetilac NoviPosetilac { get; private set; }
        public DodajPosetiocaProzor()
        {
            InitializeComponent();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // 1. Prikupljanje podataka iz TextBox-ova
            // Napomena: Proveri nazive svojih klasa (npr. Adresa)
            string odabraniTekst = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            StatusPosetioca status;
            if (odabraniTekst == "Redovan")
            {
                status = StatusPosetioca.R;
            }
            else
            {
                status = StatusPosetioca.V;
            }
          
            NoviPosetilac = new Posetilac
            {
                
               // BrClanskeKarte = txtBrClanske.Text,
                Ime = txtIme.Text,
                Prezime = txtPrezime.Text,
                DatumRodjenja = dpDatum.SelectedDate ?? DateTime.Now,
                Telefon = txtTelefon.Text,
                Email = txtEmail.Text,
                Status = status,
                // Ako je adresa poseban objekat:
                Adresa = new Adresa { Ulica = txtAdresa.Text }
            };
            

            // 2. Postavljanje DialogResult zatvara prozor i vraća "true" u MainWindow
            this.DialogResult = true;
        }

        private void txtBrClanske_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
