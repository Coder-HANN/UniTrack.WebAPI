using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        public DbSet<TargetNotification> TargetNotifications { get; set; }
        public DbSet<TargetNotificationCity> TargetNotificationCities { get; set; }
        public DbSet<TargetNotificationClub> TargetNotificationClubs { get; set; }
        public DbSet<TargetNotificationDepartment> TargetNotificationDepartments { get; set; }
        public DbSet<TargetNotificationUniversity> TargetNotificationUniversities { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<ClubNotification> ClubNotifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<EventImage> EventImages { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }
        public DbSet<NotificationChannelType> NotificationChannels { get; set; }
        public DbSet<EventQuestion> EventQuestions { get; set; }
        public DbSet<EventQuestionAnswer> EventQuestionAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NotificationChannelType>(builder =>
            {
                builder.HasKey(nc => new { nc.NotificationId, nc.Channel });

                builder.HasOne(nc => nc.Notification)
                      .WithMany(n => n.Channels)
                      .HasForeignKey(nc => nc.NotificationId);

                builder.Property(nc => nc.Channel)
                      .HasConversion<int>();
            });
 

            modelBuilder.Entity<EventImage>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(500);
                builder.Property(x => x.IsCover).HasDefaultValue(false);
                builder.Property(x => x.Order).HasDefaultValue(0);

                builder.HasOne(x => x.Event)
                    .WithMany(x => x.Images)
                    .HasForeignKey(x => x.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<AdminNotification>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Title);
                builder.Property(x => x.Description);
            });

            modelBuilder.Entity<Report>(builder =>
            {
         
                builder.HasKey(r => r.Id);

                builder.Property(r => r.Description).HasMaxLength(1000);
                builder.Property(r => r.Status).IsRequired();
                builder.Property(r => r.Reason).IsRequired();
                builder.Property(r => r.TargetType).IsRequired();
                builder.Property(r => r.ReviewedAt);

                builder.HasOne(r => r.User)
                    .WithMany(u => u.Reports) 
                    .HasForeignKey(r => r.ReporterUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                builder.HasOne(r => r.Club)
                    .WithMany(c => c.Reports) 
                    .HasForeignKey(r => r.ClubId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(r => r.Event)
                    .WithMany(e => e.Reports) 
                    .HasForeignKey(r => r.EventId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ClubNotification>(builder =>
            {
                builder.HasKey(cn => cn.Id);
                builder.Property(cn => cn.IsRead).IsRequired();
                builder.Property(cn => cn.ReadDate).IsRequired(false);


                builder.HasOne(cn => cn.Club)
                       .WithMany(c => c.ClubNotifications)
                       .HasForeignKey(cn => cn.ClubId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(cn => cn.Notification)
                       .WithMany(n => n.ClubNotifications)
                       .HasForeignKey(cn => cn.NotificationId)
                       .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<UserNotification>(builder =>
            {
                builder.HasKey(un => un.Id);

                builder.HasOne(un => un.User)
                       .WithMany(u => u.UserNotifications)
                       .HasForeignKey(un => un.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(un => un.Notification)
                       .WithMany(n => n.UserNotifications)
                       .HasForeignKey(un => un.NotificationId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Property(un => un.IsRead)
                       .IsRequired();

                builder.Property(un => un.ReadDate)
                       .IsRequired(false);
            });

            modelBuilder.Entity<TargetNotificationUniversity>(builder =>
            {
                builder.HasKey(tnu => tnu.Id);

                builder.HasOne(tnu => tnu.TargetNotification)
                       .WithMany(tn => tn.Universities)
                       .HasForeignKey(tnu => tnu.TargetNotificationId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(tnu => tnu.University)
                       .WithMany(u => u.TargetNotificationUniversities)
                       .HasForeignKey(tnu => tnu.UniversityId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TargetNotificationDepartment>(builder =>
            {
                builder.HasKey(tnd => tnd.Id);

                builder.HasOne(tnc => tnc.TargetNotification)
                        .WithMany(tn => tn.Departments)
                        .HasForeignKey(tnc => tnc.TargetNotificationId)
                        .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(tnc => tnc.Department)
                      .WithMany(c => c.Departments)
                      .HasForeignKey(tnc => tnc.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TargetNotificationClub>(builder =>
            {
                builder.HasKey(tnc => tnc.Id);

                builder.HasOne(tnc => tnc.Club)
                       .WithMany(c => c.TargetNotificationClubs)
                       .HasForeignKey(tnc => tnc.ClubId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(tnc =>tnc.TargetNotification)
                        .WithMany(tn => tn.Clubs)
                        .HasForeignKey(tnc =>tnc.TargetNotificationId)
                        .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TargetNotificationCity>(builder =>
            { 
                builder.HasKey(tn => tn.Id);

                builder.HasOne(e => e.City)
                       .WithMany(c => c.TargetNotificationCities)
                       .HasForeignKey(e => e.CityId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(e => e.TargetNotification)
                      .WithMany(c => c.City)
                      .HasForeignKey(e => e.TargetNotificationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TargetNotification>(builder =>
            {
                builder.HasKey(tn => tn.Id);
                
                builder.HasOne(tn => tn.Notification)
                       .WithMany(n => n.Targets)
                       .HasForeignKey(tn => tn.NotificationId)
                       .OnDelete(DeleteBehavior.Restrict);
                
            });

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
                builder.Property(n => n.Type).IsRequired().HasMaxLength(50);
                builder.Property(n => n.CreatedAt).IsRequired();
                builder.Property(n => n.LogoUrl);
                builder.Property(n => n.Title);
            });

            modelBuilder.Entity<ClubTeam>(builder =>
            {
                builder.HasKey(ct => ct.Id);
                builder.Property(ct => ct.Title).IsRequired();

                builder.HasOne(ct => ct.Club)
                       .WithMany(c => c.ClubTeams)
                       .HasForeignKey(ct => ct.ClubId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ct => ct.User)
                       .WithOne(u => u.ClubTeam)
                       .HasForeignKey<ClubTeam>(ct => ct.UserId)
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
                builder.Property(ud => ud.ProfileImageUrl).IsRequired(false);
                builder.Property(ud => ud.Graduaiton_Date).IsRequired();
                builder.Property(ud => ud.IsNotified).IsRequired();
                builder.Property(ud => ud.Language);

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
                          .WithMany(d => d.UserDetails)
                          .HasForeignKey(ud => ud.DepartmentId)
                          .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Club>(builder => 
            { 
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Description).HasMaxLength(200);
                builder.Property(c => c.LongDescription).HasMaxLength(750);
                builder.Property(c => c.InstagramLink);
                builder.Property(c => c.TwitterLink);
                builder.Property(c => c.WebsiteLink);
                builder.Property(c => c.LinkedlnLink);
                builder.Property(c => c.TikTokLink);
                builder.Property(c => c.LogoUrl);
                builder.Property(c => c.CoverImageUrl);
                builder.Property(c => c.Name).HasMaxLength(50);
                builder.Property(c => c.Tag);
                builder.Property(c => c.President);
                builder.Property(c => c.Follower);
                builder.Property(c => c.ContectEmail);
                builder.Property(c => c.PresidentMail);
                builder.Property(c => c.Password);
                builder.Property(c => c.Role);
                builder.Property(c => c.ClubCreatedDate);
                builder.Property(c => c.IsDoping);
               

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
                builder.Property(e => e.StartTime);
                builder.Property(e => e.EndTime);
                builder.Property(e => e.EventTag);
                builder.Property(e => e.Description);
                builder.Property(e => e.Title);
                builder.Property(e => e.EndDate);
                builder.Property(e => e.Location);
                builder.Property(e=> e.IsActived);
                builder.Property(e => e.Time);
                builder.Property(e => e.Status);
                builder.Property(e => e.SheetsId);
                builder.Property(e => e.CheckInToken);
                builder.Property(e => e.QrCodeUrl);
                builder.Property(e => e.IsDoping);
                

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
            modelBuilder.Entity<EventQuestion>(builder =>
            {
                builder.HasKey(q => q.Id);
                builder.Property(q => q.QuestionText).IsRequired().HasMaxLength(500);
                builder.Property(q => q.CreatedAt).IsRequired();

                builder.HasOne(q => q.Event)
                    .WithMany(e => e.Questions)
                    .HasForeignKey(q => q.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(q => q.User)
                    .WithMany(u => u.EventQuestions)
                    .HasForeignKey(q => q.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(q => q.Answer)
                    .WithOne(a => a.Question)
                    .HasForeignKey<EventQuestionAnswer>(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EventQuestionAnswer>(builder =>
            {
                builder.HasKey(a => a.Id);
                builder.Property(a => a.AnswerText).IsRequired().HasMaxLength(1000);
                builder.Property(a => a.AnsweredAt).IsRequired();

                builder.HasOne(a => a.Club)
                    .WithMany(c => c.EventQuestionAnswers)
                    .HasForeignKey(a => a.ClubId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
