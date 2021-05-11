using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApplication
{
    #region MusicApp
    /* many-to-many in EF
     * 
     * the different ways to do it: 
     * 
     *      -fluent api (current in use)
     *      -InverseProperty
     *      -Using Data Annotations
     */
    public class MusicAppContext : DbContext
    {
        public MusicAppContext() : base("name=MusicAppDBConnectionString")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MusicAppContext>());
            Database.SetInitializer<MusicAppContext>(
    new DropCreateDatabaseAlways<MusicAppContext>());
            this.Configuration.LazyLoadingEnabled = true;   // Entity framework staat lazyloading bij default aan;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<ArtistSong>()
            //   .HasRequired(f => f.Song)
            //   .WithRequiredDependent()
            //   .WillCascadeOnDelete(false);

            modelBuilder.Entity<Album>()
                .HasMany(a => a.Songs)
                .WithMany(s => s.Albums)
                .Map(cs =>
                {
                    cs.MapLeftKey("AlbumId");
                    cs.MapRightKey("SongId");
                    cs.ToTable("AlbumSongs");
                });
            modelBuilder.Entity<Song>()
                .HasMany(s => s.Artists)
                .WithMany(a => a.Songs)
                .Map(cs =>
                {
                    cs.MapLeftKey("SongId");
                    cs.MapRightKey("ArtistId");
                    cs.ToTable("ArtistSongs");
                });
            modelBuilder.Entity<Song>()
                .HasMany(s => s.Playlists)
                .WithMany(p => p.Songs)
                .Map(cs =>
                {
                    cs.MapLeftKey("SongId");
                    cs.MapRightKey("PlaylistId");
                    cs.ToTable("PlaylistSongs");
                });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Firstname { get; set; } = "";
        [MaxLength(50)]
        [Required]
        public string Lastname { get; set; } = "";
        [MaxLength(50)]
        [Required]
        public string Email { get; set; } = "";
        [MaxLength(50)]
        [Required]
        public string Password { get; set; } = "";
        virtual public ICollection<Playlist> Playlists { get; set; } //  Source: https://www.ryadel.com/en/enable-or-disable-lazyloading-in-entity-framework/
                                                                     //  why virtual? it enables Lazy loading and will be overridden.   
                                                                     //  lazy loading = the "SQL query" loads only the data we need instead of loading all data (+relations) and filter afterwards.
        virtual public ICollection<Interaction> Interactions { get; set; }
        public User()
        {
            Playlists = new HashSet<Playlist>();
            Interactions = new HashSet<Interaction>();
        }
    }
    public class Song
    {
        [Key]
        public int SongId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Title { get; set; } = "";
        public int Length { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public virtual ICollection<Interaction> Interactions { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
        //public virtual ICollection<SongAlbum> SongAlbums { get; set; }
        //[InverseProperty(nameof(Album.Songs))]                            <----  InverseProperty 
        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<Artist> Artists { get; set; }
        public Song()
        {
            Albums = new HashSet<Album>();
            Artists = new HashSet<Artist>();
            Playlists = new HashSet<Playlist>();
            Interactions = new HashSet<Interaction>();
        }
    }
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public virtual ICollection<Song> Songs { get; set; }
        public Album()
        {
            Songs = new HashSet<Song>();
        }
    }
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public virtual ICollection<Song> Songs { get; set; }
        public Artist()
        {
            Songs = new HashSet<Song>();
        }
    }
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public virtual ICollection<Song> Songs { get; set; }
        public Playlist()
        {
            Songs = new HashSet<Song>();
        }
    }
    public class Interaction
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Key]
        [Column(Order = 2)]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }
        public bool Liked { get; set; } = false;
        public int PlayCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }

    // --------------------- Koppeltabellen / Using Data Annotations ------------------------------------

    public class SongAlbum
    {
        [Key]
        [Column(Order = 1)]
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
        [Key]
        [Column(Order = 2)]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }
    }
    public class ArtistSong
    {
        [Key]
        [Column(Order = 1)]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
    }
    public class PlaylistSong
    {
        [Key]
        [Column(Order = 1)]
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }
        [Key]
        [Column(Order = 2)]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }
    }
    #endregion
}
