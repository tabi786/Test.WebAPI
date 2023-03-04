using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static System.Net.Mime.MediaTypeNames;


#nullable disable

namespace Test.WebAPI
{
	public partial class TestDataContext : DbContext
	{


		public TestDataContext()
		{
		}

		public TestDataContext(DbContextOptions<TestDataContext> options)
				: base(options)
		{
		}





		public virtual DbSet<User> Users { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{ }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("PK_Users_ID");

				entity.ToTable(tb =>
				{
					tb.HasTrigger("trig_Users_Inserted");
					tb.HasTrigger("trig_Users_Updated");
				});

				entity.HasIndex(e => e.Active, "User_Active");

				entity.HasIndex(e => e.AlternateEmail, "User_AlternateEmail");

				entity.HasIndex(e => e.CountryId, "User_Country");

				entity.HasIndex(e => e.CreatedOn, "User_Created").IsDescending();

				entity.HasIndex(e => e.CreatedOn, "User_CreatedOn").IsDescending();

				entity.HasIndex(e => e.Fullname, "User_Fullname");

				entity.HasIndex(e => e.Givenname, "User_Givenname");

				entity.HasIndex(e => e.InvitedByUserId, "User_InvitedByUser").HasFilter("([InvitedByUserID] IS NOT NULL)");

				entity.HasIndex(e => e.LeftLegUserId, "User_LeftLegUser")
						.IsUnique()
						.HasFilter("([LeftLegUserID] IS NOT NULL)");

				entity.HasIndex(e => e.Level, "User_Level");

				entity.HasIndex(e => e.MemberId, "User_Member");

				entity.HasIndex(e => e.ModifiedOn, "User_ModifiedOn").IsDescending();

				entity.HasIndex(e => e.ParentUserId, "User_Parent").HasFilter("([ParentUserID] IS NOT NULL)");

				entity.HasIndex(e => new { e.ParentUserId, e.ParentUserLeg }, "User_Parent_Leg")
						.IsUnique()
						.HasFilter("([UserType]=(2) AND [ParentUserID] IS NOT NULL AND [ParentUserLeg] IS NOT NULL)");

				entity.HasIndex(e => e.ParentUserLeg, "User_Parent_Leg1").HasFilter("([ParentUserLeg] IS NOT NULL)");

				entity.HasIndex(e => new { e.PersonalIdentificationNumber, e.Nationality }, "User_PersonalIdentificationNumber_Nationality")
						.IsUnique()
						.HasFilter("([Nationality] IS NOT NULL AND [PersonalIdentificationNumber] IS NOT NULL)");

				entity.HasIndex(e => e.PhoneNumber, "User_PhoneNumber");

				entity.HasIndex(e => e.RightLegUserId, "User_RightLegUser")
						.IsUnique()
						.HasFilter("([RightLegUserID] IS NOT NULL)");

				entity.HasIndex(e => e.RowNumber, "User_RowNumber");

				entity.HasIndex(e => e.Surname, "User_Surname");

				entity.HasIndex(e => e.UserType, "User_UserType");

				entity.HasIndex(e => e.Username, "User_Username");

				entity.Property(e => e.Id)
						.ValueGeneratedNever()
						.HasColumnName("ID");
				entity.Property(e => e.AccountTypeId).HasDefaultValueSql("((1))");
				entity.Property(e => e.Active).HasDefaultValueSql("((1))");
				entity.Property(e => e.AlternateEmail).HasMaxLength(100);
				entity.Property(e => e.AutoDeleteOn)
						.HasDefaultValueSql("(dateadd(day,(30),getutcdate()))")
						.HasColumnType("datetime");
				entity.Property(e => e.CompletePhoneNumber).HasMaxLength(20);
				entity.Property(e => e.CountryId).HasColumnName("CountryID");
				entity.Property(e => e.CountryOfBirthId).HasColumnName("CountryOfBirthID");
				entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
				entity.Property(e => e.CreatedOn)
						.HasDefaultValueSql("(getutcdate())")
						.HasColumnType("datetime");
				entity.Property(e => e.DiscordPlatform).HasDefaultValueSql("((1))");
				entity.Property(e => e.DroidLanguageId).HasColumnName("DroidLanguageID");
				entity.Property(e => e.EmailPlatform).HasDefaultValueSql("((0))");
				entity.Property(e => e.Fullname).HasMaxLength(200);
				entity.Property(e => e.Gender)
						.IsRequired()
						.HasMaxLength(1)
						.HasDefaultValueSql("(N'N')")
						.IsFixedLength();
				entity.Property(e => e.Givenname).HasMaxLength(100);
				entity.Property(e => e.HasProfilePicture).HasDefaultValueSql("((0))");
				entity.Property(e => e.IOslanguageId).HasColumnName("iOSLanguageID");
				entity.Property(e => e.Investment).HasColumnType("money");
				entity.Property(e => e.InvitedByUserId).HasColumnName("InvitedByUserID");
				entity.Property(e => e.Kyccompleted).HasColumnName("KYCCompleted");
				entity.Property(e => e.LastUserInvitationSentDate).HasColumnType("date");
				entity.Property(e => e.LedgerEndSequenceNumber).HasColumnName("ledger_end_sequence_number");
				entity.Property(e => e.LedgerEndTransactionId).HasColumnName("ledger_end_transaction_id");
				entity.Property(e => e.LedgerStartSequenceNumber).HasColumnName("ledger_start_sequence_number");
				entity.Property(e => e.LedgerStartTransactionId).HasColumnName("ledger_start_transaction_id");
				entity.Property(e => e.LeftLegUserId).HasColumnName("LeftLegUserID");
				entity.Property(e => e.MaxUserAddressesAllowed).HasDefaultValueSql("((100))");
				entity.Property(e => e.MaxUserBankInformationsAllowed).HasDefaultValueSql("((100))");
				entity.Property(e => e.MaxUserInvitationsAllowedPerDay).HasDefaultValueSql("((100))");
				entity.Property(e => e.MaxUserPersonalIdentificationsAllowed).HasDefaultValueSql("((100))");
				entity.Property(e => e.MemberId)
						.IsRequired()
						.HasMaxLength(27)
						.HasComputedColumnSql("(concat([Nationality],'-',[RowNumber]))", false)
						.HasColumnName("MemberID");
				entity.Property(e => e.ModifiedByUserId).HasColumnName("ModifiedByUserID");
				entity.Property(e => e.ModifiedOn)
						.HasDefaultValueSql("(getutcdate())")
						.HasColumnType("datetime");
				entity.Property(e => e.Nationality)
						.HasMaxLength(2)
						.HasDefaultValueSql("(N'NA')")
						.IsFixedLength();
				entity.Property(e => e.OnboardingCompletedOn).HasColumnType("datetime");
				entity.Property(e => e.OtherLanguageId).HasColumnName("OtherLanguageID");
				entity.Property(e => e.PaidRankId).HasColumnName("PaidRankID");
				entity.Property(e => e.ParentUserId).HasColumnName("ParentUserID");
				entity.Property(e => e.PersonalIdentificationNumber).HasMaxLength(50);
				entity.Property(e => e.PersonalIdentificationNumberMasked).HasMaxLength(50);
				entity.Property(e => e.PhoneNumber).HasMaxLength(20);
				entity.Property(e => e.PreferredLanguageId)
						.HasDefaultValueSql("((1))")
						.HasColumnName("PreferredLanguageID");
				entity.Property(e => e.ProfileType).HasDefaultValueSql("((1))");
				entity.Property(e => e.RankId).HasColumnName("RankID");
				entity.Property(e => e.RightLegUserId).HasColumnName("RightLegUserID");
				entity.Property(e => e.RowNumber).ValueGeneratedOnAdd();
				entity.Property(e => e.Surname).HasMaxLength(100);
				entity.Property(e => e.TaxableCountryId).HasColumnName("TaxableCountryID");
				entity.Property(e => e.ThemeDroid)
						.HasMaxLength(1)
						.HasDefaultValueSql("(N'L')")
						.IsFixedLength();
				entity.Property(e => e.ThemeOther)
						.HasMaxLength(1)
						.HasDefaultValueSql("(N'D')")
						.IsFixedLength();
				entity.Property(e => e.ThemeiOs)
						.HasMaxLength(1)
						.HasDefaultValueSql("(N'L')")
						.IsFixedLength()
						.HasColumnName("ThemeiOS");
				entity.Property(e => e.TradingExperience).HasDefaultValueSql("((1))");
				entity.Property(e => e.UserType).HasDefaultValueSql("((1))");
				entity.Property(e => e.Username).HasMaxLength(100);
				entity.Property(e => e.WhatsappPlatform).HasDefaultValueSql("((0))");

				entity.HasOne(d => d.CreatedByUser).WithMany(p => p.InverseCreatedByUser)
						.HasForeignKey(d => d.CreatedByUserId)
						.HasConstraintName("FK_Users_CreatedBy");

				entity.HasOne(d => d.InvitedByUser).WithMany(p => p.InverseInvitedByUser)
						.HasForeignKey(d => d.InvitedByUserId)
						.HasConstraintName("FK_Users_InvitedByUser");

				entity.HasOne(d => d.LeftLegUser).WithOne(p => p.InverseLeftLegUser)
						.HasForeignKey<User>(d => d.LeftLegUserId)
						.HasConstraintName("FK_Users_LeftLegUser");

				entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.InverseModifiedByUser)
						.HasForeignKey(d => d.ModifiedByUserId)
						.HasConstraintName("FK_Users_ModifiedBy");

				entity.HasOne(d => d.ParentUser).WithMany(p => p.InverseParentUser)
						.HasForeignKey(d => d.ParentUserId)
						.HasConstraintName("FK_Users_Parent");

				entity.HasOne(d => d.RightLegUser).WithOne(p => p.InverseRightLegUser)
						.HasForeignKey<User>(d => d.RightLegUserId)
						.HasConstraintName("FK_Users_RightLegUser");

			});


			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

	}
}