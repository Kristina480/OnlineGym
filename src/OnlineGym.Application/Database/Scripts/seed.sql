-- =====================================================
-- KORISNICKI NALOZI
-- =====================================================

INSERT INTO korisnicki_nalozi
(korisnicko_ime, lozinka, vrsta_korisnika)
VALUES
    ('admin','admin123','ADMIN'),
    ('marko_trener','123','TRENER'),
    ('ana_trener','123','TRENER'),
    ('milan','123','KLIJENT'),
    ('jelena','123','KLIJENT'),
    ('nikola','123','KLIJENT');

--------------------------------------------------------

-- ADMINISTRATOR

INSERT INTO administratori
(nalog_id)
VALUES
    (1);

--------------------------------------------------------

-- TRENERI

INSERT INTO treneri
(nalog_id, ime, prezime, specijalnost, prosecna_ocena, skolovanje, preporuke)
VALUES
    (2,'Marko','Markovic','Bodybuilding',4.80,'FTN','Odlican trener'),
    (3,'Ana','Jovanovic','Fitness',4.95,'DIF','Veliko iskustvo');

--------------------------------------------------------

-- KLIJENTI

INSERT INTO klijenti
(nalog_id, ime, prezime, visina, tezina, cilj,
 broj_treninga_nedeljno, oprema, zdravstveni_problemi)
VALUES
    (4,'Milan','Petrovic',185,90,'Mršavljenje',3,'Tegovi',''),
    (5,'Jelena','Nikolic',170,60,'Kondicija',4,'Prostirka',''),
    (6,'Nikola','Ilic',178,82,'Mišićna masa',5,'Tegovi','Povreda kolena');

--------------------------------------------------------

-- LICENCE

INSERT INTO licence
(trener_id,naziv,tip_dokumenta,datum_izdavanja)
VALUES
    (1,'Personal Trainer','PDF','2024-01-15'),
    (2,'Fitness Instructor','PDF','2023-10-01');

--------------------------------------------------------

-- PAKETI

INSERT INTO paketi_cena
(trener_id,broj_treninga_nedeljno,cena_mesecno)
VALUES
    (1,3,12000),
    (1,5,18000),
    (2,3,11000),
    (2,5,17000);

--------------------------------------------------------

-- ZAHTEVI ZA SARADNJU

INSERT INTO zahtevi_za_saradnju
(klijent_id,trener_id,status)
VALUES
    (1,1,'ODOBREN'),
    (2,2,'ODOBREN'),
    (3,1,'CEKANJE');

--------------------------------------------------------

-- SARADNJE

INSERT INTO saradnje
(trener_id,klijent_id,zahtev_id,
 datum_od,datum_do,status,
 broj_treninga_nedeljno,cena_mesecno)
VALUES
    (1,1,1,'2026-07-01',NULL,'AKTIVNA',3,12000),
    (2,2,2,'2026-07-01',NULL,'AKTIVNA',5,17000);

--------------------------------------------------------

-- TRENINZI

INSERT INTO treninzi
(saradnja_id,datum,status,uradjen,komentar,ocena_treninga)
VALUES
    (1,'2026-07-10 18:00','ZAVRSEN',true,'Odlican trening',5),
    (1,'2026-07-12 18:00','ZAKAZAN',false,NULL,NULL),
    (2,'2026-07-11 19:00','ZAVRSEN',true,'Napredak',4);

--------------------------------------------------------

-- VEZBE

INSERT INTO vezbe
(trener_id,trening_id,naziv,link_snimka,
 broj_ponavljanja,trajanje,uradjena)
VALUES
    (1,1,'Bench Press','',10,60,true),
    (1,1,'Cucanj','',12,60,true),
    (1,2,'Mrtvo Dizanje','',8,60,false),
    (2,3,'Iskorak','',15,45,true),
    (2,3,'Sklekovi','',20,30,true);

--------------------------------------------------------

-- SPRAVE

INSERT INTO sprave
(vezba_id,naziv)
VALUES
    (1,'Bench'),
    (2,'Squat Rack'),
    (3,'Sipka'),
    (4,'Smith masina');

--------------------------------------------------------

-- REKVIZITI

INSERT INTO rekviziti
(vezba_id,naziv)
VALUES
    (1,'Tegovi'),
    (2,'Sipka'),
    (4,'Girja'),
    (5,'Prostirka');

--------------------------------------------------------

-- OCENE

INSERT INTO ocene
(klijent_id,trener_id,ocena,komentar)
VALUES
    (1,1,5,'Odlican trener'),
    (2,2,5,'Sve preporuke'),
    (3,1,4,'Zadovoljan');

--------------------------------------------------------

-- CLANARINE

INSERT INTO clanarine
(saradnja_id,mesec,datum_uplate,iznos,placeno)
VALUES
    (1,'2026-07-01','2026-07-02',12000,true),
    (2,'2026-07-01','2026-07-03',17000,true);

--------------------------------------------------------

-- PRIJAVE

INSERT INTO prijave
(klijent_id,trener_id,administrator_id,opis,status)
VALUES
    (1,2,1,'Kasnjenje na trening','CEKANJE'),
    (2,1,1,'Neodgovaranje na poruke','ODOBREN');

--------------------------------------------------------

-- ZAHTEVI ZA REGISTRACIJU

INSERT INTO zahtevi_za_registraciju
(trener_id,administrator_id,status)
VALUES
    (1,1,'ODOBREN'),
    (2,1,'ODOBREN');

--------------------------------------------------------

-- NOTIFIKACIJE

INSERT INTO notifikacije
(trener_id,sadrzaj,tip)
VALUES
    (1,'Imate novu prijavu.','PRIJAVA'),
    (2,'Nova saradnja je odobrena.','SARADNJA');