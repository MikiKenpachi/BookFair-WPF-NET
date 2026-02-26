using Core.DAO;
using Core.DTO;
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
        public IzdavacDTO MojDTo { get; set; }
        public List<Autor> SviAutori { get; set; }

        // 1. DODAJ OVO: Ovo svojstvo omogućava glavnom prozoru da "izvuče" rezultat
        public Izdavac NoviIzdavac { get; private set; }

        public DodajIzdavacaProzor(List<Autor> sviAutori, Izdavac postojeciIzdavac = null)
        {
            InitializeComponent();

            if (postojeciIzdavac != null)
            {
                // 1. Uzimamo sve knjige izdavaca -> iz svake knjige uzimamo LISTU autora -> spajamo u jednu listu
                // 'SelectMany' ide kroz svaku knjigu i "skuplja" autore u jedan veliki niz
                var autoriKojiRadeZaIzdavaca = postojeciIzdavac.ListaKnjiga
                    .SelectMany(k => k.ListaAutora)
                    .GroupBy(a => a.Broj_lk) // Grupišemo po ID-u da izbegnemo duplikate ako autor ima više knjiga
                    .Select(g => g.First())
                    .ToList();

                cbSefovi.ItemsSource = autoriKojiRadeZaIzdavaca;

                // 2. Sada tražimo šefa unutar te filtrirane liste
                // 'a' je ovde JEDAN autor, pa 'a.Broj_lk' sada radi!
                var selektovaniSef = autoriKojiRadeZaIzdavaca
                    .FirstOrDefault(a => a.Broj_lk == postojeciIzdavac.SefIzdavaca?.Broj_lk);

                MojDTo = new IzdavacDTO
                {
                    Sifra = postojeciIzdavac.Sifra,
                    Naziv = postojeciIzdavac.Naziv,
                    SefIzdavaca = selektovaniSef,
                    ListaAutora = postojeciIzdavac.ListaAutora,
                    ListaKnjiga = postojeciIzdavac.ListaKnjiga
                };

                this.Title = "Izmena izdavača";
                txtSifra.IsEnabled = false;
            }
            else
            {
                // Za novog izdavača nudimo sve autore jer još nema svojih knjiga
                cbSefovi.ItemsSource = sviAutori;
                MojDTo = new IzdavacDTO();
                this.Title = "Dodaj novog izdavača";
            }

            this.DataContext = MojDTo;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Validacija preko DTO-a
            if (!string.IsNullOrEmpty(MojDTo["Sifra"]) ||
                !string.IsNullOrEmpty(MojDTo["Naziv"]) ||
                !string.IsNullOrEmpty(MojDTo["SefIzdavaca"]))
            {
               MessageBox.Show("Molimo ispravite greške označene crvenom bojom!", "Validacija");
                return;
            }

            // 2. MAPIRANJE: Popunjavamo javno svojstvo klase podacima iz DTO-a
            NoviIzdavac = new Izdavac
            {
                Sifra = MojDTo.Sifra,
                Naziv = MojDTo.Naziv,
                SefIzdavaca = MojDTo.SefIzdavaca,
                ListaAutora = MojDTo.ListaAutora,
                ListaKnjiga = MojDTo.ListaKnjiga
            };

            this.DialogResult = true;
        }



        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
