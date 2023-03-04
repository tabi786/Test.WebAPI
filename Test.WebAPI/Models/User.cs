using System;
using System.Collections.Generic;

namespace Test.WebAPI;

public partial class User
{
  public Guid Id { get; set; }

  public long RowNumber { get; set; }

  public string Username { get; set; }

  public string Givenname { get; set; }

  public string Surname { get; set; }

  public string PersonalIdentificationNumber { get; set; }

  public string PersonalIdentificationNumberMasked { get; set; }

  public string AlternateEmail { get; set; }

  public string PhoneNumber { get; set; }

  public string CompletePhoneNumber { get; set; }

  public string Nationality { get; set; }

  public short? CountryId { get; set; }

  public Guid? ParentUserId { get; set; }

  public byte? ParentUserLeg { get; set; }

  public Guid? LeftLegUserId { get; set; }

  public Guid? RightLegUserId { get; set; }

  public string MemberId { get; set; }

  public string Gender { get; set; }

  public bool? Active { get; set; }

  public byte? ProfileType { get; set; }

  public short AccountTypeId { get; set; }

  public int? Level { get; set; }

  public byte? UserType { get; set; }

  public byte? PaidRankId { get; set; }

  public decimal? Investment { get; set; }

  public byte? RankId { get; set; }

  public int? LeftLegActivePoints { get; set; }

  public int? RightLegActivePoints { get; set; }

  public int? LeftLegAccumlatedPoints { get; set; }

  public int? RightLegAccumlatedPoints { get; set; }

  public bool OnboardingCompleted { get; set; }

  public DateTime? OnboardingCompletedOn { get; set; }

  public DateTime? AutoDeleteOn { get; set; }

  public Guid? InvitedByUserId { get; set; }

  public int? LeftLegChildrenCount { get; set; }

  public int? RightLegChildrenCount { get; set; }

  public bool? HasProfilePicture { get; set; }

  public byte? LastAutoLeg { get; set; }

  public byte? MaxUserPersonalIdentificationsAllowed { get; set; }

  public byte? MaxUserAddressesAllowed { get; set; }

  public byte? MaxUserBankInformationsAllowed { get; set; }

  public int? MaxUserInvitationsAllowedPerDay { get; set; }

  public DateTime? LastUserInvitationSentDate { get; set; }

  public bool? EmailPlatform { get; set; }

  public bool? WhatsappPlatform { get; set; }

  public bool? DiscordPlatform { get; set; }

  public string AboutMe { get; set; }

  public short? CountryOfBirthId { get; set; }

  public short? TaxableCountryId { get; set; }

  public short? PreferredLanguageId { get; set; }

  public byte? TradingExperience { get; set; }

  public bool? Kyccompleted { get; set; }

  public bool? FinancialInfoCompleted { get; set; }

  public byte? PrimaryPaymentMethodType { get; set; }

  public long? PrimaryPaymentMethodId { get; set; }

  public short? DroidLanguageId { get; set; }

  public short? IOslanguageId { get; set; }

  public short? OtherLanguageId { get; set; }

  public string ThemeDroid { get; set; }

  public string ThemeiOs { get; set; }

  public string ThemeOther { get; set; }

  public DateTime? CreatedOn { get; set; }

  public Guid? CreatedByUserId { get; set; }

  public DateTime? ModifiedOn { get; set; }

  public Guid? ModifiedByUserId { get; set; }

  public long LedgerStartTransactionId { get; set; }

  public long? LedgerEndTransactionId { get; set; }

  public long LedgerStartSequenceNumber { get; set; }

  public long? LedgerEndSequenceNumber { get; set; }

  public string Fullname { get; set; }

  public virtual User CreatedByUser { get; set; }

  public virtual ICollection<User> InverseCreatedByUser { get; } = new List<User>();

  public virtual ICollection<User> InverseInvitedByUser { get; } = new List<User>();

  public virtual User InverseLeftLegUser { get; set; }

  public virtual ICollection<User> InverseModifiedByUser { get; } = new List<User>();

  public virtual ICollection<User> InverseParentUser { get; } = new List<User>();

  public virtual User InverseRightLegUser { get; set; }

  public virtual User InvitedByUser { get; set; }


  public virtual User LeftLegUser { get; set; }

  public virtual User ModifiedByUser { get; set; }


  public virtual User ParentUser { get; set; }

  public virtual User RightLegUser { get; set; }

}