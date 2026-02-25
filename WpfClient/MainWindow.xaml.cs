using Core;
using Core.DAO;
using Core.Storage;
using Core.Storage.Serialization;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    public partial class MainWindow : Window
    {
        // ── ObservableCollection-e prikaz u DataGrid-u ──────────────────
        public ObservableCollection<Posetilac> Posetioci { get; set; }
        public ObservableCollection<Autor> Autori { get; set; }
        public ObservableCollection<Knjiga> Knjige { get; set; }
        public ObservableCollection<Izdavac> Izdavaci { get; set; }

        // ── Views za filtriranje/sortiranje ─────────────────────────────
        public ICollectionView PosetiociView { get; set; }
        public ICollectionView AutoriView { get; set; }
        public ICollectionView KnjigeView { get; set; }

        // ── DAO sloj ────────────────────────────────────────────────────
        private readonly KnjigaDAO knjigaDao = new KnjigaDAO();
        private readonly AutorDAO autorDao = new AutorDAO();
        private readonly AdresaDAO adresaDao = new AdresaDAO();
        private readonly IzdavacDAO izdavacDao = new IzdavacDAO();
        private readonly PosetilacDAO posetilacDao = new PosetilacDAO();
        private readonly KupiliDAO kupiliDao = new KupiliDAO();

        // ── Master liste — sadrže SVE entitete (osnova za paginaciju) ───
        // VAŽNO: ovo su ISTE instance koje DataBinding poveže međusobno.
        // Nikad ih ne zamenjuj novim listama — samo dodaj/ukloni elemente.
        private List<Posetilac> sviPosetiociList;
        private List<Autor> sviAutoriList;
        private List<Knjiga> sveKnjigeList;
        private List<Izdavac> sviIzdavaciList;
        private List<Kupovina> sveKupovineList;

        // ── Paginacija ──────────────────────────────────────────────────
        private int trenutnaStranica = 1;
        private const int velicinaStranice = 16;

        // ================================================================
        // Konstruktor
        // ================================================================
        public MainWindow()
        {
            InitializeComponent();

            // ----------------------------------------------------------
            // KORAK 1: Učitaj sve entitete iz fajlova (samo jednom!)
            // ----------------------------------------------------------
            sviPosetiociList = posetilacDao.GetAll();
            sviAutoriList = autorDao.GetAll();
            sveKnjigeList = knjigaDao.GetAll();
            sviIzdavaciList = izdavacDao.GetAll();             
            sveKupovineList = kupiliDao.GetAll();

            // ----------------------------------------------------------
            // KORAK 2: Poveži adrese (poseban fajl, ID je ključ veze)
            // ----------------------------------------------------------
            foreach (var p in sviPosetiociList)
                p.Adresa = adresaDao.GetByVlasnikID(p.BrClanskeKarte);

            foreach (var a in sviAutoriList)
                a.Adresa = adresaDao.GetByVlasnikID(a.Broj_lk);

            // ----------------------------------------------------------
            // KORAK 3: DataBinding
            // ----------------------------------------------------------
            DataBinding.PoveziSve(
                sviPosetiociList,
                sveKnjigeList,
                sviAutoriList,
                sviIzdavaciList,
                sveKupovineList);

            // ----------------------------------------------------------
            // KORAK 4: Napravi ObservableCollection-e i ICollectionView-e
            // ObservableCollection počinje prazna — OsveziPrikaz() je puni
            // ----------------------------------------------------------
            Posetioci = new ObservableCollection<Posetilac>();
            Autori = new ObservableCollection<Autor>();
            Knjige = new ObservableCollection<Knjiga>();
            Izdavaci = new ObservableCollection<Izdavac>(sviIzdavaciList);

            PosetiociView = CollectionViewSource.GetDefaultView(Posetioci);
            AutoriView = CollectionViewSource.GetDefaultView(Autori);
            KnjigeView = CollectionViewSource.GetDefaultView(Knjige);

            // ----------------------------------------------------------
            // KORAK 5: Poveži DataGrid-ove sa View-ovima
            // ----------------------------------------------------------
            DataGridPosetioci.ItemsSource = PosetiociView;
            DataGridAutori.ItemsSource = AutoriView;
            DataGridKnjige.ItemsSource = KnjigeView;

            // ----------------------------------------------------------
            // KORAK 6: Popuni prvi prikaz (stranica 1)
            // ----------------------------------------------------------
            OsveziPrikaz();

            this.DataContext = this;

            // ----------------------------------------------------------
            // Tajmer za sat u status baru
            // ----------------------------------------------------------
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        // ================================================================
        // Paginacija
        // ================================================================

        /// <summary>
        /// Puni ObservableCollection-e podacima za trenutnu stranicu.
        /// Radi sa master listama — DataBinding veze su već uspostavljene.
        /// </summary>
        private void OsveziPrikaz()
        {
            int preskoci = (trenutnaStranica - 1) * velicinaStranice;

            // Posetioci
            Posetioci.Clear();
            foreach (var p in sviPosetiociList.Skip(preskoci).Take(velicinaStranice))
                Posetioci.Add(p);
            if (txtStranaInfo != null)
                txtStranaInfo.Text = $"Stranica {trenutnaStranica}";

            // Autori
            Autori.Clear();
            foreach (var a in sviAutoriList.Skip(preskoci).Take(velicinaStranice))
                Autori.Add(a);
            if (txtStranaInfoAutori != null)
                txtStranaInfoAutori.Text = $"Stranica {trenutnaStranica}";

            // Knjige
            Knjige.Clear();
            foreach (var k in sveKnjigeList.Skip(preskoci).Take(velicinaStranice))
                Knjige.Add(k);
            if (txtStranaInfoKnjige != null)
                txtStranaInfoKnjige.Text = $"Stranica {trenutnaStranica}";
        }

        private void btnPrethodna_Click(object sender, RoutedEventArgs e)
        {
            if (trenutnaStranica > 1)
            {
                trenutnaStranica--;
                OsveziPrikaz();
            }
        }

        private void btnSledeca_Click(object sender, RoutedEventArgs e)
        {
            // Koristi najveću od tri liste kao osnov za max stranicu
            int maxBroj = Math.Max(sviPosetiociList.Count,
                          Math.Max(sviAutoriList.Count, sveKnjigeList.Count));

            if (trenutnaStranica * velicinaStranice < maxBroj)
            {
                trenutnaStranica++;
                OsveziPrikaz();
            }
        }

        // ================================================================
        // Save — snima SVE entitete u fajlove
        // ================================================================
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Snimamo master liste (ne ObservableCollection koje mogu biti
                // nepotpune zbog paginacije!)
                posetilacDao.SaveAll(sviPosetiociList);
                autorDao.SaveAll(sviAutoriList);
                knjigaDao.SaveAll(sveKnjigeList);

                // Izdavači — snimamo pravu listu, ne praznu!
                izdavacDao.SaveAll(Izdavaci.ToList());

                // Kupovine
                kupiliDao.Save(); // interno drži listu i snima je

                // Adrese — skupi od svih posetilaca i autora
                var sveAdrese = new List<Adresa>();
                foreach (var p in sviPosetiociList)
                    if (p.Adresa != null) sveAdrese.Add(p.Adresa);
                foreach (var a in sviAutoriList)
                    if (a.Adresa != null) sveAdrese.Add(a.Adresa);
                adresaDao.SaveAll(sveAdrese);

                MessageBox.Show("Sve izmene su uspešno sačuvane!",
                    "Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ================================================================
        // Dodavanje entiteta
        // ================================================================
        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedIndex == 0) // Posetioci
            {
                var dijalog = new DodajPosetiocaProzor();
                dijalog.Owner = this;

                if (dijalog.ShowDialog() == true)
                {
                    Posetilac p = dijalog.NoviPosetilac;

                    // Generiši sledeći ID na osnovu ukupnog broja u master listi
                    int maxId = sviPosetiociList
                        .Select(x => {
                            var deo = x.BrClanskeKarte?.Replace("CK-", "");
                            return int.TryParse(deo, out int n) ? n : 0;
                        }).DefaultIfEmpty(0).Max();

                    int nextId = maxId + 1;
                    p.BrClanskeKarte = $"CK-{nextId}";

                    if (p.Adresa != null)
                        p.Adresa.VlasnikID = p.BrClanskeKarte;


                    // 
                    posetilacDao.Add(p);
                    
                    if (p.Adresa != null)
                        adresaDao.Add(p.Adresa);

                    OsveziPrikaz();
                }
            }
            else if (MainTabControl.SelectedIndex == 1) // Autori
            {
                var dijalog = new DodajAutoraProzor();
                dijalog.Owner = this;

                if (dijalog.ShowDialog() == true)
                {
                    Autor a = dijalog.NoviAutor;

                    if (a.Adresa != null)
                        a.Adresa.VlasnikID = a.Broj_lk;


                    autorDao.Add(a);
                    if (a.Adresa != null)
                        adresaDao.Add(a.Adresa);

                    OsveziPrikaz();
                }
            }
            else if (MainTabControl.SelectedIndex == 2) // Knjige
            {
                var dijalog = new DodajKnjiguProzor();
                dijalog.Owner = this;

                if (dijalog.ShowDialog() == true)
                {
                    Knjiga k = dijalog.NovaKnjiga;
                    knjigaDao.Add(k);

                    // Uspostavi bidirekcione veze
                    if (k.ListaAutora != null)
                        foreach (var autor in k.ListaAutora)
                            autor.DodajSpisakKnjiga(k);

                    if (k.Izdavac != null)
                        k.Izdavac.DodajKnjigu(k);

                    OsveziPrikaz();
                }
            }
        }

        // ================================================================
        // Brisanje entiteta
        // ================================================================
        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            int aktivniTab = MainTabControl.SelectedIndex;

            if (aktivniTab == 0) // Posetioci
            {
                var selektovan = DataGridPosetioci.SelectedItem as Posetilac;
                if (selektovan == null)
                {
                    MessageBox.Show("Prvo selektujte posetioca u tabeli!");
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete posetioca {selektovan.Ime} {selektovan.Prezime}?",
                    "Brisanje posetioca", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    sviPosetiociList.Remove(selektovan);
                    posetilacDao.Remove(selektovan);
                    if (selektovan.Adresa != null)
                        adresaDao.Remove(selektovan.BrClanskeKarte);
                    OsveziPrikaz();
                }
            }
            else if (aktivniTab == 1) // Autori
            {
                var selektovan = DataGridAutori.SelectedItem as Autor;
                if (selektovan == null)
                {
                    MessageBox.Show("Prvo selektujte autora u tabeli!");
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete autora {selektovan.Ime} {selektovan.Prezime}?",
                    "Brisanje autora", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    sviAutoriList.Remove(selektovan);
                    autorDao.Remove(selektovan);
                    if (selektovan.Adresa != null)
                        adresaDao.Remove(selektovan.Broj_lk);
                    OsveziPrikaz();
                }
            }
            else if (aktivniTab == 2) // Knjige
            {
                var selektovana = DataGridKnjige.SelectedItem as Knjiga;
                if (selektovana == null)
                {
                    MessageBox.Show("Prvo selektujte knjigu u tabeli!");
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete knjigu \"{selektovana.Naziv}\"?",
                    "Brisanje knjige", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    sveKnjigeList.Remove(selektovana);
                    knjigaDao.Remove(selektovana);
                    OsveziPrikaz();
                }
            }
        }

        // ================================================================
        // Izmena entiteta
        // ================================================================
        private void BtnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            int aktivniTab = MainTabControl.SelectedIndex;

            if (aktivniTab == 0) // Posetioci
            {
                var selektovan = DataGridPosetioci.SelectedItem as Posetilac;
                if (selektovan == null)
                {
                    MessageBox.Show("Morate prvo selektovati posetioca u tabeli!", "Obaveštenje");
                    return;
                }

                // Prosleđujemo sve knjige da IzmenaPosetioca može da ponudi listu za zelje
                var prozor = new IzmenaPosetioca(selektovan, sveKnjigeList, kupiliDao);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    // Snimi izmene adrese
                    if (selektovan.Adresa != null)
                        adresaDao.Update(selektovan.Adresa);

                    // Snimi izmene posetioca
                    posetilacDao.Update(selektovan);

                    PosetiociView.Refresh();
                }

                DataGridPosetioci.SelectedItem = null;
            }
            else if (aktivniTab == 1) // Autori
            {
                var selektovan = DataGridAutori.SelectedItem as Autor;
                if (selektovan == null)
                {
                    MessageBox.Show("Morate prvo selektovati autora u tabeli!", "Obaveštenje");
                    return;
                }

                var prozor = new IzmenaAutora(selektovan, sveKnjigeList, autorDao, knjigaDao);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    if (selektovan.Adresa != null)
                        adresaDao.Update(selektovan.Adresa);

                    autorDao.Update(selektovan);
                    AutoriView.Refresh();
                }

                DataGridAutori.SelectedItem = null;
            }
            else if (aktivniTab == 2) // Knjige
            {
                var selektovana = DataGridKnjige.SelectedItem as Knjiga;
                if (selektovana == null)
                {
                    MessageBox.Show("Morate prvo selektovati knjigu u tabeli!", "Obaveštenje");
                    return;
                }

                // Prosleđujemo sve autore za izbor autora knjige
                var prozor = new DodajKnjiguProzor(selektovana);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    knjigaDao.Save();
                    KnjigeView.Refresh();
                }

                DataGridKnjige.SelectedItem = null;
            }
        }

        // ================================================================
        // Pretraga
        // ================================================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filter = txtPretraga.Text.ToLower();

            if (MainTabControl.SelectedIndex == 0)
                FiltrirajPosetioce(filter);
            else if (MainTabControl.SelectedIndex == 1)
                FiltrirajAutore(filter);
            else if (MainTabControl.SelectedIndex == 2)
                FiltrirajKnjige(filter);
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
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                PosetiociView.Filter = obj =>
                {
                    var p = obj as Posetilac;
                    if (p == null) return false;

                    string prezime = p.Prezime?.ToLower() ?? "";
                    string ime = p.Ime?.ToLower() ?? "";
                    string karta = p.BrClanskeKarte?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else if (delovi.Length == 2)
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                    else if (delovi.Length >= 3)
                        return karta.Contains(delovi[0]) && ime.Contains(delovi[1]) && prezime.Contains(delovi[2]);

                    return false;
                };
            }

            PosetiociView.Refresh();
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
                string[] delovi = upit.ToLower().Split(',');
                for (int i = 0; i < delovi.Length; i++) delovi[i] = delovi[i].Trim();

                AutoriView.Filter = obj =>
                {
                    var a = obj as Autor;
                    if (a == null) return false;

                    string prezime = a.Prezime?.ToLower() ?? "";
                    string ime = a.Ime?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else if (delovi.Length >= 2)
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);

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
                    return (k.Naziv?.ToLower().Contains(f) == true) ||
                           (k.ISBN?.ToLower().Contains(f) == true);
                };
            }

            KnjigeView.Refresh();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        // ================================================================
        // Menu Bar handlers
        // ================================================================
        private void MenuPosetioci_Click(object sender, RoutedEventArgs e) =>
            MainTabControl.SelectedIndex = 0;

        private void MenuAutori_Click(object sender, RoutedEventArgs e) =>
            MainTabControl.SelectedIndex = 1;

        private void MenuKnjige_Click(object sender, RoutedEventArgs e) =>
            MainTabControl.SelectedIndex = 2;

        private void MenuIzdavaci_Click(object sender, RoutedEventArgs e)
        {
            // Prosleđujemo referencu na listu koju MainWindow već ima
            IzdavaciProzor popUp = new IzdavaciProzor(this.Izdavaci);
            popUp.Owner = this;
            popUp.ShowDialog();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Sajam Knjiga v1.0\n\n" +
                "Aplikacija za kupovinu knjiga u okviru Sajma Knjiga.\n\n" +
                "Autori:\n" +
                "- - - - - - - - - - - - -\n" +
                "[Miloš Trišić] - RA 39/2023\n" +
                "[Boris Stepanović] - RA 97/2023\n",
                "O aplikaciji");
        }

        private void MenuClose_Click(object sender, RoutedEventArgs e) =>
            this.Close();

        // ================================================================
        // Keyboard shortcuts
        // ================================================================
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool isCtrl = Keyboard.Modifiers == ModifierKeys.Control;

            if (isCtrl && e.Key == Key.N)
            {
                BtnDodaj_Click(sender, null);
                e.Handled = true;
            }
            else if (isCtrl && e.Key == Key.S)
            {
                BtnSave_Click(sender, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                MenuClose_Click(sender, null);
                e.Handled = true;
            }
        }

        // ================================================================
        // Ostalo
        // ================================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = SystemParameters.PrimaryScreenWidth * 0.75;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.75;
        }

        private void DataGridPosetioci_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void timer_Tick(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            lblTime.Text = now.ToString("HH:mm:ss");
            lblDate.Text = now.ToString("dd.MM.yyyy.");
        }
    }
}