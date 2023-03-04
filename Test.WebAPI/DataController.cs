using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Freedom.Web.Shared;
using Freedom.Web.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OData.ModelBuilder;
using Microsoft.Graph;
using Freedom.Web.Shared.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Freedom.Web.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Stripe.Terminal;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Text;
using Microsoft.Graph.SecurityNamespace;

namespace Freedom.Web.API.Controllers
{

	[Route("api/v1/[controller]")]
	[ApiController]
	public class DataController : ODataController
	{

		private FreedomDataContext freedomDataContext { get; set; }
		private IUserManager UserManager { get; set; }
		public IConfiguration Configuration { get; set; }

		public ILogger _logger;

		public DataController(IUserManager userManager, FreedomDataContext freedomDataContext, IConfiguration configuration, ILogger<DataController> logger)
		{
			this.UserManager = userManager;
			this.freedomDataContext = freedomDataContext;
			Configuration = configuration;
			_logger = logger;
		}


		[HttpGet("{entity}")]
		[EnableQueryWithMetadata]
		public IActionResult Get(string entity)
		{

			_logger.LogInformation($"Getting {entity}");

			switch (entity.ToLower())
			{
				case "platforms":
					return Ok(this.freedomDataContext.Platforms);
				case "countries":
					return Ok(this.freedomDataContext.Countries);
				case "languages":
					return Ok(this.freedomDataContext.Languages);
				case "continents":
					return Ok(this.freedomDataContext.Continents);
				case "currencies":
					return Ok(this.freedomDataContext.Currencies);
				case "users":
					return Ok(this.freedomDataContext.Users);
				case "userpersonalidentifications":
					return Ok(this.freedomDataContext.UserPersonalIdentifications);
				case "useraddresses":
					return Ok(this.freedomDataContext.UserAddresses);
				case "userbankinformations":
					return Ok(this.freedomDataContext.UserBankInformations);
				case "userinvitations":
					return Ok(this.freedomDataContext.UserInvitations); // needs full query with filters api/v1/data/userinvitations?$filter=InvitedUserId eq @UserId&$top=1&$orderby=Id desc&$select=id,givenname,surname,fullname,status,usertype,invitationCode,invitedUserId
				case "usernotifications":
					return Ok(this.freedomDataContext.UserNotifications);
				case "notificationtypes":
					return Ok(this.freedomDataContext.NotificationTypes);
				case "userledgeraccounts":
					return Ok(this.freedomDataContext.LedgerAccounts);
				case "usercreditcards":
					return Ok(this.freedomDataContext.UserCreditCards);
				case "affiliatelevels":
					return Ok(this.freedomDataContext.AffiliateLevels);
				//case "ledgeraccounts_history":
				//	return Ok(this.freedomDataContext.LedgerAccountsHistories);
				case "settingfieldtypes":
					return Ok(this.freedomDataContext.SettingFieldTypes);
				case "settings":
					return Ok(this.freedomDataContext.Settings);
				case "settingsections":
					return Ok(this.freedomDataContext.SettingSections);
				case "settingtypes":
					return Ok(this.freedomDataContext.SettingTypes);
				case "ledgertransactions":
					return Ok(this.freedomDataContext.LedgerTransactions);
				case "uservouchers":
					return Ok(this.freedomDataContext.UserVouchers);
				case "uservouchershares":
					return Ok(this.freedomDataContext.UserVoucherShares);
				case "accounttypes":
					return Ok(this.freedomDataContext.AccountTypes);
				case "brokertypes":
					return Ok(this.freedomDataContext.Brokers);
				case "educations":
					return Ok(this.freedomDataContext.Educations);
				case "employmentandincomesources":
					return Ok(this.freedomDataContext.EmploymentAndIncomeSources);
				case "estimatednetworths":
					return Ok(this.freedomDataContext.EstimatedNetWorths);
				case "estimatedtotalannualincomes":
					return Ok(this.freedomDataContext.EstimatedTotalAnnualIncomes);
				case "investmentamounts":
					return Ok(this.freedomDataContext.InvestmentAmounts);
				case "natureoftransactions":
					return Ok(this.freedomDataContext.NatureOfTransactions);
				case "sourceofinvestmentfunds":
					return Ok(this.freedomDataContext.SourceOfInvestmentFunds);
				case "tradingexperiences":
					return Ok(this.freedomDataContext.TradingExperiences);
				case "termandconditions":
					return Ok(this.freedomDataContext.TermAndConditions);
				case "termandconditiontypes":
					return Ok(this.freedomDataContext.TermAndConditionTypes);		
				case "documenttypes":
					return Ok(this.freedomDataContext.DocumentTypes);	
				case "userdocuments":
					return Ok(this.freedomDataContext.UserDocuments);		
				case "companyaccounttypes":
					return Ok(this.freedomDataContext.CompanyAccountTypes);				
				case "purposeofinvestments":
					return Ok(this.freedomDataContext.PurposeOfInvestments);
				case "userbrokers":
					return Ok(this.freedomDataContext.UserBrokers);
				case "userincomeinformations":
					return Ok(this.freedomDataContext.UserIncomeInformations);
				case "usercompanyinformations":
					return Ok(this.freedomDataContext.UserCompanyInformations);
				case "markets":
					return Ok(this.freedomDataContext.Markets);	
				case "usermarkets":
					return Ok(this.freedomDataContext.UserMarkets);
				case "products":
					return Ok(this.freedomDataContext.Products);
				case "producttypes":
					return Ok(this.freedomDataContext.ProductTypes);
				case "userproducts":
					return Ok(this.freedomDataContext.UserProducts);
				case "subscriptions":
					return Ok(this.freedomDataContext.Subscriptions);
				case "usersubscriptions":
					return Ok(this.freedomDataContext.UserSubscriptions);

				default:
					_logger.LogInformation($"Getting {entity} error. Not Found.");
					return new NotFoundResult();


			}
		}


