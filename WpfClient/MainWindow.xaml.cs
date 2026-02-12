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
            MessageBox.Show("Sajam Knjiga v1.0\n\nAplikacija za kupovinu knjiga u okviru Sajma Knjiga.\n\nAutori: \n- - - - - - - - - - - - -\n[Milos Trisic] - RA 39/2023\n[Boris Stepanovic] - RA 97/2023\n");
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
                    int nextId = posetilacDao.GetAll().Count + 1;  // broj novog posetioca
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

                IzmenaPosetioca prozor = new IzmenaPosetioca(selektovan);
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

            private void FiltrirajPosetioce(string filter)
            {

                if (PosetiociView == null) return;

                if (string.IsNullOrWhiteSpace(filter))
                {
                    PosetiociView.Filter = null; // Poništi filter ako je polje prazno
                }
                else
                {
                    PosetiociView.Filter = obj =>
                    {
                        var p = obj as Posetilac;
                        if (p == null) return false;

                        // 1. Priprema filtera (sve u mala slova i brišemo razmake sa krajeva)
                        string f = txtPretraga.Text.ToLower().Trim();

                        if (string.IsNullOrEmpty(f)) return true;

                        // 2. Provera imena i prezimena (ToLower obezbeđuje da "V" nađe i "v" i "V")
                        bool matchImePrezime = p.Ime.ToLower().Contains(filter) ||
                               p.Prezime.ToLower().Contains(filter) ||
                               p.Adresa.Ulica.ToLower().Contains(filter) ||
                               p.Adresa.Broj.ToLower().Contains(filter) ||
                               p.Adresa.Grad.ToLower().Contains(filter) ||
                               p.Adresa.Drzava.ToLower().Contains(filter) ||

                               p.BrClanskeKarte.ToLower().Contains(filter);

                        // 3. Provera Statusa
                        // Proveravamo da li je unos "v" i da li je status V
                        // ILI da li string reprezentacija enuma sadrži filter
                        bool matchStatus = false;
                        if (f == "v")
                        {
                            matchStatus = (p.Status == StatusPosetioca.V);
                        }
                        else if (f == "r")
                        {
                            matchStatus = (p.Status == StatusPosetioca.R);
                        }
                        else
                        {
                            // Ovo pokriva slučaj ako neko ukuca "Redovan" ili "V.I.P."
                            matchStatus = p.Status.ToString().ToLower().Contains(f);
                        }

                        // 4. Rezultat: Prikaži ako se poklapa bilo šta od navedenog
                        return matchImePrezime || matchStatus;

                  
                    };
                }
                PosetiociView.Refresh(); // Osveži prikaz u DataGrid-u
            }

        private void FiltrirajAutore(string filter)
        {
            if (AutoriView == null) return;

            if (string.IsNullOrWhiteSpace(filter))
            {
                AutoriView.Filter = null;
            }
            else
            {
                string f = filter.ToLower().Trim();

                AutoriView.Filter = obj =>
                {
                    var a = obj as Autor;
                    if (a == null) return false;

                    // 1. Provera Imena i Prezimena
                    bool matchImePrezime = a.Ime.ToLower().Contains(f) ||
                                           a.Prezime.ToLower().Contains(f);

                    // 2. Provera Broja lične karte (string)
                    bool matchLk = a.Broj_lk.ToLower().Contains(f);

                    // 3. Provera E-maila (string)
                    bool matchEmail = a.Email.ToLower().Contains(f);

                    // 4. Provera Datuma rođenja (DateTime)
                    // Pretvaramo datum u string formata dd.MM.yyyy da bi korisnik mogao 
                    // da kuca npr. "1985" ili "05.10" i nađe autora
                    bool matchDatum = a.Datum_rodjenja.ToString("dd.MM.yyyy").Contains(f);

                    // Vraća true ako bilo koji uslov ispunjava kriterijum
                    return matchImePrezime || matchLk || matchEmail || matchDatum;
                };
            }
            AutoriView.Refresh();
        }

        private void FiltrirajKnjige(string filter)
        {
            if (KnjigeView == null) return;

            if (string.IsNullOrWhiteSpace(filter))
            {
                KnjigeView.Filter = null;
            }
            else
            {
                string f = filter.ToLower().Trim();

                KnjigeView.Filter = obj =>
                {
                    var k = obj as Knjiga;
                    if (k == null) return false;

                    // 1. Provera ISBN-a i Naziva
                    bool matchOsnovno = k.ISBN.ToLower().Contains(f) ||
                                        k.Naziv.ToLower().Contains(f);

                    // 2. Provera Godine izdanja i Cene (pretvaramo u string za pretragu)
                    bool matchTehnicki = k.Godina_izdanja.ToString().Contains(f) ||
                                         k.Cena.ToString().Contains(f);

                    // 3. Provera Žanra (Enum)
                    // Replace('_', ' ') omogućava da korisnik kuca "Naučna fantastika" 
                    // iako je u Enumu "Naučna_fantastika"
                    string zanrString = k.Zanr.ToString().ToLower().Replace('_', ' ');
                    bool matchZanr = zanrString.Contains(f);

                    return matchOsnovno || matchTehnicki || matchZanr;
                };
            }
            KnjigeView.Refresh();
        }

        private void MenuIzdavaci_Click(object sender, RoutedEventArgs e)
        {
            // 1. Inicijalizacija
            IzdavaciProzor popUp = new IzdavaciProzor();

            // 2. Postavljanje vlasnika (da pop-up bude centriran preko glavnog prozora)
            popUp.Owner = this;

            // 3. Povezivanje podataka (ovde pozivaš svoju listu izdavača)
            // popUp.dgIzdavaci.ItemsSource = tvojDAO.GetAll();

            // 4. Prikaz (ShowDialog znači da ne možeš kliknuti nazad dok ne zatvoriš pop-up)
            popUp.ShowDialog();
        }
    }
}
