using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Domain.Entities;

namespace UniTrack.Persistence.Context
{
    public class UniTrackDbContext : DbContext
    {
        public UniTrackDbContext(DbContextOptions<UniTrackDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<UserClub> UserClubs { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ClubTeam> ClubTeams { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Like>(builder =>
            {
                builder.HasKey(l => l.Id);

                builder.HasOne(l => l.User)
                       .WithMany(u => u.Likes)
                       .HasForeignKey(l => l.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(l => l.Comment)
                       .WithMany(c => c.Likes)
                       .HasForeignKey(l => l.CommentId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(l => l.Club)
                       .WithMany(c => c.Likes)
                       .HasForeignKey(l => l.ClubId)
                       .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Notification>(builder =>
            {
                builder.HasKey(n => n.Id);
                builder.Property(n => n.Message).IsRequired().HasMaxLength(250);
                builder.Property(n => n.Title).IsRequired().HasMaxLength(100);
                builder.Property(n => n.Type).IsRequired().HasMaxLength(50);
                builder.Property(n => n.RelatedEntityId).IsRequired();
                builder.Property(n => n.IsRead).IsRequired();
                builder.Property(n => n.CreatedAt).IsRequired();
                builder.Property(n => n.Logo);
                

                builder.HasOne(n => n.User)
                       .WithMany(u => u.Notification)
                       .HasForeignKey(n => n.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(n => n.Club)
                        .WithMany(c => c.Notifications)
                        .HasForeignKey(n => n.ClubId)
                        .OnDelete(DeleteBehavior.Cascade);
                        
            });

            modelBuilder.Entity<ClubTeam>(builder =>
            {
                builder.HasKey(ct => ct.Id);
                builder.Property(ct => ct.UserDetailId).IsRequired().HasMaxLength(50);
                builder.Property(ct => ct.Title).IsRequired().HasMaxLength(50);

                builder.HasOne(ct => ct.Club)
                       .WithMany(c => c.ClubTeams)
                       .HasForeignKey(ct => ct.ClubId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ct => ct.UserDetail)
                       .WithOne(ud => ud.ClubTeam)
                       .HasForeignKey<ClubTeam>(ct => ct.UserDetailId)
                          .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Comment>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Point).IsRequired().HasMaxLength(5);
                builder.Property(c => c.Description).IsRequired().HasMaxLength(500);
                builder.Property(c => c.LikeCount);

                builder.HasOne(c => c.User)
                       .WithMany(eu => eu.Comments)
                       .HasForeignKey(eu => eu.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(c => c.Event)
                          .WithMany(e => e.Comments)
                          .HasForeignKey(e => e.EventId)
                          .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(c => c.Club)
                            .WithMany(cu => cu.Comments)
                            .HasForeignKey(cu => cu.ClubId)
                            .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EventUser>(builder =>
            {
                builder.HasKey(eu => new { eu.EventId, eu.UserId });
                builder.Property(eu => eu.IsJoined);
                builder.Property(eu => eu.IsCheckedIn);
                builder.Property(eu => eu.CheckedInAt);
                builder.Property(eu => eu.IsJoinedForSponsor);


                builder.HasOne(eu => eu.Event)
                       .WithMany(e => e.EventUsers)
                       .HasForeignKey(eu => eu.EventId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(eu => eu.User)
                       .WithMany(u => u.EventUsers)
                       .HasForeignKey(eu => eu.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserClub>(builder =>
            {
                builder.HasKey(eu => new { eu.ClubId, eu.UserId }); ;
                builder.Property(cu => cu.IsFollowing);
                builder.Property(c => c.IsNotification);

                builder.HasOne(cu => cu.Club)
                       .WithMany(c => c.UserClubs)
                       .HasForeignKey(cu => cu.ClubId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(cu => cu.User)
                       .WithMany(u => u.UserClubs)
                       .HasForeignKey(cu => cu.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(u => u.Id);
                builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
                builder.Property(u => u.Password).IsRequired();
                builder.Property(u => u.Role).IsRequired();
                builder.Property(u => u.LastLoginDate);

                builder.HasOne(u => u.UserDetail)
                       .WithOne(ud => ud.User)
                       .HasForeignKey<UserDetail>(ud => ud.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<UserDetail>(builder =>
            {
                builder.HasKey(ud => ud.Id);
                builder.Property(ud => ud.Name).IsRequired().HasMaxLength(50);
                builder.Property(ud => ud.Surname).IsRequired().HasMaxLength(50);
                builder.Property(ud => ud.BirthDate).IsRequired();
                builder.Property(ud => ud.Gender).IsRequired();
                builder.Property(ud => ud.ProfileImage).IsRequired(false);
                builder.Property(ud => ud.Graduaiton_Date).IsRequired();
                builder.Property(ud => ud.IsNotified).IsRequired();
                builder.Property(ud => ud.Language);
                builder.Property(ud => ud.PhoneNumber).IsRequired();

                builder.HasOne(ud => ud.City)
                       .WithMany(c => c.UserDetails)
                       .HasForeignKey(ud => ud.CityId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ud => ud.User)
                       .WithOne(u => u.UserDetail)
                       .HasForeignKey<UserDetail>(ud => ud.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ud => ud.University)
                       .WithMany(u => u.UserDetails)
                       .HasForeignKey(ud => ud.UniverstiyId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ud => ud.Department)
                          .WithOne(d => d.UserDetail)
                          .HasForeignKey<UserDetail>(ud => ud.DepartmentId)
                          .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Club>(builder => 
            { 
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Description).HasMaxLength(50);
                builder.Property(c => c.LongDescription).HasMaxLength(750);
                builder.Property(c => c.InstagramLink);
                builder.Property(c => c.TwitterLink);
                builder.Property(c => c.WebsiteLink);
                builder.Property(c => c.LinkedlnLink);
                builder.Property(c => c.Logo);
                builder.Property(c => c.CoverImage);
                builder.Property(c => c.Name).HasMaxLength(50);
                builder.Property(c => c.Tag);
                builder.Property(c => c.President);
                builder.Property(c => c.Follower);
                builder.Property(c => c.ContectEmail);
                builder.Property(c => c.PresidentMail);
                builder.Property(c => c.Password);
                builder.Property(c => c.Role);
                builder.Property(c => c.ClubCreatedDate);
               

                builder.HasOne(c => c.City)
                       .WithMany(u => u.Clubs)
                       .HasForeignKey(c => c.CityId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(c => c.University)
                       .WithMany(u => u.Clubs)
                       .HasForeignKey(c => c.UniversityId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(u => u.UserClubs)
                        .WithOne(c => c.Club)
                        .HasForeignKey(cu => cu.ClubId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Event>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.StartDate);
                builder.Property(e => e.Quota); 
                builder.Property(e => e.Joiner);
                builder.Property(e => e.Clock);
                builder.Property(e => e.EventTag);
                builder.Property(e => e.Description);
                builder.Property(e => e.Image);
                builder.Property(e => e.Title);
                builder.Property(e => e.EndDate);
                builder.Property(e => e.Location);
                builder.Property(e=> e.IsActived);
                builder.Property(e => e.Time);
                builder.Property(e => e.Status);
                builder.Property(e => e.SheetsId);
                builder.Property(e => e.CheckInToken);
                builder.Property(e => e.QrCodeUrl);
                

                builder.HasOne(e => e.City)
                       .WithMany(c => c.Events)
                       .HasForeignKey(e => e.CityId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(e => e.University)
                          .WithMany(u => u.Events)
                          .HasForeignKey(e => e.UniversityId)
                          .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(c => c.Club)
                       .WithMany(c => c.Events)
                       .HasForeignKey(e => e.ClubId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ban>(builder =>
            {
                builder.HasKey(b => b.Id);
                builder.Property(b => b.LastDate);
                builder.Property(b => b.IsBanned);
                builder.Property(b => b.Description);

                builder.HasOne(b => b.User)
                       .WithOne(u => u.Ban)
                       .HasForeignKey<Ban>(b => b.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(b => b.Club)
                          .WithOne(c => c.Ban)
                          .HasForeignKey<Ban>(b => b.ClubId)
                          .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(b => b.Event)
                            .WithOne(e => e.Ban)
                            .HasForeignKey<Ban>(b => b.EventId)
                            .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<University>(builder =>
            {
                builder.HasKey(u => u.Id);
                builder.Property(u => u.Name).IsRequired().HasMaxLength(100);

                builder.HasOne(u => u.City)
                       .WithMany(c => c.Universities)
                       .HasForeignKey(u => u.CityId)
                       .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Department>(builder =>
            {
                builder.HasKey(d => d.Id);
                builder.Property(d => d.Name).IsRequired().HasMaxLength(100);

            });


            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
