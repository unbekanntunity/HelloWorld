﻿using HelloWorldAPI.Domain.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace HelloWorldAPI.Data
{
    public class DataContext : IdentityDbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<ImagePath> ImageUrls { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Reply> Replies { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .HasOne(x => x.Creator)
                .WithMany(x => x.Articles)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<Article>()
               .HasMany(x => x.UserLiked)
               .WithMany(x => x.ArticlesLiked);

            modelBuilder.Entity<Article>()
              .HasMany(x => x.Replies)
              .WithOne(x => x.RepliedOnArticle)
              .HasForeignKey(x => x.RepliedOnArticleId);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.Creator)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<Comment>()
                .HasMany(x => x.UserLiked)
                .WithMany(x => x.CommentsLiked);

            modelBuilder.Entity<Comment>()
                       .HasMany(x => x.Replies)
                       .WithOne(x => x.RepliedOnComment)
                       .HasForeignKey(x => x.RepliedOnCommentId);

            modelBuilder.Entity<Discussion>()
                .HasMany(x => x.Tags)
                .WithMany(x => x.Discussions);

            modelBuilder.Entity<Discussion>()
                .HasMany(x => x.Articles)
                .WithOne(x => x.Discussion)
                .HasForeignKey(x => x.DiscussionId);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.Tags)
                .WithMany(x => x.Posts);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.UserLiked)
                .WithMany(x => x.PostsLiked);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.ImagePaths)
                .WithOne();

            modelBuilder.Entity<Project>()
                .HasMany(x => x.Tags)
                .WithMany(x => x.Projects);

            modelBuilder.Entity<Project>()
                .HasMany(x => x.UserLiked)
                .WithMany(x => x.ProjectsLiked);

            modelBuilder.Entity<Project>()
                .HasMany(x => x.Members)
                .WithMany(x => x.ProjectsJoined);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Articles)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Posts)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Projects)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Discussions)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Messages)
                .WithOne(x => x.Creator)
                .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Tags)
                .WithMany(x => x.Users);

            modelBuilder.Entity<User>()
              .HasMany(x => x.Replies)
              .WithOne(x => x.Creator)
              .HasForeignKey(x => x.CreatorId);

            modelBuilder.Entity<User>()
              .HasMany(x => x.RepliesLiked)
              .WithMany(x => x.UserLiked);

            modelBuilder.Entity<Reply>()
                .HasMany(x => x.Replies)
                .WithOne(x => x.RepliedOnReply)
                .HasForeignKey(x => x.RepliedOnReplyId);

            base.OnModelCreating(modelBuilder);
        }
    }
}