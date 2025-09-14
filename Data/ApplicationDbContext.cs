using Microsoft.EntityFrameworkCore;
using GestionStages.Models;

namespace GestionStages.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Enseignant> Enseignants { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Organisme> Organismes { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Validation> Validations { get; set; }
        public DbSet<Soutenance> Soutenances { get; set; }
        public DbSet<Suivi> Suivis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Etudiant)
                .WithOne(e => e.Utilisateur)
                .HasForeignKey<Etudiant>(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Enseignant)
                .WithOne(e => e.Utilisateur)
                .HasForeignKey<Enseignant>(e => e.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.Utilisateur)
                .HasForeignKey<Admin>(a => a.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stage>()
                .HasOne(s => s.Etudiant)
                .WithMany()
                .HasForeignKey(s => s.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stage>()
                .HasOne(s => s.Enseignant)
                .WithMany()
                .HasForeignKey(s => s.EnseignantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stage>()
                .HasOne(s => s.Organisme)
                .WithMany()
                .HasForeignKey(s => s.OrganismeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Document>()
                .HasOne(d => d.Etudiant)
                .WithMany(e => e.Documents)
                .HasForeignKey(d => d.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