		[HttpGet("getusertree({userID},{startLevel},{increment})")]
		public IActionResult GetUserTree(
			string userID
			, int startLevel
			, int increment)
		{
			return Ok(Helper.GetUserTree(
				freedomDataContext
				, userID
				, startLevel
				, increment
				, _logger));
		}

		[HttpGet("getmyuservouchers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{status}')")]
		public IActionResult GetMyUserVouchers(
				int pageNumber
				, int rowOfPage
				, string sortingCol
				, string sortType
				, int status
				)
		{
			return Ok(Helper.GetMyUserVouchers(
					Configuration.GetConnectionString("DNS")
					, Helper.GetPrincipalIdentifier(HttpContext)
					, pageNumber
					, rowOfPage
					, sortingCol
					, sortType
					, status
					, string.Empty
					, _logger));
		}

		[HttpGet("searchmyuservouchers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{status}','{search}')")]
		public IActionResult SearchMyUserVouchers(
					int pageNumber
				, int rowOfPage
				, string sortingCol
				, string sortType
				, int status
				, string search
	)
		{
			return Ok(Helper.GetMyUserVouchers(
					Configuration.GetConnectionString("DNS")
					, Helper.GetPrincipalIdentifier(HttpContext)
					, pageNumber
					, rowOfPage
					, sortingCol
					, sortType
					, status
					, search
					, _logger
	));
		}



		[HttpGet("getmyuservouchershares({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{status}')")]
		public IActionResult GetMyUserVoucherShares(
								int pageNumber
								, int rowOfPage
								, string sortingCol
								, string sortType
								, int status
								)
		{
			return Ok(Helper.GetMyUserVoucherShares(
					Configuration.GetConnectionString("DNS")
					, Helper.GetPrincipalIdentifier(HttpContext)
					, pageNumber
					, rowOfPage
					, sortingCol
					, sortType
					, status
					, string.Empty
					, _logger
	));
		}


		[HttpGet("searchmyuservouchershares({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{status}','{search}')")]
		public IActionResult SearchMyUserVoucherShares(
				int pageNumber
				, int rowOfPage
				, string sortingCol
				, string sortType
				, int status
				, string search)
		{
			return Ok(Helper.GetMyUserVoucherShares(
					Configuration.GetConnectionString("DNS")
					, Helper.GetPrincipalIdentifier(HttpContext)
					, pageNumber
					, rowOfPage
					, sortingCol
					, sortType
					, status
					, search
					, _logger));
		}


		[HttpGet("searchusersforvouchershare('{search}')")]
		public IActionResult SearchUsersForVoucherShare(
				string search)
		{
			return Ok(Helper.SearchUsersForVoucherShare(
					Configuration.GetConnectionString("DNS")
					, Helper.GetPrincipalIdentifier(HttpContext)
					, search,
					_logger));
		}


		[HttpGet("getuservoucherscount")]
		public IActionResult GetUserVouchersCount()
		{
			return Ok(Helper.GetUserVouchersCount(
					Configuration.GetConnectionString("DNS")
					, Helper.GetPrincipalIdentifier(HttpContext)
					, _logger
					));
		}

		[HttpGet("getmydistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}')")]
		public IActionResult GetMyDistributorsByUserId(
	int pageNumber
	, int rowOfPage
	, string sortingCol
	, string sortType)
		{
			return Ok(Helper.GetMyDistributors(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, string.Empty
				, _logger));
		}

		[HttpGet("searchmydistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		public IActionResult SearchMyDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType
			, string search)
		{
			return Ok(Helper.GetMyDistributors(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, search
				, _logger));
		}

		[HttpGet("getdistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}')")]
		public IActionResult GetDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType)
		{
			return Ok(Helper.GetDistributors(
				freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, string.Empty, _logger));
		}


