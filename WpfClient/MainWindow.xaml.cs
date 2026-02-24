using Core;
using Core.DAO;
using Core.Storage.Serialization;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;

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

        public ObservableCollection<Izdavac> Izdavaci { get; set; }


        public ICollectionView PosetiociView { get; set; }
        public ICollectionView AutoriView { get; set; }
        public ICollectionView KnjigeView { get; set; }

        KnjigaDAO knjigaDao = new KnjigaDAO();
        AutorDAO autorDao = new AutorDAO();
        AdresaDAO adresaDao = new AdresaDAO();
        IzdavacDAO izdavacDao = new IzdavacDAO();
        PosetilacDAO posetilacDao = new PosetilacDAO();
        public MainWindow()
        {

            InitializeComponent(); // Prvo inicijalizuj komponente

            // Podešavanje tajmera za sat
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            List<Posetilac> sviPosetioci = posetilacDao.GetAll();
            List<Autor> sviAutori = autorDao.GetAll();
            List<Izdavac> sviIzdavaci = izdavacDao.GetAll();
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
            Izdavaci = new ObservableCollection<Izdavac>(sviIzdavaci);

            // Kreiramo poglede na osnovu tvojih kolekcija
            PosetiociView = CollectionViewSource.GetDefaultView(Posetioci);
            AutoriView = CollectionViewSource.GetDefaultView(Autori);
            KnjigeView = CollectionViewSource.GetDefaultView(Knjige);

            // Povezujemo DataGrid sa View-om umesto direktno sa kolekcijom
            DataGridPosetioci.ItemsSource = PosetiociView;
            // Ako imaš i ostale DataGrid-ove:
            // DataGridAutori.ItemsSource = AutoriView;
            // DataGridKnjige.ItemsSource = KnjigeView;


            // 5. Poveži sa DataGrid-om
            DataGridPosetioci.ItemsSource = Posetioci;
            this.DataContext = this;
        }

        // Prebacivanje na tab Posetioci
        private void MenuPosetioci_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
        }

        // Prebacivanje na tab Autori
        private void MenuAutori_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        // Prebacivanje na tab Knjige
        private void MenuKnjige_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2;
        }

        // Otvaranje novog prozora za Izdavače
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Provera da li je pritisnut Control taster
            bool isCtrlDown = Keyboard.Modifiers == ModifierKeys.Control;

            // Ctrl + N za New
            if (isCtrlDown && e.Key == Key.N)
            {
                BtnDodaj_Click(sender, null);
                e.Handled = true; // Sprečava dalje širenje događaja
            }

            // Ctrl + S za Save
            if (isCtrlDown && e.Key == Key.S)
            {
                BtnSave_Click(sender, null);
                e.Handled = true;
            }

            // Escape za izlaz (opciono, ali korisno)
            if (e.Key == Key.Escape)
            {
                MenuClose_Click(sender, null);
                e.Handled = true;
            }
        }

        // Help - About prozor
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sajam Knjiga v1.0\n\nAplikacija za kupovinu knjiga u okviru Sajma Knjiga.\n\nAutori: \n- - - - - - - - - - - - -\n[Miloš Trišić] - RA 39/2023\n[Boris Stepanović] - RA 97/2023\n");
        }

        // Zatvaranje aplikacije
        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Save (za sada samo potvrda)
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. ČUVANJE POSETILACA
                posetilacDao.SaveAll(new List<Posetilac>(Posetioci));

                // 2. ČUVANJE AUTORA
                autorDao.SaveAll(new List<Autor>(Autori));

                // 3. ČUVANJE KNJIGA
                knjigaDao.SaveAll(new List<Knjiga>(Knjige));

                izdavacDao.SaveAll(new List<Izdavac>());


                // 4. ČUVANJE SVIH ADRESA (Skupljamo adrese od svih)
                List<Adresa> sveAdrese = new List<Adresa>();
                foreach (var p in Posetioci) if (p.Adresa != null) sveAdrese.Add(p.Adresa);
                foreach (var a in Autori) if (a.Adresa != null) sveAdrese.Add(a.Adresa);

                adresaDao.SaveAll(sveAdrese);

                MessageBox.Show("Sve izmene su uspešno sačuvane!", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške pri čuvanju: {ex.Message}");
            }
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
                    int nextId = Posetioci.Count + 1;
                    string brClanskeKarte = $"CK-{nextId}";

                    // 1. Uzimamo kreiranog posetioca iz dijaloga
                    Posetilac p = dijaloškiProzor.NoviPosetilac;
                    Adresa d = dijaloškiProzor.NoviPosetilac.Adresa;
                    dijaloškiProzor.NoviPosetilac.Adresa.VlasnikID = brClanskeKarte;

                    p.BrClanskeKarte = brClanskeKarte;

                    if (p != null)
                    {
                        // 2. Dodajemo ga u DAO (da se upiše u fajl)
                        //posetilacDao.Add(p);
                        //adresaDao.Add(d);

                        // 3. Dodajemo ga u ObservableCollection (da se odmah pojavi u Gridu)
                        Posetioci.Add(p);
                    }
                }
            }
            if (MainTabControl.SelectedIndex == 1)
            {
                // Kreiramo instancu novog prozora
                DodajAutoraProzor dijaloškiProzor = new DodajAutoraProzor();

                // Postavljamo MainWindow kao vlasnika (da bi iskočio tačno ispred njega)
                dijaloškiProzor.Owner = this;


                // ShowDialog() zaustavlja rad u MainWindow dok se ovaj prozor ne zatvori
                if (dijaloškiProzor.ShowDialog() == true)
                {
                   
                    // 1. Uzimamo kreiranog posetioca iz dijaloga
                    Autor a = dijaloškiProzor.NoviAutor;
                    Adresa d = dijaloškiProzor.NoviAutor.Adresa;
                    dijaloškiProzor.NoviAutor.Adresa.VlasnikID=dijaloškiProzor.NoviAutor.Broj_lk;

                    

                    if (a != null)
                    {
                        // 2. Dodajemo ga u DAO (da se upiše u fajl)
                        //autorDao.Add(a);
                        //adresaDao.Add(d);

                        // 3. Dodajemo ga u ObservableCollection (da se odmah pojavi u Gridu)
                        Autori.Add(a);
                    }
                }
            }
            if (MainTabControl.SelectedIndex == 2)
            {
          
                DodajKnjiguProzor dijaloškiProzor = new DodajKnjiguProzor();

                dijaloškiProzor.Owner = this;

                if (dijaloškiProzor.ShowDialog() == true)
                {
                    Knjiga a = dijaloškiProzor.NovaKnjiga;
              
                    if (a != null)
                    {
                        //knjigaDao.Add(a);
                        Knjige.Add(a);
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
                Knjiga selektovanaKnjiga = (Knjiga)DataGridKnjige.SelectedItem;

                // 3. Proveravamo da li je korisnik uopšte išta kliknuo
                if (selektovanaKnjiga != null)
                {
                    // Potvrda brisanja (opciono, ali preporučljivo)
                    var result = MessageBox.Show("Da li ste sigurni da želite da obrišete knjigu?", "Brisanje knjige", MessageBoxButton.YesNo);

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

                Autor selektovanAutor = (Autor)DataGridAutori.SelectedItem;

                // 3. Proveravamo da li je korisnik uopšte išta kliknuo
                if (selektovanAutor != null)
                {
                    // Potvrda brisanja (opciono, ali preporučljivo)
                    var result = MessageBox.Show("Da li ste sigurni da želite da obrišete autora?", "Brisanje autora", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // A) Brišemo iz memorijske liste (ObservableCollection) 
                        // Ovo odmah sklanja red sa ekrana!
                        Autori.Remove(selektovanAutor);

                        // B) Brišemo iz fajla preko DAO
                       // adresaDao.Remove(selektovanAutor.Broj_lk);
                       // autorDao.Remove(selektovanAutor);
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
                    var result = MessageBox.Show("Da li ste sigurni da želite da obrišete posetioca?", "Brisanje posetioca", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // A) Brišemo iz memorijske liste (ObservableCollection) 
                        // Ovo odmah sklanja red sa ekrana!
                        Posetioci.Remove(selektovanPosetilac);

                        // B) Brišemo iz fajla preko DAO
                        adresaDao.Remove(selektovanPosetilac.BrClanskeKarte);
                        //posetilacDao.Remove(selektovanPosetilac);
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
            int aktivniTab = MainTabControl.SelectedIndex;

            if (aktivniTab == 0) // POSJETIOCI
            {
                // Uzmi šta je trenutno selektovano
                var selektovan = DataGridPosetioci.SelectedItem as Posetilac;

                // KLJUČNA PROVERA: Ako je null ili ako korisnik nije zapravo kliknuo na red
                if (selektovan == null)
                {
                    MessageBox.Show("Morate prvo selektovati posetioca u tabeli!", "Obaveštenje");
                    return;
                }

                IzmenaPosetioca prozor = new IzmenaPosetioca(selektovan, new List<Knjiga>(Knjige));
                prozor.Owner = this;
                if (prozor.ShowDialog() == true) { PosetiociView.Refresh(); }

                // OPCIONO: Odselektuj nakon zatvaranja prozora da "isprazniš" bafer
                DataGridPosetioci.SelectedItem = null;
            }
            else if (aktivniTab == 1) // AUTORI
            {
                var selektovan = DataGridAutori.SelectedItem as Autor;

                if (selektovan == null)
                {
                    MessageBox.Show("Morate prvo selektovati autora u tabeli!", "Obaveštenje");
                    return;
                }

                IzmenaAutora prozor = new IzmenaAutora(selektovan);
                prozor.Owner = this;
                if (prozor.ShowDialog() == true) { AutoriView.Refresh(); }

                DataGridAutori.SelectedItem = null;
            }
            else if (aktivniTab == 2) // KNJIGE
            {
                var selektovan = DataGridKnjige.SelectedItem as Knjiga;

                if (selektovan == null)
                {
                    MessageBox.Show("Morate prvo selektovati knjigu u tabeli!", "Obaveštenje");
                    return;
                }

                DodajKnjiguProzor prozor = new DodajKnjiguProzor(selektovan);
                prozor.Owner = this;
                if (prozor.ShowDialog() == true) { KnjigeView.Refresh(); }

                DataGridKnjige.SelectedItem = null;
            }
        }

        private void DataGridPosetioci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void timer_Tick(object? sender, EventArgs e)
        {
            // Uzimamo trenutno vreme i datum
            DateTime now = DateTime.Now;

            // Vreme format (npr. 22:45:10)
            lblTime.Text = now.ToString("HH:mm:ss");

            // Datum format (npr. 09.02.2026.)
            lblDate.Text = now.ToString("dd.MM.yyyy.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filter = txtPretraga.Text.ToLower();

            // Proveravamo po indexu koji je tab trenutno otvoren
            if (MainTabControl.SelectedIndex == 0) // Posetioci
            {
                FiltrirajPosetioce(filter);
            }
            else if (MainTabControl.SelectedIndex == 1) // Autori
            {
                FiltrirajAutore(filter);
            }
            else if (MainTabControl.SelectedIndex == 2) // Knjige
            {
                FiltrirajKnjige(filter);
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
        }

            private void FiltrirajPosetioce(string upit)
            {

            if (PosetiociView == null) return;

            if (string.IsNullOrWhiteSpace(upit))
            {
                PosetiociView.Filter = null;
            }
            else
            {
                // 1. Parsiranje: Razdvajamo upit po zarezu i čistimo razmake
                // Primer: "Petrovic, Petar" -> ["petrovic", "petar"]
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                PosetiociView.Filter = obj =>
                {
                    var p = obj as Posetilac;
                    if (p == null) return false;

                    // Uzimamo podatke posetioca u malim slovima radi case-insensitive poređenja
                    string prezime = p.Prezime.ToLower();
                    string ime = p.Ime.ToLower();
                    string karta = p.BrClanskeKarte.ToLower();

                    // Pravila na osnovu broja reči:
                    if (delovi.Length == 1)
                    {
                        // JEDNA REČ -> Preзиме sadrži reč
                        return prezime.Contains(delovi[0]);
                    }
                    else if (delovi.Length == 2)
                    {
                        // DVE REČI -> Prva u prezimenu, druga u imenu
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                    }
                    else if (delovi.Length >= 3)
                    {
                        // TRI REČI -> Prva u broju karte, druga u imenu, treća u prezimenu
                        return karta.Contains(delovi[0]) &&
                               ime.Contains(delovi[1]) &&
                               prezime.Contains(delovi[2]);
                    }

                    return false;
                };
            }
            PosetiociView.Refresh(); // Osveži prikaz u DataGrid-u
            }

        private void FiltrirajAutore(string upit)
        {
            if (AutoriView == null) return;

            if (string.IsNullOrWhiteSpace(upit))
            {
                AutoriView.Filter = null;
            }
            else
            {
                // Razdvajamo po zarezu i brišemo razmake
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                AutoriView.Filter = obj =>
                {
                    var a = obj as Autor;
                    if (a == null) return false;

                    string prezime = a.Prezime.ToLower();
                    string ime = a.Ime.ToLower();

                    if (delovi.Length == 1)
                    {
                        // JEDNA REČ -> Preзиме садржи реч
                        return prezime.Contains(delovi[0]);
                    }
                    else if (delovi.Length >= 2)
                    {
                        // DVE REČI -> Prva u prezimenu, druga u imenu
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                    }

                    return false;
                };
            }
            AutoriView.Refresh();
        }

        private void FiltrirajKnjige(string upit)
        {
            if (KnjigeView == null) return;

            if (string.IsNullOrWhiteSpace(upit))
            {
                KnjigeView.Filter = null;
            }
            else
            {
                string f = upit.ToLower().Trim();

                KnjigeView.Filter = obj =>
                {
                    var k = obj as Knjiga;
                    if (k == null) return false;

                    // Pretraga: Deo naziva ILI deo ISBN broja
                    return k.Naziv.ToLower().Contains(f) ||
                           k.ISBN.ToLower().Contains(f);
                };
            }
            KnjigeView.Refresh();
        }

        private void MenuIzdavaci_Click(object sender, RoutedEventArgs e)
        {
            // Prosledi listu izdavača prozoru (pretpostavka da si napravio konstruktor koji prima listu)
            IzdavaciProzor popUp = new IzdavaciProzor();
            popUp.Owner = this;
            popUp.ShowDialog();
            // Kada se zatvori ovaj prozor, NIŠTA nije upisano u fajl još uvek
        }
    }
}
