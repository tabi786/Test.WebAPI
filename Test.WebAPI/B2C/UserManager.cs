using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Azure.Storage.Blobs;
using System.Threading;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections;

namespace Test.WebAPI
{
	public class UserManager : IUserManager
	{
		private readonly GraphServiceClient _graphServiceClient = null;
		private readonly B2CUserSettings userSettings;

		//public UserManagement() { }
		public UserManager(IOptions<B2CUserSettings> userSettings)
		{
			// The client_id, client_secret, and tenant are pulled in from the appsettings.json from coach API
			this.userSettings = userSettings.Value;

			if (_graphServiceClient == null)
			{
				_graphServiceClient = new GraphServiceClient(new ClientSecretCredential(
						this.userSettings.tenant,
						this.userSettings.clientId,
						this.userSettings.clientSecret,
						new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud }));
			}
		}

		public async Task<Result> CheckIfUserEmailOrPhoneNumberAlreadyExists(string phoneNumber, string emailAddress)
		{
			try
			{
				Result result = new Result();

				B2cCustomAttributeHelper helper = new B2cCustomAttributeHelper(this.userSettings.B2cExtensionAppClientId);
				const string customAttributeName4 = "UserEmailAddress";
				const string customAttributeName5 = "phoneNumber";
				string CustomUserEmailAddress = helper.GetCompleteAttributeName(customAttributeName4);
				string CustomphoneNumber = helper.GetCompleteAttributeName(customAttributeName5);
				var checkIfUserEmailOrPhoneAlreadyExists = await this._graphServiceClient.Users.Request().Filter($"{CustomUserEmailAddress} eq '{emailAddress}' or {CustomphoneNumber} eq '{phoneNumber}'").Select($"id,Mail,MobilePhone").GetAsync();
				if (checkIfUserEmailOrPhoneAlreadyExists != null && checkIfUserEmailOrPhoneAlreadyExists.CurrentPage != null && checkIfUserEmailOrPhoneAlreadyExists.CurrentPage.Count > 0)
				{
					result.Value = null;
					result.IsSucess = true;
					if (checkIfUserEmailOrPhoneAlreadyExists.CurrentPage[0].MobilePhone == phoneNumber && checkIfUserEmailOrPhoneAlreadyExists.CurrentPage[0].Mail == emailAddress)
						result.Message = Config.EmailMobileNumberAlreadyExists;
					else if (checkIfUserEmailOrPhoneAlreadyExists.CurrentPage[0].MobilePhone == phoneNumber)
						result.Message = Config.MobileNumberAlreadyExists;
					else if (checkIfUserEmailOrPhoneAlreadyExists.CurrentPage[0].Mail == emailAddress)
						result.Message = Config.EmailAlreadyExists;

					return result;
				}
				else
				{
					result.IsSucess = true;
					result.Message = Config.NotExists;
					return result;
				}
			}
			catch (Exception) { throw; }
		}

		public async Task<string> GetUserByEmail(string email)
		{
			try
			{
				const string customAttributeName1 = "AppLanguage";
				const string customAttributeName2 = "Gender";
				const string customAttributeName3 = "DateOfBirth";
				const string customAttributeName4 = "UserEmailAddress";

				// Get the complete name of the custom attribute (Azure AD extension)
				B2cCustomAttributeHelper helper = new B2cCustomAttributeHelper(this.userSettings.B2cExtensionAppClientId);
				string AppLanguage = helper.GetCompleteAttributeName(customAttributeName1);
				string Gender = helper.GetCompleteAttributeName(customAttributeName2);
				string DateOfBirth = helper.GetCompleteAttributeName(customAttributeName3);
				string UserEmailAddress = helper.GetCompleteAttributeName(customAttributeName4);
				B2CUser b2CUser = new B2CUser();
				// Get user by sign-in name
				var user = await this._graphServiceClient.Users
						.Request()
						.Filter($"identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{this.userSettings.tenant}')")
						.Select($"id,MobilePhone,Mail")
						.GetAsync();

				//{  var a = await graphClient.Users
				//.Request()
				//.GetAsync();

				// var b = a.CurrentPage.ToList();}

				//DataServiceStreamResponse photo = await user.ThumbnailPhoto.DownloadAsync();
				if (user.CurrentPage != null && user.CurrentPage.Count > 0)
				{
					return user.CurrentPage[0].Id;
				}
				else
					return string.Empty;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<bool> DeleteUser(string email)
		{
			var user = await this.GetUserByEmail(email);

			try
			{
				return true;
			}
			catch //(Exception ex)
			{
				return false;
			}
		}

		public async Task<string> GetUserPrincipalName(string Id)
		{
			try
			{
				string userName = null;
				var result = await _graphServiceClient.Users[Id]
						.Request()
						.Select(o => new { o.Identities })
						.GetAsync();

				userName = result.UserPrincipalName;

				try
				{
					userName = result.Identities.FirstOrDefault().IssuerAssignedId;
				}
				catch
				{
				}

				return userName;
			}
			catch (Exception)
			{
				throw;
			}
		}

	}
}
