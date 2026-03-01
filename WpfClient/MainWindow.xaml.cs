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
        private readonly ZeljaDAO zeljaDao = new ZeljaDAO();

        // ── Master liste — sadrže SVE entitete (osnova za paginaciju) ───
        // VAŽNO: ovo su ISTE instance koje DataBinding poveže međusobno.
        // Nikad ih ne zamenjuj novim listama — samo dodaj/ukloni elemente.
        private List<Posetilac> sviPosetiociList;
        private List<Autor> sviAutoriList;
        private List<Knjiga> sveKnjigeList;
        private List<Izdavac> sviIzdavaciList;
        private List<Kupovina> sveKupovineList;
        private List<Zelja> sveZeljeList;

        // ── Paginacija ──────────────────────────────────────────────────
        private int stranicaPosetioci = 1;
        private int stranicaAutori = 1;
        private int stranicaKnjige = 1;
        private const int velicinaStranice = 15;

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
            sveZeljeList = zeljaDao.GetAll();

            sviPosetiociList = sviPosetiociList
            .OrderBy(p =>
            {
                var deo = p.BrClanskeKarte?.Replace("CK-", "");
                return int.TryParse(deo, out int broj) ? broj : int.MaxValue;
            })
            .ToList();



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
                sveKupovineList,
                sveZeljeList);

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
            string recStranica = Application.Current.FindResource("lblStranica").ToString();

            // Posetioci
            int preskociP = (stranicaPosetioci - 1) * velicinaStranice;
            Posetioci.Clear();
            PosetiociView.SortDescriptions.Clear();
            foreach (var p in sviPosetiociList
                .OrderBy(p => {
                    var deo = p.BrClanskeKarte?.Replace("CK-", "");
                    return int.TryParse(deo, out int broj) ? broj : int.MaxValue;
                })
                .Skip(preskociP).Take(velicinaStranice))
                Posetioci.Add(p);

            int maxStrP = (int)Math.Ceiling(sviPosetiociList.Count / (double)velicinaStranice);
            if (txtStranaInfoPosetioci != null)
                txtStranaInfoPosetioci.Text = $"{recStranica} {stranicaPosetioci}/{maxStrP}";

            // Autori
            int preskociA = (stranicaAutori - 1) * velicinaStranice;
            Autori.Clear();
            foreach (var a in sviAutoriList.Skip(preskociA).Take(velicinaStranice))
                Autori.Add(a);

            int maxStrA = (int)Math.Ceiling(sviAutoriList.Count / (double)velicinaStranice);
            if (txtStranaInfoAutori != null)
                txtStranaInfoAutori.Text = $"{recStranica} {stranicaAutori}/{maxStrA}";

            // Knjige
            int preskociK = (stranicaKnjige - 1) * velicinaStranice;
            Knjige.Clear();
            foreach (var k in sveKnjigeList.Skip(preskociK).Take(velicinaStranice))
                Knjige.Add(k);

            int maxStrK = (int)Math.Ceiling(sveKnjigeList.Count / (double)velicinaStranice);
            if (txtStranaInfoKnjige != null)
                txtStranaInfoKnjige.Text = $"{recStranica} {stranicaKnjige}/{maxStrK}";
        }

        private void btnPrethodna_Click(object sender, RoutedEventArgs e)
        {
            int tab = MainTabControl.SelectedIndex;
            if (tab == 0 && stranicaPosetioci > 1) { stranicaPosetioci--; OsveziPrikaz(); }
            else if (tab == 1 && stranicaAutori > 1) { stranicaAutori--; OsveziPrikaz(); }
            else if (tab == 2 && stranicaKnjige > 1) { stranicaKnjige--; OsveziPrikaz(); }
        }

        private void btnSledeca_Click(object sender, RoutedEventArgs e)
        {
            int tab = MainTabControl.SelectedIndex;
            if (tab == 0 && stranicaPosetioci * velicinaStranice < sviPosetiociList.Count)
            { stranicaPosetioci++; OsveziPrikaz(); }
            else if (tab == 1 && stranicaAutori * velicinaStranice < sviAutoriList.Count)
            { stranicaAutori++; OsveziPrikaz(); }
            else if (tab == 2 && stranicaKnjige * velicinaStranice < sveKnjigeList.Count)
            { stranicaKnjige++; OsveziPrikaz(); }
        }

        // ================================================================
        // Save — snima SVE entitete u fajlove
        // ================================================================
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Snimamo master liste
                posetilacDao.SaveAll(sviPosetiociList);
                autorDao.SaveAll(sviAutoriList);
                knjigaDao.SaveAll(sveKnjigeList);
                zeljaDao.Save();

                // Izdavači
                izdavacDao.SaveAll(Izdavaci.ToList());

                // Kupovine
                kupiliDao.Save();

                // Adrese
                var sveAdrese = new List<Adresa>();
                foreach (var p in sviPosetiociList)
                    if (p.Adresa != null) sveAdrese.Add(p.Adresa);
                foreach (var a in sviAutoriList)
                    if (a.Adresa != null) sveAdrese.Add(a.Adresa);
                adresaDao.SaveAll(sveAdrese);

                // --- LOKALIZACIJA PORUKE O USPEHU ---
                string porukaUspeh = Application.Current.FindResource("msgSaveSuccess").ToString();
                string naslovStatus = Application.Current.FindResource("statusTitle").ToString();

                MessageBox.Show(porukaUspeh, naslovStatus, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // --- LOKALIZACIJA PORUKE O GREŠCI ---
                string porukaGreska = Application.Current.FindResource("msgSaveError").ToString();
                string naslovGreska = Application.Current.FindResource("errorTitle").ToString();

                MessageBox.Show($"{porukaGreska} {ex.Message}", naslovGreska, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show(Application.Current.FindResource("msgSelektujPosetioca").ToString());
                    return;
                }

                string pitanje = $"{Application.Current.FindResource("msgConfirmDeletePosetilac")} {selektovan.Ime} {selektovan.Prezime}?";
                string naslov = Application.Current.FindResource("titleDeletePosetilac").ToString();

                var result = MessageBox.Show(pitanje, naslov, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    sviPosetiociList.Remove(selektovan);
                    posetilacDao.Remove(selektovan);
                    zeljaDao.RemoveByPosetilac(selektovan.BrClanskeKarte);
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
                    MessageBox.Show(Application.Current.FindResource("msgSelektujAutora").ToString());
                    return;
                }

                string pitanje = $"{Application.Current.FindResource("msgConfirmDeleteAutor")} {selektovan.Ime} {selektovan.Prezime}?";
                string naslov = Application.Current.FindResource("titleDeleteAutor").ToString();

                var result = MessageBox.Show(pitanje, naslov, MessageBoxButton.YesNo);

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
                    MessageBox.Show(Application.Current.FindResource("msgSelektujKnjigu").ToString());
                    return;
                }

                string pitanje = $"{Application.Current.FindResource("msgConfirmDeleteKnjiga")} \"{selektovana.Naziv}\"?";
                string naslov = Application.Current.FindResource("titleDeleteKnjiga").ToString();

                var result = MessageBox.Show(pitanje, naslov, MessageBoxButton.YesNo);

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
            string naslovObavestenja = Application.Current.FindResource("titleNotice").ToString();

            if (aktivniTab == 0) // Posetioci
            {
                var selektovan = DataGridPosetioci.SelectedItem as Posetilac;
                if (selektovan == null)
                {
                    MessageBox.Show(Application.Current.FindResource("msgSelectVisitorEdit").ToString(), naslovObavestenja);
                    return;
                }

                var prozor = new IzmenaPosetioca(selektovan, sveKnjigeList, kupiliDao, zeljaDao);
                prozor.Owner = this;

                if (prozor.ShowDialog() == true)
                {
                    if (selektovan.Adresa != null)
                        adresaDao.Update(selektovan.Adresa);

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
                    MessageBox.Show(Application.Current.FindResource("msgSelectAuthorEdit").ToString(), naslovObavestenja);
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
                    MessageBox.Show(Application.Current.FindResource("msgSelectBookEdit").ToString(), naslovObavestenja);
                    return;
                }

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


        private void BtnPosetiociAutora_Click(object sender, RoutedEventArgs e)
        {
            // Funkcionalnost dostupna samo na tabu Autori
            if (MainTabControl.SelectedIndex != 1)
            {
                MessageBox.Show(
                    Application.Current.FindResource("msgSelektujAutora").ToString(),
                    Application.Current.FindResource("titleNotice").ToString());
                return;
            }

            var selektovani = DataGridAutori.SelectedItem as Autor;
            if (selektovani == null)
            {
                MessageBox.Show(
                    Application.Current.FindResource("msgSelektujAutora").ToString(),
                    Application.Current.FindResource("titleNotice").ToString());
                return;
            }

            // Filtriramo: posetioci koji na listi zelja imaju barem jednu
            // knjigu izabranog autora (bez duplikata — lista je Vec<Posetilac>)
            var posetioci = sviPosetiociList
                .Where(p => p.ListaZelja
                    .Any(k => k.ListaAutora
                        .Any(a => a.Broj_lk == selektovani.Broj_lk)))
                .ToList();

            var prozor = new Posetiociautoraprozor(posetioci, selektovani.ImePrezime);
            prozor.Owner = this;
            prozor.ShowDialog();
        }


        private void BtnAutoriPosetioca_Click(object sender, RoutedEventArgs e)
        {
            var selektovani = DataGridPosetioci.SelectedItem as Posetilac;
            if (selektovani == null)
            {
                MessageBox.Show(
                    Application.Current.FindResource("msgSelektujPosetioca").ToString(),
                    Application.Current.FindResource("titleNotice").ToString());
                return;
            }

            // Svi autori čije knjige se nalaze na listi želja posjetioca
            // .SelectMany() razvija sve knjige sa liste želja u jedan niz autora
            // .GroupBy() + .Select(g => g.First()) eliminiše duplikate
            var autori = selektovani.ListaZelja
                .SelectMany(k => k.ListaAutora)
                .GroupBy(a => a.Broj_lk)
                .Select(g => g.First())
                .ToList();

            var prozor = new Autoriposetilacaprozor(autori, $"{selektovani.Ime} {selektovani.Prezime}");
            prozor.Owner = this;
            prozor.ShowDialog();
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
            string poruka1 = Application.Current.FindResource("aboutMessage1").ToString();
            string poruka2 = Application.Current.FindResource("aboutMessage2").ToString();
            string poruka3 = Application.Current.FindResource("aboutMessage3").ToString();
            string poruka4 = "[Miloš Trišić RA 39/2023]";
            string poruka5 = "[Boris Stepanović RA 97/2023]";
            string poruka = poruka1 + "\n\n" + poruka2 + "\n\n" + poruka3 + "\n- - - - - - - - - - - - - - - - - - - - - -\n" + poruka4 + "\n" +  poruka5;
            string naslov = Application.Current.FindResource("aboutTitle").ToString();

            MessageBox.Show(poruka, naslov);
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
        // Lokalizacija
        // ================================================================
        private void OsveziNaziveKolona()
        {
            // POSETIOCI
            if (DataGridPosetioci != null && DataGridPosetioci.Columns.Count > 0)
            {
                DataGridPosetioci.Columns[0].Header = Application.Current.FindResource("colClanskaKarta");
                DataGridPosetioci.Columns[1].Header = Application.Current.FindResource("colIme");
                DataGridPosetioci.Columns[2].Header = Application.Current.FindResource("colPrezime");
                DataGridPosetioci.Columns[3].Header = Application.Current.FindResource("colAdresa");
                DataGridPosetioci.Columns[4].Header = Application.Current.FindResource("colStatus");
            }

            // AUTORI
            if (DataGridAutori != null && DataGridAutori.Columns.Count > 0)
            {
                DataGridAutori.Columns[0].Header = Application.Current.FindResource("colIme");
                DataGridAutori.Columns[1].Header = Application.Current.FindResource("colPrezime");
                DataGridAutori.Columns[2].Header = Application.Current.FindResource("colLK");
                DataGridAutori.Columns[3].Header = Application.Current.FindResource("colDatumRodjenja");
                DataGridAutori.Columns[4].Header = Application.Current.FindResource("colEmail");
            }

            // KNJIGE
            if (DataGridKnjige != null && DataGridKnjige.Columns.Count > 0)
            {
                DataGridKnjige.Columns[0].Header = Application.Current.FindResource("colISBN");
                DataGridKnjige.Columns[1].Header = Application.Current.FindResource("colNaziv");
                DataGridKnjige.Columns[2].Header = Application.Current.FindResource("colCena");
                DataGridKnjige.Columns[3].Header = Application.Current.FindResource("colGodina");
                DataGridKnjige.Columns[4].Header = Application.Current.FindResource("colZanr");
            }
        }
        private void BtnSrpski_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current is App app)
            {
                app.ChangeLanguage("sr");

                // DODAJ OVE DVE LINIJE:
                OsveziNaziveKolona();
                OsveziPrikaz();

                string poruka = Application.Current.FindResource("msgJezikPromenjen").ToString();
                MessageBox.Show(poruka);
            }
        }

        private void BtnEngleski_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current is App app)
            {
                app.ChangeLanguage("en");

                // DODAJ OVE DVE LINIJE:
                OsveziNaziveKolona();
                OsveziPrikaz();

                string poruka = Application.Current.FindResource("msgJezikPromenjen").ToString();
                MessageBox.Show(poruka);
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


        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ignoriši događaje koji dolaze od child kontrola (DataGrid, ComboBox...)
            if (e.OriginalSource != MainTabControl) return;

            int tab = MainTabControl.SelectedIndex;
            if (tab == 0) stranicaPosetioci = 1;
            else if (tab == 1) stranicaAutori = 1;
            else if (tab == 2) stranicaKnjige = 1;

            OsveziPrikaz();
            OsveziVidljivostDugmadi();
        }

        private void OsveziVidljivostDugmadi()
        {
            if (btnPosetiociAutora == null || btnAutoriPosetioca == null) return;

            btnAutoriPosetioca.Visibility = MainTabControl.SelectedIndex == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            btnPosetiociAutora.Visibility = MainTabControl.SelectedIndex == 1
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private void timer_Tick(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            lblTime.Text = now.ToString("HH:mm:ss");
            lblDate.Text = now.ToString("dd.MM.yyyy.");
        }


    }
}