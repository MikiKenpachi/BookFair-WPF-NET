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
        AutorDAO autorDao = new AutorDAO();

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
                case 7:
                    DodajAutora();
                    break;
                case 8:
                    PrikazAutora();
                    break;
                case 9:
                    IzmenaAutora();
                    break;
                case 10:
                    BrisanjeAutora();
                    break;
                case 11:
                    PrikazKnjige();
                    break;
                case 12:
                    BrisanjeKnjige();
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

            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();

            Console.Write("Naziv: ");
            string naziv = Console.ReadLine();

            Console.Write("Žanr: ");
            string zanr = Console.ReadLine();

            Console.Write("Godina izdanja: ");
            string godina = Console.ReadLine();

            Console.Write("Cena: ");
            string cena = Console.ReadLine();

            Console.Write("Broj strana: ");
            string brojStrana = Console.ReadLine();

            Console.Write("Autori : ");
            //dodati autore kasnije

            Console.Write("Izdavač: ");
            string izdavac = Console.ReadLine();


            string id = (knjigaDao.GetAll().Count + 1).ToString();

            List<Autor> autori = new List<Autor>();


            Knjiga nova = new Knjiga();
            nova.ISBN = isbn;
            nova.Naziv = naziv;
            nova.Zanr = zanr;
            nova.Godina_izdanja = godina;
            nova.Cena = cena;
            nova.Broj_strana = brojStrana;
            nova.ListaAutora = autori;
            nova.Izdavac = izdavac;
            


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


        public void DodajAutora()
        {
            List<Knjiga> spisak = new List<Knjiga>();

            Console.WriteLine("=== Dodavanje autora ===");
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
            Console.Write("Broj lične karte : ");
            string lk = Console.ReadLine();
            Console.Write("Godine iskustva : ");
            int godine = int.Parse(Console.ReadLine());



            Autor novi = new Autor(ime, prezime, datum, adresa, telefon, email,  godine, lk, spisak);

            autorDao.Add(novi);

            StatusBar.PrikaziPoruku("Autor uspesno dodat!");
        }


        public void PrikazAutora()
        {
            var svi = autorDao.GetAll();
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih autora.");
                return;
            }

            Console.WriteLine("=== Lista autora ===");
            foreach (var p in svi)
            {
                Console.WriteLine($"Ime i prezime : {p.Ime} {p.Prezime}, " +
                                  $"Datum rodjenja: {p.Datum_rodjenja.ToShortDateString()}"
                                  +$"Telefon: {p.Telefon}" + $"Godine iskustva: {p.Godine_iskustva}");
            }
            Console.WriteLine("Pritisni ENTER za nastavak...");
            Console.ReadLine();
        }


        public void IzmenaAutora()
        {
            Console.Write("Unesite ličnu kartu autora za izmenu: ");
            string lck = Console.ReadLine();

            Autor autor = autorDao.GetByLicnaKarta(lck);
            if (autor == null)
            {
                StatusBar.PrikaziPoruku("Autor ne postoji!");
                return;
            }

            Console.Write("=== Izmena autora === ");
            Console.WriteLine("(ENTER - bez promene)");

            Console.Write($"Ime ({autor.Ime}): ");
            string naziv = Console.ReadLine();
            if (!string.IsNullOrEmpty(naziv))
                autor.Ime = naziv;

            Console.Write($"Prezime ({autor.Prezime}): ");
            string pr = Console.ReadLine();
            if (!string.IsNullOrEmpty(pr))
                autor.Prezime = pr;

            Console.Write($"Datum rodjenja ({autor.Datum_rodjenja}): ");
            string datum = Console.ReadLine();
            if (!string.IsNullOrEmpty(datum))
                autor.Datum_rodjenja = DateTime.Parse(datum);

            Console.Write($"Adresa ({autor.Adresa}): ");
            string adr= Console.ReadLine();
            if (!string.IsNullOrEmpty(adr))
                autor.Adresa = adr;
            Console.Write($"Telefon ({autor.Telefon}): ");
            string tel = Console.ReadLine();
            if (!string.IsNullOrEmpty(tel))
                autor.Telefon = tel;

            Console.Write($"Godine iskustva ({autor.Godine_iskustva}): ");
            string godine = Console.ReadLine();
            if (!string.IsNullOrEmpty(godine))
                autor.Godine_iskustva = int.Parse(godine);


            StatusBar.PrikaziPoruku("Autor uspešno izmenjen!");
          

        }

        public void BrisanjeAutora()
        {
            Console.Write("Unesite broj licne karte autora za brisanje: ");
            string brKarte = Console.ReadLine();

            var autor = autorDao.GetByLicnaKarta(brKarte);
            if (autor == null)
            {
                StatusBar.PrikaziPoruku("Autor ne postoji!");
                return;
            }

            autorDao.Remove(autor);
            StatusBar.PrikaziPoruku("Autor uspesno obrisan!");
        }

        public void BrisanjeKnjige()
        {
            Console.Write("Unesite ISBN knjige za brisanje: ");
            string brKarte = Console.ReadLine();

            var knjiga = knjigaDao.GetByISBN(brKarte);
            if (knjiga == null)
            {
                StatusBar.PrikaziPoruku("Knjiga ne postoji!");
                return;
            }

            knjigaDao.Remove(knjiga);
            StatusBar.PrikaziPoruku("Knjiga uspesno obrisana!");
        }
        public void PrikazKnjige()
        {
            var svi = knjigaDao.GetAll();
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih knjiga.");
                return;
            }

            Console.WriteLine("=== Lista knjiga ===");
            foreach (var p in svi)
            {
                Console.WriteLine($"ISBN: {p.ISBN}, Naziv : {p.Naziv}, " +
                                  $"Godina izdanja: {p.Godina_izdanja}, Zanr: {p.Zanr},"
                                  +$"Cena: {p.Cena}," + $"Broj strana: {p.Broj_strana}," + $"Autori: {p.ListaAutora},"
                                  + $"Izdavac: {p.Izdavac}," );
            }
            Console.WriteLine("Pritisni ENTER za nastavak...");
            Console.ReadLine();


        }
    }
}
