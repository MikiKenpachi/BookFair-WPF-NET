using Core.DAO;
using SajamKnjigaProjekat.Core;
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
    /// Interaction logic for DodajKnjiguProzor.xaml
    /// </summary>
    public partial class DodajKnjiguProzor : Window
    {
        AutorDAO autorDao = new AutorDAO();
        IzdavacDAO izdavacDao = new IzdavacDAO();

        public Knjiga? NovaKnjiga { get; private set; }
        public DodajKnjiguProzor(Knjiga a = null)
        {
            InitializeComponent();
            PopuniPodatke();
            if (a != null)
            {
                NovaKnjiga = a;
                PopuniPolja();
                this.Title = "Izmeni knjigu"; // Promenimo naslov prozora
            }
            else
            {
                this.Title = "Dodaj novou knjigu";
            }
            
        }
        private void PopuniPodatke()
        {
            // PrNazivr: dohvatiš liste iz servisa ili baze
            cbAutor.ItemsSource = autorDao.GetAll();
            cbIzdavac.ItemsSource = izdavacDao.GetAll();
        }

        private void PopuniPolja()
        {
            txtNaziv.Text = NovaKnjiga.Naziv;
            txtISBN.Text = NovaKnjiga.ISBN;
            dpGI.Text = NovaKnjiga.Godina_izdanja; // Proveri naziv property-ja
            txtCena.Text = NovaKnjiga.Cena;
            txtBS.Text = NovaKnjiga.Broj_strana;

            if (NovaKnjiga.ListaAutora != null && NovaKnjiga.ListaAutora.Count > 0)
            {
                var prviAutor = NovaKnjiga.ListaAutora[0];
                cbAutor.SelectedItem = cbAutor.Items.Cast<Autor>()
                    .FirstOrDefault(a => a.Broj_lk == prviAutor.Broj_lk);
            }

            // 2. Postavljanje Izdavača
            if (NovaKnjiga.Izdavac != null)
            {
                cbIzdavac.SelectedItem = cbIzdavac.Items.Cast<Izdavac>()
                    .FirstOrDefault(i => i.Sifra == NovaKnjiga.Izdavac.Sifra);
            }
            if (cbStatus != null && NovaKnjiga != null)
            {
                foreach (var obj in cbStatus.Items)
                {
                    // Moramo kastovati 'obj' u ComboBoxItem da bismo videli njegov Tag
                    if (obj is ComboBoxItem item)
                    {
                        string tagStavke = item.Tag?.ToString();

                        // Proveri u Debuggeru šta je tagStavke, a šta NovaKnjiga.Zanr.ToString()
                        if (tagStavke == NovaKnjiga.Zanr.ToString())
                        {
                            cbStatus.SelectedItem = item;
                            break;
                        }
                    }
                }
            }



            // Postavi i ComboBox status ako je potrebno
        }

    
        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text))
            {
                MessageBox.Show("Molimo unesite Naziv.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Ako NovaKnjiga ne postoji (dodavanje novog), kreiraj novu instancu
            // Ako postoji (izmena), samo ćemo mu ažurirati property-je
            if (NovaKnjiga == null)
            {
                NovaKnjiga = new Knjiga();
                // Ako tvoja klasa Knjiga ima i instancu klase Adresa unutar sebe:
               
            }

            NovaKnjiga.Naziv = txtNaziv.Text;

           NovaKnjiga.ISBN=txtISBN.Text;

           NovaKnjiga.Godina_izdanja = dpGI.Text; // Proveri naziv property-ja

           NovaKnjiga.Cena= txtCena.Text;

            NovaKnjiga.Broj_strana=txtBS.Text;


            // Čitanje statusa iz ComboBox-a
            if (cbStatus.SelectedItem is ComboBoxItem selectedItem)
            {
                string sadrzaj = selectedItem.Content.ToString();

                if (sadrzaj == "Romantika")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Roman;
                }
                else if(sadrzaj == "Naučna fantastika")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Naucna_Fantastika;
                }
                else if (sadrzaj == "Horor")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Horor;
                }
                else if (sadrzaj == "Kriminalistika")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Kriminalistika;
                }
                else if (sadrzaj == "Dečija literatura")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Decija_Literatura;
                }
                else if (sadrzaj == "Naučna fantastika")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Naucna_Fantastika;
                }
                else if (sadrzaj == "Poezija")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Poezija;
                }
                else if (sadrzaj == "Biografija")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Biografija;
                }
                else if (sadrzaj == "Enciklopedija")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Enciklopedija;
                }
                else if (sadrzaj == "Fantastika")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Fantastika;
                }
                else if (sadrzaj == "Stručna literatura")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Strucna_Literatura;
                }
                else if (sadrzaj == "Istorijska")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Istorijska;
                }
                else if (sadrzaj == "Drama")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Drama;
                }
                else if (sadrzaj == "Klasika")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Klasika;
                }
                else if (sadrzaj == "Triler")
                {
                    NovaKnjiga.Zanr = Knjiga.Zanrovi.Triler;
                }
            }

            // Čitanje razdvojenih polja adrese
            if (cbAutor.SelectedItem is Autor izabraniAutor)
            {
                // Upisujemo u tvoju promenljivu (pretpostavimo da se zove broj_lk_autora)
                var sviAutori = autorDao.GetAll();
                var uneseniAutori = izabraniAutor.Broj_lk.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(id => sviAutori.FirstOrDefault(a => a.Broj_lk == id.Trim()))
               .Where(a => a != null)
               .ToList();
                NovaKnjiga.ListaAutora = uneseniAutori;
            }


            if (cbIzdavac.SelectedItem is Izdavac izabraniIzdavac)
            {
              
                var sviIzdavaci = izdavacDao.GetAll();
                var izdavac = sviIzdavaci.FirstOrDefault(i => i.Sifra == izabraniIzdavac.Sifra);
                NovaKnjiga.Izdavac = izdavac;
            }

            // 4. Zatvaranje prozora uz potvrdu
            // Postavljanjem DialogResult na true, signaliziramo pozivaocu (MainWindow) da je akcija uspešna
            this.DialogResult = true;
            this.Close();

        }
    }
}
