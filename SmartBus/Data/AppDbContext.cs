using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartBus.Models;

namespace SmartBus.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Autobus> Autobusi { get; set; }

    public virtual DbSet<Karta> Karte { get; set; }

    public virtual DbSet<Korisnik> Korisnici { get; set; }

    public virtual DbSet<Obavestenje> Obavestenja { get; set; }

    public virtual DbSet<ObavestenjeKorisnik> ObavestenjaKorisnici { get; set; }

    public virtual DbSet<OcenaAutobus> OceneAutobusa { get; set; }

    public virtual DbSet<Polazak> Polasci { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokeni { get; set; }

    public virtual DbSet<Sediste> Sedista { get; set; }

    public virtual DbSet<Stanica> Stanice { get; set; }

    public virtual DbSet<DnevniIzvestaj> DnevniIzvestaji { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Autobus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("autobus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrojSedista).HasColumnName("broj_sedista");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.Prevoznik)
                .HasMaxLength(100)
                .HasColumnName("prevoznik");
        });

        modelBuilder.Entity<Karta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("karta");

            entity.HasIndex(e => e.KorisnikId, "idx_karta_korisnik");

            entity.HasIndex(e => e.PolazakId, "idx_karta_polazak");

            entity.HasIndex(e => e.SedisteId, "karta_ibfk_4");

            entity.HasIndex(e => e.KontrolorId, "kontrolor_id");

            entity.HasIndex(e => new { e.PolazakId, e.SedisteId }, "unique_sediste").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrojSedista).HasColumnName("broj_sedista");
            entity.Property(e => e.KontrolorId).HasColumnName("kontrolor_id");
            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.PlacenaCena)
                .HasPrecision(10, 2)
                .HasColumnName("placena_cena");
            entity.Property(e => e.PolazakId).HasColumnName("polazak_id");
            entity.Property(e => e.SedisteId).HasColumnName("sediste_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'kupljena'")
                .HasColumnType("enum('kupljena','validirana','otkazana')")
                .HasColumnName("status");
            entity.Property(e => e.VremeKupovine)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("vreme_kupovine");
            entity.Property(e => e.VremeValidacije)
                .HasColumnType("datetime")
                .HasColumnName("vreme_validacije");

            entity.HasOne(d => d.Kontrolor).WithMany(p => p.KartaKontrolor)
                .HasForeignKey(d => d.KontrolorId)
                .HasConstraintName("karta_ibfk_3");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.KartaKorisnik)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("karta_ibfk_1");

            entity.HasOne(d => d.Polazak).WithMany(p => p.Karta)
                .HasForeignKey(d => d.PolazakId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("karta_ibfk_2");

            entity.HasOne(d => d.Sediste).WithMany(p => p.Karta)
                .HasForeignKey(d => d.SedisteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("karta_ibfk_4");
        });

        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("korisnik");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Ime)
                .HasMaxLength(100)
                .HasColumnName("ime");
            entity.Property(e => e.Lozinka)
                .HasMaxLength(255)
                .HasColumnName("lozinka");
            entity.Property(e => e.Uloga)
                .HasDefaultValueSql("'putnik'")
                .HasColumnType("enum('admin','putnik','kontrolor')")
                .HasColumnName("uloga");
            entity.Property(e => e.VremeRegistracije)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("vreme_registracije");
        });

        modelBuilder.Entity<Obavestenje>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("obavestenje");

            entity.HasIndex(e => e.PolazakId, "polazak_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Naslov)
                .HasMaxLength(200)
                .HasColumnName("naslov");
            entity.Property(e => e.PolazakId).HasColumnName("polazak_id");
            entity.Property(e => e.Poruka)
                .HasColumnType("text")
                .HasColumnName("poruka");
            entity.Property(e => e.Tip)
                .HasColumnType("enum('izmena','vanredna_situacija')")
                .HasColumnName("tip");
            entity.Property(e => e.VremeKreiranja)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("vreme_kreiranja");

            entity.HasOne(d => d.Polazak).WithMany(p => p.Obavestenje)
                .HasForeignKey(d => d.PolazakId)
                .HasConstraintName("obavestenje_ibfk_1");
        });

        modelBuilder.Entity<ObavestenjeKorisnik>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("obavestenje_korisnik");

            entity.HasIndex(e => e.KorisnikId, "korisnik_id");

            entity.HasIndex(e => e.ObavestenjeId, "obavestenje_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.ObavestenjeId).HasColumnName("obavestenje_id");
            entity.Property(e => e.Procitano).HasColumnName("procitano");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.ObavestenjeKorisnik)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("obavestenje_korisnik_ibfk_2");

            entity.HasOne(d => d.Obavestenje).WithMany(p => p.ObavestenjeKorisnik)
                .HasForeignKey(d => d.ObavestenjeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("obavestenje_korisnik_ibfk_1");
        });

        modelBuilder.Entity<OcenaAutobus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ocena_autobus");

            entity.HasIndex(e => e.AutobusId, "autobus_id");

            entity.HasIndex(e => e.KorisnikId, "korisnik_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutobusId).HasColumnName("autobus_id");
            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.Ocena).HasColumnName("ocena");

            entity.HasOne(d => d.Autobus).WithMany(p => p.OcenaAutobus)
                .HasForeignKey(d => d.AutobusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ocena_autobus_ibfk_1");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.OcenaAutobus)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ocena_autobus_ibfk_2");
        });

        modelBuilder.Entity<Polazak>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("polazak");

            entity.HasIndex(e => e.AutobusId, "autobus_id");

            entity.HasIndex(e => new { e.StanicaOd, e.StanicaDo, e.VremePolaska }, "idx_polazak_pretraga");

            entity.HasIndex(e => e.StanicaDo, "stanica_do");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutobusId).HasColumnName("autobus_id");
            entity.Property(e => e.Cena)
                .HasPrecision(10, 2)
                .HasColumnName("cena");
            entity.Property(e => e.Peron).HasColumnName("peron");
            entity.Property(e => e.StanicaDo).HasColumnName("stanica_do");
            entity.Property(e => e.StanicaOd).HasColumnName("stanica_od");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'aktivan'")
                .HasColumnType("enum('aktivan','otkazan','izmenjen')")
                .HasColumnName("status");
            entity.Property(e => e.VremePolaska)
                .HasColumnType("datetime")
                .HasColumnName("vreme_polaska");

            entity.HasOne(d => d.Autobus).WithMany(p => p.Polazak)
                .HasForeignKey(d => d.AutobusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("polazak_ibfk_1");

            entity.HasOne(d => d.StanicaDoNavigation).WithMany(p => p.PolazakStanicaDoNavigation)
                .HasForeignKey(d => d.StanicaDo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("polazak_ibfk_3");

            entity.HasOne(d => d.StanicaOdNavigation).WithMany(p => p.PolazakStanicaOdNavigation)
                .HasForeignKey(d => d.StanicaOd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("polazak_ibfk_2");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("refresh_token");

            entity.HasIndex(e => e.KorisnikId, "korisnik_id");

            entity.HasIndex(e => e.Token, "token").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DatumIsteka)
                .HasColumnType("datetime")
                .HasColumnName("datum_isteka");
            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.Opozvan).HasColumnName("opozvan");
            entity.Property(e => e.Token)
                .HasMaxLength(512)
                .HasColumnName("token");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.RefreshToken)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refresh_token_ibfk_1");
        });

        modelBuilder.Entity<Sediste>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sediste");

            entity.HasIndex(e => new { e.AutobusId, e.Broj }, "autobus_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutobusId).HasColumnName("autobus_id");
            entity.Property(e => e.Broj).HasColumnName("broj");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'slobodno'")
                .HasColumnType("enum('slobodno','zauzeto')")
                .HasColumnName("status");

            entity.HasOne(d => d.Autobus).WithMany(p => p.Sediste)
                .HasForeignKey(d => d.AutobusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sediste_ibfk_1");
        });

        modelBuilder.Entity<Stanica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stanica");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrojPerona).HasColumnName("broj_perona");
            entity.Property(e => e.Naziv)
                .HasMaxLength(100)
                .HasColumnName("naziv");
        });

        modelBuilder.Entity<DnevniIzvestaj>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("dnevni_izvestaj");

            entity.HasIndex(e => e.KontrolorId, "kontrolor_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datum).HasColumnName("datum");
            entity.Property(e => e.FizickiIncidenti).HasColumnName("fizicki_incidenti");
            entity.Property(e => e.IznosPlacenihKazni)
                .HasPrecision(10, 2)
                .HasColumnName("iznos_placenih_kazni");
            entity.Property(e => e.KontrolorId).HasColumnName("kontrolor_id");
            entity.Property(e => e.NepravilnostiNaVozilu)
                .HasColumnType("text")
                .HasColumnName("nepravilnosti_na_vozilu");
            entity.Property(e => e.NevalidneKarte).HasColumnName("nevalidne_karte");
            entity.Property(e => e.OdbijenaPlacanja).HasColumnName("odbijena_placanja");
            entity.Property(e => e.OpisIncidenata)
                .HasColumnType("text")
                .HasColumnName("opis_incidenata");
            entity.Property(e => e.PoziviPolicije).HasColumnName("pozivi_policije");
            entity.Property(e => e.PregledaneKarte).HasColumnName("pregledane_karte");
            entity.Property(e => e.PrekrsajniNalozi).HasColumnName("prekrsajni_nalozi");
            entity.Property(e => e.PrijavljenaLica).HasColumnName("prijavljena_lica");
            entity.Property(e => e.ProblemiSaSistemom)
                .HasColumnType("text")
                .HasColumnName("problemi_sa_sistemom");
            entity.Property(e => e.PutniciBezKarte).HasColumnName("putnici_bez_karte");
            entity.Property(e => e.Relacije)
                .HasColumnType("text")
                .HasColumnName("relacije");
            entity.Property(e => e.Smena)
                .HasColumnType("enum('jutarnja','popodnevna','nocna')")
                .HasColumnName("smena");
            entity.Property(e => e.ValidneKarte).HasColumnName("validne_karte");
            entity.Property(e => e.VerbalniIncidenti).HasColumnName("verbalni_incidenti");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
