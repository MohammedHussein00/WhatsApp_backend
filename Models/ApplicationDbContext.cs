using WhatsApp_Clone.Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Music_Aditor.Tables;

namespace WhatsApp_Clone.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);




            modelBuilder.Entity<IsRepliedMessage>()
                .HasKey(t => new { t.RepliedMessageId, t.AnsweredMessageId });



            modelBuilder.Entity<ConnectionGroupViaSignalR>()
                .HasKey(t => new { t.FirstUserId, t.SecondUserId });

            modelBuilder.Entity<GroupUser>()
                    .HasKey(t => new { t.UserId, t.GroupId });


            modelBuilder.Entity<MessageGroup>()
                .HasKey(t => new { t.MessageId, t.GroupId });

            modelBuilder.Entity<IsRepliedMessage>()
                .HasOne(ir => ir.RepliedMessage)
                .WithMany(m => m.RepliedMessages)
                .HasForeignKey(ir => ir.RepliedMessageId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if needed

            modelBuilder.Entity<IsRepliedMessage>()
                .HasOne(ir => ir.AnsweredMessage)
                .WithMany(m => m.AnsweredMessages)
                .HasForeignKey(ir => ir.AnsweredMessageId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if needed

        }


        public DbSet<User> Users { get; set; }
        public DbSet<IsForwardMessage> IsForwardMessages { get; set; }
        public DbSet<IsRepliedMessage> IsRepliedMessages { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConnectionViaSignalRUser> ConnectionViaSignalRUsers { get; set; }
        public DbSet<ConnectionGroupViaSignalR> ConnectionGroupViaSignalRs { get; set; }
        public DbSet<React> Reacts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<MessageGroup> MessageGroups { get; set; }

        
    }
}
