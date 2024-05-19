using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PROJECTALTERAPI.Models;

public partial class AlterDbContext : DbContext
{
    public AlterDbContext()
    {
    }

    public AlterDbContext(DbContextOptions<AlterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<Date> Dates { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<Exchange> Exchanges { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Link> Links { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<RatingStar> RatingStars { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost; Database=alter_db; Username=postgres; Password=2023");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("certification_pkey");

            entity.ToTable("certification");

            entity.Property(e => e.CertificationId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("certification_id");
            entity.Property(e => e.CertificationPicture).HasColumnName("certification_picture");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Certifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Date>(entity =>
        {
            entity.HasKey(e => e.DateId).HasName("date_pkey");

            entity.ToTable("date");

            entity.Property(e => e.DateId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("date_id");
            entity.Property(e => e.DateInfo).HasColumnName("date_info");
            entity.Property(e => e.ExchangeId).HasColumnName("exchange_id");

            entity.HasOne(d => d.Exchange).WithMany(p => p.Dates)
                .HasForeignKey(d => d.ExchangeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("exchange_id");
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("email_pkey");

            entity.ToTable("email");

            entity.Property(e => e.EmailId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("email_id");
            entity.Property(e => e.EmailAdresse)
                .HasColumnType("character varying")
                .HasColumnName("email_adresse");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Emails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Exchange>(entity =>
        {
            entity.HasKey(e => e.ExchangeId).HasName("exchange_pkey");

            entity.ToTable("exchange");

            entity.Property(e => e.ExchangeId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("exchange_id");
            entity.Property(e => e.ReciverId).HasColumnName("reciver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SkillReceiveId).HasColumnName("skill_receive_id");
            entity.Property(e => e.SkillSendId).HasColumnName("skill_send_id");
            entity.Property(e => e.Statues)
                .HasColumnType("character varying")
                .HasColumnName("statues");

            entity.HasOne(d => d.Reciver).WithMany(p => p.ExchangeRecivers)
                .HasForeignKey(d => d.ReciverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reciver_id");

            entity.HasOne(d => d.Sender).WithMany(p => p.ExchangeSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sender_id");

            entity.HasOne(d => d.SkillReceive).WithMany(p => p.ExchangeSkillReceives)
                .HasForeignKey(d => d.SkillReceiveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("skill_receive_id");

            entity.HasOne(d => d.SkillSend).WithMany(p => p.ExchangeSkillSends)
                .HasForeignKey(d => d.SkillSendId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("skill_send_id");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.FeedbackId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("feedback_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("languages_pkey");

            entity.ToTable("languages");

            entity.Property(e => e.LanguageId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("language_id");
            entity.Property(e => e.LanguageName)
                .HasColumnType("character varying")
                .HasColumnName("language_name");
            entity.Property(e => e.SkillId).HasColumnName("skill_id");

            entity.HasOne(d => d.Skill).WithMany(p => p.Languages)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("skill_id");
        });

        modelBuilder.Entity<Link>(entity =>
        {
            entity.HasKey(e => e.LinksId).HasName("links_pkey");

            entity.ToTable("links");

            entity.Property(e => e.LinksId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("links_id");
            entity.Property(e => e.LinkInformation).HasColumnName("link_information");
            entity.Property(e => e.SkillId).HasColumnName("skill_id");

            entity.HasOne(d => d.Skill).WithMany(p => p.Links)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("skill_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("Message_pkey");

            entity.ToTable("message");

            entity.Property(e => e.MessageId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("message_id");
            entity.Property(e => e.Content)
                .HasColumnType("character varying")
                .HasColumnName("content");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receiver_id");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sender_id");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("offer_pkey");

            entity.ToTable("offer");

            entity.Property(e => e.OfferId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("offer_id");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.OfferInfo).HasColumnName("offer_info");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Request).WithMany(p => p.Offers)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("request_id");

            entity.HasOne(d => d.User).WithMany(p => p.Offers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<RatingStar>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("rating_star_pkey");

            entity.ToTable("rating_star");

            entity.Property(e => e.RatingId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("rating_id");
            entity.Property(e => e.Rating)
                .HasColumnType("character varying")
                .HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RatingStars)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("request_pkey");

            entity.ToTable("request");

            entity.Property(e => e.RequestId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("request_id");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.RequestDescription).HasColumnName("request_description");
            entity.Property(e => e.RequestTitle)
                .HasColumnType("character varying")
                .HasColumnName("request_title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("skills_pkey");

            entity.ToTable("skills");

            entity.Property(e => e.SkillId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("skill_id");
            entity.Property(e => e.SkillDescription).HasColumnName("skill_description");
            entity.Property(e => e.SkillLevel)
                .HasColumnType("character varying")
                .HasColumnName("skill_level");
            entity.Property(e => e.SkillName)
                .HasColumnType("character varying")
                .HasColumnName("skill_name");
            entity.Property(e => e.SkillType)
                .HasColumnType("character varying")
                .HasColumnName("skill_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");

            entity.HasOne(d => d.User).WithMany(p => p.Skills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("token_pkey");

            entity.ToTable("token");

            entity.Property(e => e.TokenId)
                .ValueGeneratedNever()
                .HasColumnName("token_id");
            entity.Property(e => e.Token1)
                .HasColumnType("character varying")
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("topics_pkey");

            entity.ToTable("topics");

            entity.Property(e => e.TopicId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("topic_id");
            entity.Property(e => e.TopicName)
                .HasColumnType("character varying")
                .HasColumnName("topic_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Topics)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.UserId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(1L, null, 0L, null, null, null)
                .HasColumnName("user_id");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Picture).HasColumnName("picture");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
            entity.Property(e => e.VerifiedUser).HasColumnType("character varying[]");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("knowledge _pkey");

            entity.ToTable("wishlist");

            entity.Property(e => e.WishlistId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("wishlist_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WishlistName)
                .HasColumnType("character varying")
                .HasColumnName("wishlist_name");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
