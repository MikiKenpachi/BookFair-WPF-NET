using Core.DAO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for IzdavaciProzor.xaml
    /// </summary>
    /// 

    public partial class IzdavaciProzor : Window
    {
        // Koristimo referencu na listu iz MainWindow
        public ObservableCollection<Izdavac> ListaIzdavaca { get; set; }

        public IzdavaciProzor(ObservableCollection<Izdavac> listaIzGlavnog)
        {
            InitializeComponent();

            // Povezujemo se na istu listu koju koristi MainWindow
            ListaIzdavaca = listaIzGlavnog;
            DataGridIzdavaci.ItemsSource = ListaIzdavaca;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            
            AutorDAO autorDao = new AutorDAO();
            List<Autor> sviAutori = autorDao.GetAll();
            DodajIzdavacaProzor prozor = new DodajIzdavacaProzor(sviAutori, null);
            prozor.Owner = this;

            if (prozor.ShowDialog() == true)
            {
                // SAMO dodajemo u listu. NE zovemo dao.Add()!
                ListaIzdavaca.Add(prozor.NoviIzdavac);
            }
        }
        private void BtnPrikaziAutore_Click(object sender, RoutedEventArgs e)
        {
            var selektovaniIzdavac = DataGridIzdavaci.SelectedItem as Izdavac;
            if (selektovaniIzdavac == null) { /* MessageBox... */ return; }

            // 1. Dobavi sve adrese (npr. preko DAO klase)
            AdresaDAO adresaDao = new AdresaDAO();
            List<Adresa> sveAdrese = adresaDao.GetAll();

            // 2. Izvuci autore izdavača
            var autori = selektovaniIzdavac.ListaKnjiga
                .SelectMany(k => k.ListaAutora)
                .Distinct()
                .ToList();

            // 3. Poveži svakog autora sa njegovom adresom preko Broj_lk
            foreach (var autor in autori)
            {
                // Tražimo adresu koja ima isti Broj_lk kao autor
                autor.Adresa = sveAdrese.FirstOrDefault(a => a.VlasnikID == autor.Broj_lk);
            }

            // 4. Pošalji listu sa povezanim adresama u prozor
            AutoriIzdavacaProzor prozor = new AutoriIzdavacaProzor(autori, selektovaniIzdavac.Naziv);
            prozor.ShowDialog();
        }

        private void BtnPrikaziKnjige_Click(object sender, RoutedEventArgs e)
        {
            var selektovaniIzdavac = DataGridIzdavaci.SelectedItem as Izdavac;

            if (selektovaniIzdavac == null)
            {
                MessageBox.Show(
                    "Molimo selektujte izdavača.",
                    "Obaveštenje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // ListaKnjiga je već popunjena u DataBinding.PoveziSve()
            // — samo je prosleđujemo novom prozoru
            List<Knjiga> knjige = selektovaniIzdavac.ListaKnjiga;

            KnjigeIzdavacaProzor prozor = new KnjigeIzdavacaProzor(knjige, selektovaniIzdavac.Naziv);
            prozor.Owner = this;
            prozor.ShowDialog();
        }

        private void BtnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            Izdavac selektovan = (Izdavac)DataGridIzdavaci.SelectedItem;
            if (selektovan != null)
            {
                // You need to provide a List<Autor> as the first argument.
                AutorDAO autorDao = new AutorDAO();
                List<Autor> sviAutori = autorDao.GetAll();
                DodajIzdavacaProzor prozor = new DodajIzdavacaProzor(sviAutori, selektovan);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    DataGridIzdavaci.Items.Refresh();
                    // Komentar "NE zovemo dao.Update()!" — izmena se čuva samo u memoriji OC-a
                }
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            Izdavac selektovan = (Izdavac)DataGridIzdavaci.SelectedItem;

            if (selektovan != null)
            {
                // Izvlačimo lokalizovane tekstove
                string poruka = Application.Current.FindResource("msgPotvrdaBrisanjaIzdavaca").ToString();
                string naslov = Application.Current.FindResource("titlePotvrda").ToString();

                if (MessageBox.Show(poruka, naslov, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Uklanjamo iz liste
                    ListaIzdavaca.Remove(selektovan);
                }
            }
            else
            {
                // Opciono: poruka ako ništa nije selektovano
                string porukaGreska = Application.Current.FindResource("msgNisteSelektovaliIzdavaca").ToString();
                string naslovObavestenje = Application.Current.FindResource("titleObavestenje").ToString();

                MessageBox.Show(porukaGreska, naslovObavestenje, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
