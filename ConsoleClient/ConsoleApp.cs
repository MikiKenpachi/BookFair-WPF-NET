using Core.DAO;
using Core.Storage;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        AdresaDAO adresaDao= new AdresaDAO();
        IzdavacDAO izdavacDao = new IzdavacDAO();
        KupiliDAO kupiliDao = new KupiliDAO(); //neiskorisceno

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

       /* public void TestDataBinding()
        {
            // 1️⃣ Učitaj sve podatke iz DAO-a
            var posetioci = posetilacDao.GetAll();
            var knjige = knjigaDao.GetAll();
            var autori = autorDao.GetAll();
            var izdavaci = izdavacDao.GetAll();
            var kupovine = kupiliDao.GetAll(); // koristi instancu kupiliDao

            // 2️⃣ Pozovi DataBinding
            DataBinding.PoveziSve(posetioci, knjige, autori, izdavaci, kupovine);

            // 3️⃣ Test ispisa za proveru veza
            Console.WriteLine("=== Test DataBinding ===\n");

            foreach (var p in posetioci)
            {
                Console.WriteLine($"Posetilac: {p.Ime} {p.Prezime}, broj kupovina: {kupovine.Count}");
            }

            foreach (var k in knjige)
            {
                string autoriStr = k.ListaAutora.Count > 0
                    ? string.Join(", ", k.ListaAutora.Select(a => a.Ime + " " + a.Prezime))
                    : "Nema autora";

                string izdavac = k.Izdavac != null ? k.Izdavac.Naziv : "Nema izdavača";

                Console.WriteLine($"Knjiga: {k.Naziv}, Autori: {autoriStr}, Izdavač: {izdavac}");
            }

            foreach (var i in izdavaci)
            {
                string sef = i.SefIzdavaca != null ? i.SefIzdavaca.Ime + " " + i.SefIzdavaca.Prezime : "Nema šefa";
                Console.WriteLine($"Izdavač: {i.Naziv}, Šef: {sef}, broj autora: {i.ListaAutora.Count}");
            }

            Console.WriteLine("\n=== Kraj testa DataBinding ===");
        }*/


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
                case 13:
                    PrikaziAdrese();
                    break;
                case 14:
                    DodajIzdavaca();
                    break;
                case 15:
                    
                default:
                    StatusBar.PrikaziPoruku("Nepoznata opcija");
                    break;

            }
        }

        public void PrikaziAdrese()
        {
            var sve = adresaDao.GetAll();
            if(sve.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih adresa.");
                return;
            }
            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("        ADRESE : ");
            Console.WriteLine(new string('=', 30));
            foreach(var a in sve)
            {
                Console.WriteLine($"Ulica : {a.Ulica} {a.Broj} , Grad : {a.Grad} , Drzava : {a.Drzava}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
            Console.ReadLine();
        }
        public void DodajPosetioca()
        {
            Adresa adr=new Adresa();

            Console.WriteLine("=== Dodavanje posetioca ===");

            int nextId = posetilacDao.GetAll().Count + 1;  // broj novog posetioca
            string brClanskeKarte = $"CK-{nextId}";
            Console.Write("Ime : ");
            string ime = Console.ReadLine();
            Console.Write("Prezime : ");
            string prezime = Console.ReadLine();
            Console.Write("Datum rodjenja yyyy/mm/dd : ");
            DateTime datum = DateTime.Parse(Console.ReadLine());
            Console.Write("Adresa : ");
            adr = DodajAdresu(brClanskeKarte);
            Console.Write("Telefon : ");
            string telefon = Console.ReadLine();
            Console.Write("Email : ");
            string email = Console.ReadLine();
            
            Console.Write("Godina clanstva : ");
            int godina = int.Parse(Console.ReadLine());
            Console.Write("Status (R - redovan, V - vip) : ");
            StatusPosetioca status = Console.ReadLine().ToUpper() == "V" ? StatusPosetioca.V : StatusPosetioca.R;

           
            Posetilac novi = new Posetilac(ime,prezime,datum,adr,telefon,email, brClanskeKarte, godina,status);

            posetilacDao.Add(novi);

            StatusBar.PrikaziPoruku("Posetilac uspesno dodat!");
        }

        public void PrikazPosetilaca()
        {
            var svi = posetilacDao.GetAll(); 
            if (svi.Count == 0)
            {
                StatusBar.PrikaziPoruku("Nema registrovanih posetilaca.");
                return;
            }

            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("      LISTA POSETILACA");
            Console.WriteLine(new string('=', 30));

            foreach (var p in svi)
            {
                
                string statusOpis = p.Status == StatusPosetioca.V ? "VIP Posetilac" : "Redovni posetilac";

                string adresaInfo = p.Adresa != null
                    ? $"{p.Adresa.Ulica} {p.Adresa.Broj}, {p.Adresa.Grad}"
                    : "Nema adrese";

                Console.WriteLine($"Članska karta:  {p.BrClanskeKarte}");
                Console.WriteLine($"Ime i prezime:  {p.Ime} {p.Prezime}");
                Console.WriteLine($"Status:         {statusOpis}");
                Console.WriteLine($"Član od:        {p.GodinaClanstva}. godine");
                Console.WriteLine($"Prosečna ocena: {p.ProsecnaOcena:F2}");
                Console.WriteLine($"Telefon:        {p.Telefon}");
                Console.WriteLine($"Email:          {p.Email}");

                Console.WriteLine($"Adresa:         {adresaInfo}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
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

            IzmeniAdresu(posetilac.BrClanskeKarte);

            StatusBar.PrikaziPoruku("Posetilac uspesno izmenjen!");
            posetilacDao.Save();

            
        }

        public void IzmeniAdresu(string vlasnikID)
        {
            var adresa = adresaDao
                .GetAll()
                .FirstOrDefault(a => a.VlasnikID == vlasnikID);

            if (adresa == null)
            {
                StatusBar.PrikaziPoruku("Adresa za datog vlasnika ne postoji!");
                return;
            }

            Console.Write("Unesite novu ulicu: ");
            adresa.Ulica = Console.ReadLine();

            Console.Write("Unesite novi broj: ");
            adresa.Broj = Console.ReadLine();

            Console.Write("Unesite novi grad: ");
            adresa.Grad = Console.ReadLine();

            Console.Write("Unesite novu državu: ");
            adresa.Drzava = Console.ReadLine();

            adresaDao.Update(adresa);

            StatusBar.PrikaziPoruku("Adresa uspešno izmenjena!");
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


            var adresa = adresaDao.GetAll().FirstOrDefault(a => a.VlasnikID == brKarte);
            if (adresa != null)
            {
                adresaDao.Remove(adresa.VlasnikID);
                posetilacDao.Remove(posetilac);
            }

            posetilacDao.Remove(posetilac);
            StatusBar.PrikaziPoruku("Posetilac uspesno obrisan!");
            StatusBar.PrikaziPoruku("Adresa uspesno obrisana!");
        }

        public void DodajIzdavaca()
        {
            Console.Write("Unesite šifru izdavača: ");
            string sifra = Console.ReadLine();
            Console.Write("Unesite naziv izdavača: ");
            string naziv = Console.ReadLine();
            Console.Write("Unesite Broj LK autora (Šefa): ");
            string lkSefa = Console.ReadLine();

            var autor = autorDao.GetByLicnaKarta(lkSefa);

            if (autor == null)
            {
                StatusBar.PrikaziPoruku("Greška: Autor sa tim LK ne postoji!");
                return;
            }

            if (autor.Godine_iskustva < 5)
            {
                StatusBar.PrikaziPoruku("Greška: Šef mora imati min. 5 godina iskustva!");
                return;
            }

            Izdavac noviIzdavac = new Izdavac(sifra, naziv, autor, new List<Autor>(), new List<Knjiga>());
            izdavacDao.Add(noviIzdavac);
            StatusBar.PrikaziPoruku("Izdavač uspešno dodat!");
        }
        public void DodajKnjigu()
        {
            Console.WriteLine("=== Dodavanje knjige ===");

            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();

            Console.Write("Naziv: ");
            string naziv = Console.ReadLine();

            Console.Write("Žanr (Romantika, Naučna_fantastika, Misterija, Biografija, Istorijski_roman, Fantazija, Triler, Horor, Poezija, Drama): ");
            string zanrInput = Console.ReadLine();
            var zanr = (Knjiga.Zanrovi)Enum.Parse(typeof(Knjiga.Zanrovi), zanrInput.Replace(' ', '_'), true);

            Console.Write("Godina izdanja: ");
            string godina = Console.ReadLine();

            Console.Write("Cena: ");
            string cena = Console.ReadLine();

            Console.Write("Broj strana: ");
            string brojStrana = Console.ReadLine();

            // -----------------------
            // Unos autora
            var sviAutori = autorDao.GetAll();
            Console.WriteLine("Dostupni autori:");
            foreach (var a in sviAutori)
                Console.WriteLine($"{a.Broj_lk} - {a.Ime} {a.Prezime}");

            Console.Write("Unesite Broj LK autora (odvojene sa ; ako ih je više): ");
            string autoriInput = Console.ReadLine();
            var uneseniAutori = autoriInput.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(id => sviAutori.FirstOrDefault(a => a.Broj_lk == id.Trim()))
                .Where(a => a != null)
                .ToList();

            // -----------------------
            // Unos izdavača
            var sviIzdavaci = izdavacDao.GetAll();
            Console.WriteLine("Dostupni izdavači:");
            foreach (var i in sviIzdavaci)
                Console.WriteLine($"{i.Sifra} - {i.Naziv}");

            Console.Write("Unesite ID izdavača: ");
            string izdavacID = Console.ReadLine();
            var izdavac = sviIzdavaci.FirstOrDefault(i => i.Sifra == izdavacID);

            // -----------------------
            // Kreiranje nove knjige
            Knjiga nova = new Knjiga
            {
                ISBN = isbn,
                Naziv = naziv,
                Zanr = zanr,
                Godina_izdanja = godina,
                Cena = cena,
                Broj_strana = brojStrana,
                ListaAutora = uneseniAutori,
                Izdavac = izdavac
            };

            // -----------------------
            // Čuvanje knjige
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
                knjiga.Zanr = (Knjiga.Zanrovi)Enum.Parse(typeof(Knjiga.Zanrovi), zanr.Replace(' ', '_'), true);

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
                knjiga.Izdavac.Naziv = izdavac;

            StatusBar.PrikaziPoruku("Knjiga uspešno izmenjena!");
            knjigaDao.Save();

        }

        public Adresa DodajAdresu(string ID) //id moze biti broj licne karte autora, ili broj clanske karte posetioca (sluzi kao ID)
        {
            
            Console.Write("Ime ulice : ");
            string ulica = Console.ReadLine();
            Console.Write("Grad : ");
            string grad = Console.ReadLine();
            Console.Write("Broj : ");
            string broj = Console.ReadLine();
            Console.Write("Drzava : ");
            string drzava = Console.ReadLine();


            Adresa adresa = new Adresa(ID, ulica, grad, broj, drzava);

            adresaDao.Add(adresa);
            

            return adresa;
        }
        public void DodajAutora()
        {
            Adresa adr = new Adresa();

            Console.WriteLine("=== Dodavanje autora ===");
            Console.Write("Ime : ");
            string ime = Console.ReadLine();
            Console.Write("Prezime : ");
            string prezime = Console.ReadLine();
            Console.Write("Datum rodjenja yyyy/mm/dd : ");

            DateTime datum = DateTime.Parse(Console.ReadLine());

            Console.Write("Telefon : ");
            string telefon = Console.ReadLine();
            Console.Write("Email : ");
            string email = Console.ReadLine();       
            Console.Write("Broj lične karte : ");
            string lk = Console.ReadLine();
            Console.Write("Godine iskustva : ");
            int godine = int.Parse(Console.ReadLine());

            Console.WriteLine("Dodaj adresu");
            adr = DodajAdresu(lk);

            Autor novi = new Autor(ime, prezime, datum, adr, telefon, email, godine, lk);

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

            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("       LISTA AUTORA");
            Console.WriteLine(new string('=', 30));

            foreach (var p in svi)
            {

                string adresaInfo = p.Adresa != null
                    ? $"{p.Adresa.Ulica} {p.Adresa.Broj}, {p.Adresa.Grad}"
                    : "Nema adrese";

                string knjigeID = (p.SpisakKnjiga != null && p.SpisakKnjiga.Any())
                    ? string.Join(", ", p.SpisakKnjiga.Select(k => k.ISBN))
                    : "Nema zapisanih knjiga";

                
                Console.WriteLine($"LK:       {p.Broj_lk}");
                Console.WriteLine($"Autor:    {p.Ime} {p.Prezime}");
                Console.WriteLine($"Rođen:    {p.Datum_rodjenja.ToShortDateString()}");
                Console.WriteLine($"Iskustvo: {p.Godine_iskustva} god.");
                Console.WriteLine($"Telefon:        {p.Telefon}");
                Console.WriteLine($"Email:          {p.Email}");
                Console.WriteLine($"Adresa:   {adresaInfo}");
                Console.WriteLine($"Knjige (ISBN): {knjigeID}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
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


            Console.Write($"Telefon ({autor.Telefon}): ");
            string tel = Console.ReadLine();
            if (!string.IsNullOrEmpty(tel))
                autor.Telefon = tel;

            Console.Write($"Godine iskustva ({autor.Godine_iskustva}): ");
            string godine = Console.ReadLine();
            if (!string.IsNullOrEmpty(godine))
                autor.Godine_iskustva = int.Parse(godine);

            IzmeniAdresu(autor.Broj_lk);

            StatusBar.PrikaziPoruku("Autor uspešno izmenjen!");
            autorDao.Save();

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

            adresaDao.Remove(autor.Broj_lk);
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

            Console.WriteLine("\n" + new string('=', 30));
            Console.WriteLine("       LISTA KNJIGA");
            Console.WriteLine(new string('=', 30));

            foreach (var p in svi)
            {

                string autoriID = (p.ListaAutora != null && p.ListaAutora.Any())
                    ? string.Join(", ", p.ListaAutora.Select(a => a.Broj_lk))
                    : "Nema autora";

                string izdavacSifra = p.Izdavac != null ? p.Izdavac.Sifra : "Nema izdavača";

                Console.WriteLine($"ISBN: {p.ISBN}");
                Console.WriteLine($"Naziv:  {p.Naziv}");
                Console.WriteLine($"(Zanr, god_izd, brstr): {p.Zanr} | {p.Godina_izdanja}. god | {p.Broj_strana} str.");
                Console.WriteLine($"Cena:    {p.Cena} RSD");
                Console.WriteLine($"Izdavač: {izdavacSifra}");
                Console.WriteLine($"Autori (LK): {autoriID}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nPritisni ENTER za nastavak...");
            Console.ReadLine();
        }

    }
}