		[HttpGet("searchdistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		public IActionResult SearchDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType
			, string search)
		{
			if (!string.IsNullOrEmpty(search))
				search = search.Trim();
			return Ok(Helper.GetDistributors(
				freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, search, _logger));
		}

		[HttpGet("getpendingcustomers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}')")]
		public IActionResult GetPendingCustomers(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType)
		{
			return Ok(Helper.GetPendingCustomers(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, string.Empty, _logger));
		}

		[HttpGet("searchpendingcustomers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		public IActionResult GetPendingCustomers(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType
			, string search)
		{
			return Ok(Helper.GetPendingCustomers(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, search, _logger));
		}

		[HttpGet("getpendingdistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}')")]
		public IActionResult GetPendingDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType)
		{
			return Ok(Helper.GetPendingDistributors(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, string.Empty, _logger));
		}

		[HttpGet("searchpendingdistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		public IActionResult SearchPendingDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType
			, string search)
		{
			return Ok(Helper.GetPendingDistributors(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, search, _logger));
		}

		[HttpGet("getcustomers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}')")]
		public IActionResult GetCustomers(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType)
		{
			return Ok(Helper.GetCustomers(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, string.Empty, _logger));
		}

		[HttpGet("searchcustomers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		public IActionResult SearchCustomers(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType
			, string search)
		{
			return Ok(Helper.GetCustomers(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, search
				, _logger));
		}

		[HttpGet("getfirstlinedistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}')")]
		public IActionResult GetFirstLineDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType)
		{
			return Ok(Helper.GetFirstLineDistributors(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, string.Empty, _logger));
		}

		//[HttpGet("blockpositions({start},{end})")]
		//public IActionResult BlockPositions(
		//	int start
		//	, int end)
		//{
		//	return Ok(Helper.BlockPositions(
		//		Configuration.GetConnectionString("DNS")
		//		, Helper.GetPrincipalIdentifier(HttpContext)
		//		, start
		//		, end));
		//}

		[HttpGet("searchfirstlinedistributors({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		public IActionResult SearchFirstLineDistributors(
			int pageNumber
			, int rowOfPage
			, string sortingCol
			, string sortType
			, string search)
		{
			return Ok(Helper.GetFirstLineDistributors(freedomDataContext
				, Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, pageNumber
				, rowOfPage
				, sortingCol
				, sortType
				, search, _logger));
		}

		/*
		[HttpGet("updateuserparentwithleg('{parentUserId}',{userLegId},'{modifiedByUserId}',{modifiedByAppId}" +
				",'{modifiedByAppName}','{modifiedByAppVersionNumber}','{modifiedByBrowserName}','{modifiedByBrowserVersion}'" +
				",'{modifiedByModel}','{modifiedByManufacturer}','{modifiedByDeviceType}','{modifiedByNetworkType}',{modifiedByPlatformId}" +
				",'{modifiedByPlatformVersion}','{modifiedByLatitude}','{modifiedByLongitude}','{modifiedByCity}','{modifiedByRegion}'" +
				",{modifiedByLanguageID},{modifiedByCountryID},'{modifiedByIP}')")]
		public IActionResult UpdateUserParentWithLeg(
				string parentUserId, byte userLegId, string modifiedByUserId, byte modifiedByAppId, string modifiedByAppName,
				string modifiedByAppVersionNumber, string modifiedByBrowserName, string modifiedByBrowserVersion, string modifiedByModel,
				string modifiedByManufacturer, string modifiedByDeviceType, string modifiedByNetworkType, byte modifiedByPlatformId,
				string modifiedByPlatformVersion, string modifiedByLatitude, string modifiedByLongitude,
				string modifiedByCity, string modifiedByRegion, short modifiedByLanguageID, short modifiedByCountryID, string modifiedByIP)
		{

			if (!string.IsNullOrEmpty(modifiedByAppName))
				modifiedByAppName = modifiedByAppName.Trim();

			if (!string.IsNullOrEmpty(modifiedByAppVersionNumber))
				modifiedByAppVersionNumber = modifiedByAppVersionNumber.Trim();

			if (!string.IsNullOrEmpty(modifiedByBrowserName))
				modifiedByBrowserName = modifiedByBrowserName.Trim();

			if (!string.IsNullOrEmpty(modifiedByBrowserVersion))
				modifiedByBrowserVersion = modifiedByBrowserVersion.Trim();

			if (!string.IsNullOrEmpty(modifiedByModel))
				modifiedByModel = modifiedByModel.Trim();

			if (!string.IsNullOrEmpty(modifiedByManufacturer))
				modifiedByManufacturer = modifiedByManufacturer.Trim();

			if (!string.IsNullOrEmpty(modifiedByDeviceType))
				modifiedByDeviceType = modifiedByDeviceType.Trim();

			if (!string.IsNullOrEmpty(modifiedByNetworkType))
				modifiedByNetworkType = modifiedByNetworkType.Trim();

			if (!string.IsNullOrEmpty(modifiedByPlatformVersion))
				modifiedByPlatformVersion = modifiedByPlatformVersion.Trim();

			if (!string.IsNullOrEmpty(modifiedByLatitude))
				modifiedByLatitude = modifiedByLatitude.Trim();

			if (!string.IsNullOrEmpty(modifiedByLongitude))
				modifiedByLongitude = modifiedByLongitude.Trim();

			if (!string.IsNullOrEmpty(modifiedByCity))
				modifiedByCity = modifiedByCity.Trim();

			if (!string.IsNullOrEmpty(modifiedByRegion))
				modifiedByRegion = modifiedByRegion.Trim();

			if (!string.IsNullOrEmpty(modifiedByIP))
				modifiedByIP = modifiedByIP.Trim();

			return Ok(Helper.UpdateUserParentWithLeg(
				freedomDataContext
				, Helper.GetPrincipalIdentifier(HttpContext)
				, parentUserId
				, userLegId
				, modifiedByUserId
				, modifiedByAppId
				, modifiedByAppName
				, modifiedByAppVersionNumber
				, modifiedByBrowserName
				, modifiedByBrowserVersion
				, modifiedByModel
				, modifiedByManufacturer
				, modifiedByDeviceType
				, modifiedByNetworkType
				, modifiedByPlatformId
				, modifiedByPlatformVersion
				, modifiedByLatitude
				, modifiedByLongitude
				, modifiedByCity
				, modifiedByRegion
				, modifiedByLanguageID
				, modifiedByCountryID
				, modifiedByIP));
		}
		*/

		[HttpGet("personalidentificationnumbervalidate('{personalidentificationNumber}',{nationality})")]
		public IActionResult PersonalIdentificationNumberValidate(
			string personalidentificationNumber
			, string nationality)
		{
			if (!string.IsNullOrEmpty(personalidentificationNumber))
				personalidentificationNumber = personalidentificationNumber.Trim();

			return Ok(Helper.PersonalIdentificationNumberExists(
				freedomDataContext
				, Helper.GetPrincipalIdentifier(HttpContext)
				, personalidentificationNumber
				, nationality
				, Configuration.GetConnectionString("DNS")
				, _logger
				));
		}

		[HttpGet("usernameexists('{username}')")]
		public IActionResult UsernameExists(
			string username)
		{
			if (!string.IsNullOrEmpty(username))
				username = username.Trim();

			return Ok(Helper.UsernameExists(
				username
				, Configuration.GetConnectionString("DNS")
				, _logger
				));
		}

		[HttpGet("invitationexists('{email}','{phonenumber}')")]
		public IActionResult InvitationExists(
			string email
			, string phonenumber)
		{
			if (!string.IsNullOrEmpty(email))
				email = email.Trim();

			if (!string.IsNullOrEmpty(phonenumber))
				phonenumber = phonenumber.Trim();

			return Ok(Helper.InvitationExists(
				email
				, phonenumber
				, Configuration.GetConnectionString("DNS")
				, _logger
				));
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromBody] string userJson)
		{
	var currentPrincipalId = Helper.GetPrincipalIdentifier(HttpContext);
			var currentPrincipalName = await this.UserManager.GetUserPrincipalName(currentPrincipalId);

#if DEBUG
			var storageSection = Configuration.GetSection("StorageQueue").Get<StorageQueueSettings>();
			var serviceBusSettings = Configuration.GetSection("WebServiceBus").Get<WebServiceBusSettings>();
#else
			WebServiceBusSettings serviceBusSettings = new();
			serviceBusSettings.NotificationQueueName = Configuration["NotificationQueueName"];
			serviceBusSettings.NotificationConnectionString = Configuration["NotificationConnectionString"];
			serviceBusSettings.UserAnncesstorQueueName = Configuration["UserAnncesstorQueueName"];
			serviceBusSettings.UserAnncesstorConnectionString = Configuration["UserAnncesstorConnectionString"];
			//serviceBusSettings.TradingQueueName = Configuration["TradingQueueName"];
			//serviceBusSettings.TradingConnectionString = Configuration["TradingConnectionString"];

			StorageQueueSettings storageSection = new();
			storageSection.ClientId = Configuration["DBDecryptClientId"];
			storageSection.ClientSecret = Configuration["DBDecryptClientSecret"];
			storageSection.QueueName = Configuration["BlobQueueName"];
			storageSection.QueueConnectionString = Configuration["BlobConnectionString"];
#endif

			return Ok(Helper.PostData(
				freedomDataContext
				, currentPrincipalName
				, currentPrincipalId
				, Configuration.GetConnectionString("DNS")
				, storageSection
				, serviceBusSettings
				, UserManager
				, userJson
				, _logger));
		}

		[HttpGet("InsertMessageInServiceBus")]
		public async Task<ActionResult> InsertMessageInServiceBus()
		{
			//var currentPrincipalId = Helper.GetPrincipalIdentifier(HttpContext);
			//var currentPrincipalName = await this.UserManager.GetUserPrincipalName(currentPrincipalId);
			var storageSection = Configuration.GetSection("TradingServiceBus").Get<ServiceBusQueueSettings>();
			//RunSQLReturnDataSet
			List<SqlParameter> parameters = new List<SqlParameter>();
			//var value = Database.RunSQLReturnDataTable(
			var reader = Database.RunSQLReturnReader($@"select * from [dbo].[Users] where ID = '2B510E07-3699-4B16-8374-6E3A1933D664'", Configuration.GetConnectionString("DNS"));
			var markets = Database.RunSQLReturnReader($@"select * from [dbo].[UserMarkets] where UserID = '2B510E07-3699-4B16-8374-6E3A1933D664'", Configuration.GetConnectionString("DNS"));
			var brokers = Database.RunSQLReturnReader($@"select * from [dbo].[UserBrokers] where UserID = '2B510E07-3699-4B16-8374-6E3A1933D664'", Configuration.GetConnectionString("DNS"));
			var products = Database.RunSQLReturnReader($@"select * from [dbo].[UserProducts] where UserID = '2B510E07-3699-4B16-8374-6E3A1933D664'", Configuration.GetConnectionString("DNS"));
			TradingUser tuser= new TradingUser();

			//using (var reader = Database.RunSPReturnReader("proc_GetUserDetailByID", sqlConnectionString,
			//		Database.MakeParameter("@ID", SqlDbType.UniqueIdentifier, 16, Guid.Parse(userId))))

			if (reader.HasRows && reader.Read())
			{
				tuser.Id = Database.GetColumnValueGuid(reader, "ID");
				tuser.Username = Database.GetColumnValueString(reader, "Username");
				tuser.ProductType = Database.GetColumnValueString(products, "Name");
				tuser.TotalAccountBalance = Database.GetColumnValueString(products, "Gender");
				tuser.UserMarkets = Database.GetColumnValueString(markets, "Gender");
				tuser.TradingStatus = Database.GetColumnValueString(products, "Gender");
				tuser.Broker = Database.GetColumnValueString(brokers, "Gender");
				tuser.BrokerId = Database.GetColumnValueInteger(brokers, "Gender");
				tuser.BrokerUsername = Database.GetColumnValueString(brokers, "Gender");
				tuser.BrokerPassword = Database.GetColumnValueString(brokers, "Gender");
				tuser.RiskProfile = Database.GetColumnValueString(products, "Gender");
				tuser.MinRiskPercentage = Database.GetColumnValueString(products, "Gender");
				tuser.MaxRiskPercentage = Database.GetColumnValueString(products, "Gender");
				tuser.Balance = Database.GetColumnValueString(products, "Gender");

			}


	var temp = 	Helper.InsertMessageInTradingServiceBusAsync(
					storageSection.TradingConnectionString
				, storageSection.TradingQueueName
				, tuser);

			return	Ok(temp);
		}


		[HttpGet("updateotp('{otp}',{otpType},{systemID},{status})")]
		public async Task<ActionResult> UpdateOTP(
			string otp,
			byte otpType,
			int systemID,
			byte status)
		{
			var storageSection = Configuration.GetSection("StorageQueue").Get<StorageQueueSettings>();
#if MYFREEDOMAZUREPROD
			storageSection.ClientId = Configuration["DBDecryptClientId"];
			storageSection.ClientSecret = Configuration["DBDecryptClientSecret"];
			storageSection.QueueName = Configuration["BlobQueueName"];
			storageSection.QueueConnectionString = Configuration["BlobConnectionString"];
#endif
			return Ok(Helper.UpdateUserOTP(
				Helper.GetPrincipalIdentifier(HttpContext)
				, otp
				, otpType
				, systemID
				, status
				, Configuration.GetConnectionString("DNS")
				, storageSection
				, UserManager
				, freedomDataContext
				, _logger
				));
		}

		[HttpGet("redeemvoucher({serialNumber})")]
		public async Task<ActionResult> RedeemVoucher(Guid serialNumber)
		{
			var storageSection = Configuration.GetSection("StorageQueue").Get<StorageQueueSettings>();
#if MYFREEDOMAZUREPROD
            storageSection.ClientId = Configuration["DBDecryptClientId"];
            storageSection.ClientSecret = Configuration["DBDecryptClientSecret"];
            storageSection.QueueName = Configuration["BlobQueueName"];
            storageSection.QueueConnectionString = Configuration["BlobConnectionString"];
#endif
			return Ok(Helper.RedeemVoucher(
					Helper.GetPrincipalIdentifier(HttpContext)
				, serialNumber
				, Configuration.GetConnectionString("DNS")
				, storageSection
				, freedomDataContext
				, _logger
				));
		}


		//		[HttpGet("sharevoucher({voucherCode},{redeemUserID})")]
		//		public async Task<ActionResult> ShareVoucher(Guid voucherCode, Guid redeemUserID)
		//		{
		//			var storageSection = Configuration.GetSection("StorageQueue").Get<StorageQueueSettings>();
		//#if MYFREEDOMAZUREPROD
		//            storageSection.ClientId = Configuration["DBDecryptClientId"];
		//            storageSection.ClientSecret = Configuration["DBDecryptClientSecret"];
		//            storageSection.QueueName = Configuration["BlobQueueName"];
		//            storageSection.QueueConnectionString = Configuration["BlobConnectionString"];
		//#endif
		//			return Ok(Helper.ShareVoucher(
		//				  Helper.GetPrincipalIdentifier(HttpContext)
		//				, voucherCode
		//				, redeemUserID
		//				, Configuration.GetConnectionString("DNS")
		//				, storageSection
		//				, freedomDataContext
		//				));
		//		}




		[HttpGet("GetAffiliateDashboardData({affiliateType},{graphPeriod},{startDate},{endDate})")]
		public IActionResult GetAffiliateDashboardData(
			AffiliateType affiliateType
			, GraphPeriod graphPeriod
			, DateTime startDate
			, DateTime endDate)
		{
			return Ok(Helper.GetAffiliateDashboardData(
				Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, affiliateType
				, graphPeriod
				, startDate
				, endDate, _logger));
		}

		[HttpGet("GetInvestorsDashboardData({investorType},{graphPeriod},{startDate},{endDate})")]
		public IActionResult GetInvestorsDashboardData(
			InvestorType investorType
			, GraphPeriod graphPeriod
			, DateTime startDate
			, DateTime endDate)
		{
			return Ok(Helper.GetInvestorsDashboardData(
				Configuration.GetConnectionString("DNS")
				, Helper.GetPrincipalIdentifier(HttpContext)
				, investorType
				, graphPeriod
				, startDate
				, endDate, _logger));
		}

		//[HttpGet("getuservouchers({pageNumber},{rowOfPage},'{sortingCol}','{sortType}','{search}')")]
		//public IActionResult GetUserVouchers(
		//		int pageNumber
		//		, int rowOfPage
		//		//, int status
		//		//, bool isVoucherShared
		//		, string sortingCol
		//		, string sortType
		//		, string search)
		//{
		//	return Ok(Helper.GetUserVouchers(freedomDataContext
		//			, Helper.GetPrincipalIdentifier(HttpContext)
		//			, pageNumber
		//			, rowOfPage
		//			//, status
		//			//, isVoucherShared
		//			, sortingCol
		//			, sortType
		//			, search));
		//}



		//[HttpGet("getuserdocuments({documentTypeID})")]
		//public async Task<ActionResult> GetUserDocuments(int documentTypeID)
		//{
		//	var storageSection = Configuration.GetSection("StorageQueue").Get<StorageQueueSettings>();

		//	return Ok(Helper.GetUserDocuments(
		//			Helper.GetPrincipalIdentifier(HttpContext)
		//		,	documentTypeID
		//		, Configuration.GetConnectionString("DNS")
		//		, storageSection
		//		, freedomDataContext
		//		));
		//}






		//[HttpGet("resetuseraccount({amount})")]
		//public async Task<ActionResult> ResetUserAccount(string amount)
		//{
		//	return Ok(Helper.ResetUserAccount(
		//	Helper.GetPrincipalIdentifier(HttpContext),
		//		amount
		//	, Configuration.GetConnectionString("DNS")
		//	, freedomDataContext
		//	));
		//}



		[HttpGet("getalldrowpdownforIndivisual")]
		public IActionResult GetalldrowpdownforIndivisual()
		{
			return Ok(Helper.GetalldrowpdownforIndivisual(
							Configuration.GetConnectionString("DNS"),
							_logger));
		}


		
		[HttpGet("updateuserpoint")]
		public IActionResult UpdateUserPoints()
		{


			//var UserID = "18721116-C6CA-4569-A16A-CE35AAC48F30";			//
			//var UserID = "114EE916-571A-417D-A82E-CCF7F733C907";   // 231
			var UserID = "AA4A7F99-FFAA-40F6-BCEC-C4AC34AFA71C";  // with both legs

			List<SqlParameter> parameters = new List<SqlParameter>();


			parameters.Add(Database.MakeParameter("@Balance", SqlDbType.Money, 8, 50));

			List<string> leftLegUserIds = new List<string>();
			List<string> leftLegUserId = new List<string>();
			List<string> rightLegUserIds = new List<string>();
			List<string> rightLegUserId = new List<string>();
			
			using (var reader = Database.RunSPReturnReader(
				"[proc_GetAncestors]"
					, Configuration.GetConnectionString("DNS")
					, Database.MakeParameter("@UserID", SqlDbType.UniqueIdentifier, 16, Guid.Parse(UserID)))
				)
			{
				if (reader != null && reader.HasRows)
				{
					while (reader.Read())
					{
						if (Database.GetColumnValueTinyInt(reader, "ParentUserLeg").Equals(1))
						{
							leftLegUserIds.Add("'" + Database.GetColumnValueString(reader, "ParentUserId") + "'");
							leftLegUserId.Add(Database.GetColumnValueString(reader, "ParentUserId"));
						}
						else if (Database.GetColumnValueTinyInt(reader, "ParentUserLeg").Equals(2))
						{ 
							rightLegUserIds.Add("'" + Database.GetColumnValueString(reader, "ParentUserId") + "'");
							rightLegUserId.Add(Database.GetColumnValueString(reader, "ParentUserId") );
						}
						}

					StringBuilder sql1Builder = new StringBuilder();

					for (int i = 0; i < leftLegUserIds.Count; i++)
					{
						parameters.Add(Database.MakeParameter($"@UserIdL{i}", SqlDbType.UniqueIdentifier, 16, Guid.Parse(leftLegUserId[i])));
						sql1Builder.AppendLine($@" IF NOT EXISTS (SELECT * FROM [dbo].[UserPoints] 	WHERE [dbo].[UserPoints].[UserId] = @UserIdL{i})");
						sql1Builder.AppendLine($@"BEGIN INSERT INTO UserPoints(UserID,UserType,ParentUserLeg,Level,CurrencyID,Amount,PointCategory,Status,AffiliateLevelID,CommissionPercentage) Values (@UserIdL{i},1,1,1,124,0,1,1,1,1) END ");					
					}
					for (int i = 0; i < rightLegUserIds.Count; i++)
					{
						parameters.Add(Database.MakeParameter($"@UserIdR{i}", SqlDbType.UniqueIdentifier, 16, Guid.Parse(rightLegUserId[i])));
						sql1Builder.AppendLine($@" IF NOT EXISTS (SELECT * FROM [dbo].[UserPoints] 	WHERE [dbo].[UserPoints].[UserId] = @UserIdR{i})");
						sql1Builder.AppendLine($@"BEGIN INSERT INTO UserPoints(UserID,UserType,ParentUserLeg,Level,CurrencyID,Amount,PointCategory,Status,AffiliateLevelID,CommissionPercentage) Values (@UserIdR{i},1,1,2,124,0,1,1,1,1) END ");
					}





					sql1Builder.AppendLine(@$"
	
							BEGIN TRY
							BEGIN TRANSACTION ");

					if (leftLegUserIds.Count > 0)
						sql1Builder.AppendLine(@$"
									UPDATE [dbo].[UserPoints]
									SET [dbo].[UserPoints].[Amount] = COALESCE ([dbo].[UserPoints].[Amount], 0) + @Balance
									WHERE [dbo].[UserPoints].[UserID] IN ({string.Join(",", leftLegUserIds.ToArray())}) ");

					if (rightLegUserIds.Count > 0)
						sql1Builder.AppendLine(@$"
									UPDATE [dbo].[UserPoints]
									SET [dbo].[UserPoints].[Amount] = COALESCE ([dbo].[UserPoints].[Amount], 0) + @Balance
									WHERE [dbo].[UserPoints].[UserID] IN ({string.Join(",", rightLegUserIds.ToArray())}) ");

					sql1Builder.AppendLine(@$" 
							COMMIT
						END TRY
	          BEGIN CATCH
								ROLLBACK
						END CATCH
            ");

					Database.RunSQL(sql1Builder.ToString(), Configuration.GetConnectionString("DNS"), parameters.ToArray());

				}
			}

			return Ok();

		}


		//[HttpGet("updateuserpointforcustomer")]
		//public IActionResult UpdateUserPointsforCustomer()
		//{


		//	//var UserID = "18721116-C6CA-4569-A16A-CE35AAC48F30";			//
		//	var UserID = "114EE916-571A-417D-A82E-CCF7F733C907";   // 231
		//																												 //var UserID = "AA4A7F99-FFAA-40F6-BCEC-C4AC34AFA71C";  // with both legs

		//	//[UserType]

		//	StringBuilder sql1Builder1 = new StringBuilder();
		//	sql1Builder1.AppendLine(@$"select UserType from users where id = '98263580-F3BF-4EB8-993B-078050FF0CBF'");

		//	var userreader = Database.RunSQLReturnReader(sql1Builder1, Configuration.GetConnectionString("DNS"));

		//	var usertype = 1;
		//	if (userreader != null && userreader.HasRows)
		//	{
		//		if (Database.GetColumnValueTinyInt(userreader, "UserType").Equals(2)) 
		//		{
		//				usertype= 2;
		//		}
		//	}			



		//	List<SqlParameter> parameters = new List<SqlParameter>();
		//	parameters.Add(Database.MakeParameter("@Balance", SqlDbType.Money, 8, 50));

		//	List<string> leftLegUserIds = new List<string>();
		//	List<string> leftLegUserId = new List<string>();
		//	List<string> rightLegUserIds = new List<string>();
		//	List<string> rightLegUserId = new List<string>();

		//	using (var reader = Database.RunSPReturnReader(
		//		"[proc_GetAncestors]"
		//			, Configuration.GetConnectionString("DNS")
		//			, Database.MakeParameter("@UserID", SqlDbType.UniqueIdentifier, 16, Guid.Parse(UserID)))
		//		)
		//	{
		//		if (reader != null && reader.HasRows)
		//		{
		//			while (reader.Read())
		//			{
		//				if (Database.GetColumnValueTinyInt(reader, "ParentUserLeg").Equals(1))
		//				{
		//					leftLegUserIds.Add("'" + Database.GetColumnValueString(reader, "ParentUserId") + "'");
		//					leftLegUserId.Add(Database.GetColumnValueString(reader, "ParentUserId"));
		//				}
		//				else if (Database.GetColumnValueTinyInt(reader, "ParentUserLeg").Equals(2))
		//				{
		//					rightLegUserIds.Add("'" + Database.GetColumnValueString(reader, "ParentUserId") + "'");
		//					rightLegUserId.Add(Database.GetColumnValueString(reader, "ParentUserId"));
		//				}
		//			}

		//			StringBuilder sql1Builder = new StringBuilder();

		//			for (int i = 0; i < leftLegUserIds.Count; i++)
		//			{
		//				parameters.Add(Database.MakeParameter($"@UserIdL{i}", SqlDbType.UniqueIdentifier, 16, Guid.Parse(leftLegUserId[i])));
		//				sql1Builder.AppendLine($@" IF NOT EXISTS (SELECT * FROM [dbo].[UserPoints] 	WHERE [dbo].[UserPoints].[UserId] = @UserIdL{i})");
		//				sql1Builder.AppendLine($@"BEGIN INSERT INTO UserPoints(UserID,UserType,ParentUserLeg,Level,CurrencyID,Amount,PointCategory,Status,AffiliateLevelID,CommissionPercentage) Values (@UserIdL{i},1,1,1,124,0,1,1,1,1) END ");
		//			}
		//			for (int i = 0; i < rightLegUserIds.Count; i++)
		//			{
		//				parameters.Add(Database.MakeParameter($"@UserIdR{i}", SqlDbType.UniqueIdentifier, 16, Guid.Parse(rightLegUserId[i])));
		//				sql1Builder.AppendLine($@" IF NOT EXISTS (SELECT * FROM [dbo].[UserPoints] 	WHERE [dbo].[UserPoints].[UserId] = @UserIdR{i})");
		//				sql1Builder.AppendLine($@"BEGIN INSERT INTO UserPoints(UserID,UserType,ParentUserLeg,Level,CurrencyID,Amount,PointCategory,Status,AffiliateLevelID,CommissionPercentage) Values (@UserIdR{i},1,1,2,124,0,1,1,1,1) END ");
		//			}





		//			sql1Builder.AppendLine(@$"
	
		//					BEGIN TRY
		//					BEGIN TRANSACTION ");

		//			if (leftLegUserIds.Count > 0)
		//				sql1Builder.AppendLine(@$"
		//							UPDATE [dbo].[UserPoints]
		//							SET [dbo].[UserPoints].[Amount] = COALESCE ([dbo].[UserPoints].[Amount], 0) + @Balance
		//							WHERE [dbo].[UserPoints].[UserID] IN ({string.Join(",", leftLegUserIds.ToArray())}) ");

		//			if (rightLegUserIds.Count > 0)
		//				sql1Builder.AppendLine(@$"
		//							UPDATE [dbo].[UserPoints]
		//							SET [dbo].[UserPoints].[Amount] = COALESCE ([dbo].[UserPoints].[Amount], 0) + @Balance
		//							WHERE [dbo].[UserPoints].[UserID] IN ({string.Join(",", rightLegUserIds.ToArray())}) ");

		//			sql1Builder.AppendLine(@$" 
		//					COMMIT
		//				END TRY
	 //         BEGIN CATCH
		//						ROLLBACK
		//				END CATCH
  //          ");

		//			Database.RunSQL(sql1Builder.ToString(), Configuration.GetConnectionString("DNS"), parameters.ToArray());

		//		}
		//	}

		//	return Ok();

		//}









		//[HttpDelete("{entity}")]
		//public bool Delete(string entity, int id)
		//{
		//	try
		//	{

		//		switch (entity.ToLower())
		//		{
		//			case "useraddresses":
		//				var UserAddresseEntityInDb = freedomDataContext.UserAddresses.SingleOrDefault(t => t.Id == id);
		//				if (UserAddresseEntityInDb != null)
		//				{
		//					freedomDataContext.UserAddresses.Remove(UserAddresseEntityInDb);
		//					freedomDataContext.SaveChanges();
		//					return true;
		//				}
		//				else
		//				{
		//					return false;
		//				}

		//			case "usercreditcards":
		//				var CreditCardEntityInDb = freedomDataContext.UserCreditCards.SingleOrDefault(t => t.Id == id);
		//				if (CreditCardEntityInDb != null)
		//				{
		//					freedomDataContext.UserCreditCards.Remove(CreditCardEntityInDb);
		//					freedomDataContext.SaveChanges();
		//					return true;
		//				}
		//				else
		//				{
		//					return false;
		//				}

		//			default:
		//				return false;
		//		}
		//	}
		//	catch (System.Exception exception)
		//	{
		//		return false;
		//	}
		//}
	}
}
