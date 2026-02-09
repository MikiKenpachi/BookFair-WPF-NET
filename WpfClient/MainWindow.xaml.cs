using Core;
using Core.DAO;
using Core.Storage.Serialization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Posetilac> Posetioci { get; set; }
        public ObservableCollection<Autor> Autori { get; set; }
        public ObservableCollection<Knjiga> Knjige { get; set; }

        KnjigaDAO knjigaDao = new KnjigaDAO();
        AutorDAO autorDao = new AutorDAO();
        AdresaDAO adresaDao = new AdresaDAO();
        IzdavacDAO izdavacDao = new IzdavacDAO();
        PosetilacDAO posetilacDao = new PosetilacDAO();
        public MainWindow()
        {

            InitializeComponent(); // Prvo inicijalizuj komponente
          

            List<Posetilac> sviPosetioci = posetilacDao.GetAll();
            List<Autor> sviAutori = autorDao.GetAll();
            var listaIzFajla = knjigaDao.GetAll();


            foreach (var p in sviPosetioci)
            {
                // Koristimo BrClanskeKarte kao ID vlasnika (ili BrLicneKarte, proveri šta ti je ID)
                p.Adresa = adresaDao.GetByVlasnikID(p.BrClanskeKarte);
            }

            foreach (var p in sviAutori)
            {
                // Koristimo BrClanskeKarte kao ID vlasnika (ili BrLicneKarte, proveri šta ti je ID)
                p.Adresa = adresaDao.GetByVlasnikID(p.Broj_lk);
            }

            // 4. Sada napuni ObservableCollection uparenim podacima
            Posetioci = new ObservableCollection<Posetilac>(sviPosetioci);
            Autori = new ObservableCollection<Autor>(sviAutori);
            Knjige = new ObservableCollection<Knjiga>(listaIzFajla);


            // 5. Poveži sa DataGrid-om
            DataGridPosetioci.ItemsSource = Posetioci;
            this.DataContext = this;
        }
        


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           this.Width = SystemParameters.PrimaryScreenWidth * 0.75;
           this.Height = SystemParameters.PrimaryScreenHeight * 0.75;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            // Proveravamo da li je korisnik na tabu "Posetioci"
            if (MainTabControl.SelectedIndex == 0)
            {
                // Kreiramo instancu novog prozora
                DodajPosetiocaProzor dijaloškiProzor = new DodajPosetiocaProzor();

                // Postavljamo MainWindow kao vlasnika (da bi iskočio tačno ispred njega)
                dijaloškiProzor.Owner = this;


                // ShowDialog() zaustavlja rad u MainWindow dok se ovaj prozor ne zatvori
                if (dijaloškiProzor.ShowDialog() == true)
                {
                    int nextId = posetilacDao.GetAll().Count + 1;  // broj novog posetioca
                    string brClanskeKarte = $"CK-{nextId}";
                    // 1. Uzimamo kreiranog posetioca iz dijaloga
                    Posetilac p = dijaloškiProzor.NoviPosetilac;
                    Adresa d = dijaloškiProzor.NoviPosetilac.Adresa;

                    p.BrClanskeKarte=brClanskeKarte;

                    if (p != null)
                    {
                        // 2. Dodajemo ga u DAO (da se upiše u fajl)
                        posetilacDao.Add(p);
                        adresaDao.Add(d);

                        // 3. Dodajemo ga u ObservableCollection (da se odmah pojavi u Gridu)
                        Posetioci.Add(p);
                    }
                }
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {

            int aktivniTab = MainTabControl.SelectedIndex;

            if (aktivniTab == 2) // Ako je selektovan tab "Knjige"
            {
                // 2. Uzimamo selektovani red i "kastujemo" ga u klasu Knjiga
                Knjiga selektovanaKnjiga = (Knjiga)dbKnjige.SelectedItem;

                // 3. Proveravamo da li je korisnik uopšte išta kliknuo
                if (selektovanaKnjiga != null)
                {
                    // Potvrda brisanja (opciono, ali preporučljivo)
                    var result = MessageBox.Show("Da li ste sigurni?", "Potvrda", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // A) Brišemo iz memorijske liste (ObservableCollection) 
                        // Ovo odmah sklanja red sa ekrana!
                        Knjige.Remove(selektovanaKnjiga);
                        

                        // B) Brišemo iz fajla preko DAO
                        knjigaDao.Remove(selektovanaKnjiga);
                       
                    }
                }
                else
                {
                    MessageBox.Show("Prvo selektujte red u tabeli koji želite da obrišete!");
                }
            }

            if (aktivniTab == 1) // Ako je selektovan tab "Autor"
            {
               
                Autor selektovanAutor = (Autor)dbAutori.SelectedItem;

                // 3. Proveravamo da li je korisnik uopšte išta kliknuo
                if (selektovanAutor != null)
                {
                    // Potvrda brisanja (opciono, ali preporučljivo)
                    var result = MessageBox.Show("Da li ste sigurni?", "Potvrda", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // A) Brišemo iz memorijske liste (ObservableCollection) 
                        // Ovo odmah sklanja red sa ekrana!
                        Autori.Remove(selektovanAutor);

                        // B) Brišemo iz fajla preko DAO
                        autorDao.Remove(selektovanAutor);
                        adresaDao.Remove(selektovanAutor.Adresa.Ulica);
                    }
                }
                else
                {
                    MessageBox.Show("Prvo selektujte red u tabeli koji želite da obrišete!");
                }
            }

            if (aktivniTab == 0) // Ako je selektovan tab "Posetioci"
            {
                // 2. Uzimamo selektovani red i "kastujemo" ga u klasu Knjiga
                Posetilac selektovanPosetilac = (Posetilac)DataGridPosetioci.SelectedItem;

                // 3. Proveravamo da li je korisnik uopšte išta kliknuo
                if (selektovanPosetilac != null)
                {
                    // Potvrda brisanja (opciono, ali preporučljivo)
                    var result = MessageBox.Show("Da li ste sigurni?", "Potvrda", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // A) Brišemo iz memorijske liste (ObservableCollection) 
                        // Ovo odmah sklanja red sa ekrana!
                        Posetioci.Remove(selektovanPosetilac);

                        // B) Brišemo iz fajla preko DAO
                        posetilacDao.Remove(selektovanPosetilac);
                    }
                }
                else
                {
                    MessageBox.Show("Prvo selektujte red u tabeli koji želite da obrišete!");
                }
            }
        }

        private void BtnIzmeni_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void DataGridPosetioci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}