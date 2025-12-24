using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class ConsoleApp
    {
        //objekti u DAO sloju
        PosetilacDAO posetilacDao = new PosetilacDAO();
        KnjigaDAO knjigaDao = new KnjigaDAO();

        public void Run()
        {
            int opcija;
            do
            {
                MenuBar.PrikaziMeni();
                opcija = MenuBar.UcitajOpciju();
                ObradiOpciju(opcija);

            }while(opcija != 0);
        }

        public void ObradiOpciju(int opcija)
        {
            switch (opcija)
            {
                case 0:
                    Console.WriteLine("Kraj programa...");
                    break;
                case 1:
                    DodajPosetioca();
                    break;
                case 2:
                    PrikazPosetilaca();
                    break;
                case 3:
                    IzmenaPosetioca();  
                    break;
                case 4:
                    BrisanjePosetioca();
                    break;
                case 5:
                    DodajKnjigu();
                    break;
                case 6:
                    IzmenaKnjige();
                    break;
                //TODO
                default:
                    StatusBar.PrikaziPoruku("Nepoznata opcija");
                    break;

            }
        }

        
        public void DodajPosetioca()
        {
            Console.WriteLine("=== Dodavanje posetioca ===");
            Console.Write("Ime : ");
            string ime = Console.ReadLine();
            Console.Write("Prezime : ");
            string prezime = Console.ReadLine();
            Console.Write("Datum rodjenja yyyy/mm/dd : ");
            DateTime datum = DateTime.Parse(Console.ReadLine());
            Console.Write("Adresa : ");
            string adresa = Console.ReadLine();
            Console.Write("Telefon : ");
            string telefon = Console.ReadLine();
            Console.Write("Email : ");
            string email = Console.ReadLine();
            string brkarte = (posetilacDao.GetALL().Count + 1).ToString();
            Console.Write("Godina clanstva : ");
            int godina = int.Parse(Console.ReadLine());
            Console.Write("Status (R - redovan, V - vip) : ");
            StatusPosetioca status = Console.ReadLine().ToUpper() == "V" ? StatusPosetioca.V : StatusPosetioca.R;

            Posetilac novi = new Posetilac(ime,prezime,datum,adresa,telefon,email,brkarte,godina,status);

            posetilacDao.Add(novi);

            StatusBar.PrikaziPoruku("Posetilac uspesno dodat!");
        }

        public void PrikazPosetilaca()
        {
            var svi = posetilacDao.GetALL();
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih posetilaca.");
                return;
            }

            Console.WriteLine("=== Lista posetilaca ===");
            foreach (var p in svi)
            {
                Console.WriteLine($"Br karte: {p.BrClanskeKarte}, Ime i prezime : {p.Ime} {p.Prezime}, " +
                                  $"Datum rodjenja: {p.DatumRodjenja.ToShortDateString()}, Status: {p.Status}");
            }
            Console.WriteLine("Pritisni ENTER za nastavak...");
            Console.ReadLine();
        }

        public void IzmenaPosetioca()
        {
            Console.Write("Unesite broj clanske karte posetioca za izmenu: ");
            string brKarte = Console.ReadLine();

            var posetilac = posetilacDao.GetByClanskaKarta(brKarte);
            if (posetilac == null)
            {
                StatusBar.PrikaziPoruku("Posetilac ne postoji!");
                return;
            }

            Console.Write("Novo ime / ENTER (bez izmena): ");
            string ime = Console.ReadLine();
            if (!string.IsNullOrEmpty(ime)) posetilac.Ime = ime;

            Console.Write("Novo prezime / ENTER (bez izmena): ");
            string prezime = Console.ReadLine();
            if (!string.IsNullOrEmpty(prezime)) posetilac.Prezime = prezime;

            Console.Write("Nova adresa / ENTER (bez izmena): ");
            string adresa = Console.ReadLine();
            if (!string.IsNullOrEmpty(adresa)) posetilac.Adresa = adresa;

            Console.Write("Telefon / ENTER (bez izmena): ");
            string telefon = Console.ReadLine();
            if (!string.IsNullOrEmpty(telefon)) posetilac.Telefon = telefon;

            Console.Write("Email / ENTER (bez izmena): ");
            string email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email)) posetilac.Email = email;

            Console.Write("Status R / V / ENTER (bez izmena): ");
            string status = Console.ReadLine().ToUpper();
            if (status == "R") posetilac.Status = StatusPosetioca.R;
            else if (status == "V") posetilac.Status = StatusPosetioca.V;

            StatusBar.PrikaziPoruku("Posetilac uspesno izmenjen!");
        }

        public void BrisanjePosetioca()
        {
            Console.Write("Unesite broj clanske karte posetioca za brisanje: ");
            string brKarte = Console.ReadLine();

            var posetilac = posetilacDao.GetByClanskaKarta(brKarte);
            if (posetilac == null)
            {
                StatusBar.PrikaziPoruku("Posetilac ne postoji!");
                return;
            }

            posetilacDao.Remove(posetilac);
            StatusBar.PrikaziPoruku("Posetilac uspesno obrisan!");
        }

        public void DodajKnjigu()
        {
            Console.WriteLine("=== Dodavanje knjige ===");

            Console.Write("Naziv: ");
            string naziv = Console.ReadLine();

            Console.Write("Izdavač: ");
            string izdavac = Console.ReadLine();

            Console.Write("Godina izdanja: ");
            string godina = Console.ReadLine();

            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();

            Console.Write("Broj strana: ");
            string brojStrana = Console.ReadLine();

            Console.Write("Jezik: ");
            string jezik = Console.ReadLine();

            string id = (knjigaDao.GetAll().Count + 1).ToString();
            
            List<Autor> autori = new List<Autor>();
            List<string> zanrovi = new List<string>();
            List<string> kljucneReci = new List<string>();

            Knjiga nova = new Knjiga(
                iSBN: isbn,
                naziv: naziv,
                zanr: "",
                godina_izdanja: godina,
                cena: "",
                broj_strana: brojStrana,
                autori: autori,
                izdavac: izdavac,
                kupili: new List<string>(),
                na_Listi_Zelja: new List<string>()
            );


            knjigaDao.Add(nova);

            StatusBar.PrikaziPoruku("Knjiga uspešno dodata!");
        }

        public void IzmenaKnjige()
        {
            Console.Write("Unesite ISBN knjige za izmenu: ");
            string isbn = Console.ReadLine();

            Knjiga knjiga = knjigaDao.GetByISBN(isbn);
            if (knjiga == null)
            {
                StatusBar.PrikaziPoruku("Knjiga ne postoji!");
                return;
            }

            Console.Write("=== Izmena knjige === ");
            Console.WriteLine("(ENTER - bez promene)");

            Console.Write($"Naziv ({knjiga.Naziv}): ");
            string naziv = Console.ReadLine();
            if (!string.IsNullOrEmpty(naziv))
                knjiga.Naziv = naziv;

            Console.Write($"Žanr ({knjiga.Zanr}): ");
            string zanr = Console.ReadLine();
            if (!string.IsNullOrEmpty(zanr))
                knjiga.Zanr = zanr;

            Console.Write($"Godina izdanja ({knjiga.Godina_izdanja}): ");
            string godina = Console.ReadLine();
            if (!string.IsNullOrEmpty(godina))
                knjiga.Godina_izdanja = godina;

            Console.Write($"Cena ({knjiga.Cena}): ");
            string cena = Console.ReadLine();
            if (!string.IsNullOrEmpty(cena))
                knjiga.Cena = cena;

            Console.Write($"Broj strana ({knjiga.Broj_strana}): ");
            string brStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(brStr))
                knjiga.Broj_strana = brStr;

            Console.Write($"Izdavač ({knjiga.Izdavac}): ");
            string izdavac = Console.ReadLine();
            if (!string.IsNullOrEmpty(izdavac))
                knjiga.Izdavac = izdavac;

            StatusBar.PrikaziPoruku("Knjiga uspešno izmenjena!");
           // StatusBar.PrikaziPoruku(knjiga.(#metodazaispisknjige#))

        }
    }
}
