﻿#if __MOBILE__

using System;

#if __IOS__

using Foundation;
using Security;

#elif __ANDROID__

using System.Collections.Generic;

using Java.IO;
using Java.Security;
using Javax.Crypto;

using Android.Content;

#endif

namespace NomadCode.Azure
{
	public partial class AzureClient // Keychain
	{

		class KeychainResult
		{
			public string Account { get; set; }
			public string PrivateKey { get; set; }
			public KeychainResult () { }
			public KeychainResult (string account, string privateKey) { Account = account; PrivateKey = privateKey; }
		}

#if __IOS__

		SecRecord genericRecord (string service) => new SecRecord (SecKind.GenericPassword)
		{
			Service = $"{NSBundle.MainBundle.BundleIdentifier}-{service}"
		};


		KeychainResult getItemFromKeychain (string service)
		{
			SecStatusCode status;

			var record = SecKeyChain.QueryAsRecord (genericRecord (service), out status);

			if (status == SecStatusCode.Success && record != null)
			{
				var account = record.Account;

				var privateKey = NSString.FromData (record.ValueData, NSStringEncoding.UTF8).ToString ();

				return new KeychainResult (account, privateKey);
			}

			return new KeychainResult ();
		}


		bool saveItemToKeychain (string service, string account, string privateKey)
		{
			var record = genericRecord (service);

			record.Account = account;

			record.ValueData = NSData.FromString (privateKey, NSStringEncoding.UTF8);

			// Delete any existing items
			SecKeyChain.Remove (record);

			// Add the new keychain item
			var status = SecKeyChain.Add (record);

			var success = status == SecStatusCode.Success;

			if (!success)
			{
				System.Diagnostics.Debug.WriteLine ($"Error in Keychain: {status}");
				System.Diagnostics.Debug.WriteLine ($"If you are seeing error code '-34018' got to Project Options -> iOS Bundle Signing -> make sure Entitlements.plist is populated for Custom Entitlements for iPhoneSimulator configs");
			}

			return success;
		}


		bool removeItemFromKeychain (string service)
		{
			var record = genericRecord (service);

			var status = SecKeyChain.Remove (record);

			var success = status == SecStatusCode.Success;

			if (!success)
			{
				System.Diagnostics.Debug.WriteLine ($"Error in Keychain: {status}");
				System.Diagnostics.Debug.WriteLine ($"If you are seeing error code '-34018' got to Project Options -> iOS Bundle Signing -> make sure Entitlements.plist is populated for Custom Entitlements for iPhoneSimulator configs");
			}

			return success;
		}

#else

		static Dictionary<string, KeyStore> keyStoresCache = new Dictionary<string, KeyStore> ();


		KeyStore getKeystore (string service)
		{
			var context = Android.App.Application.Context;


			var serviceId = $"{context.PackageName}-{service}";

			KeyStore keystore;

			if (keyStoresCache.TryGetValue (serviceId, out keystore))
			{
				return keystore;
			}

			var password = service.ToCharArray ();

			keystore = KeyStore.GetInstance (KeyStore.DefaultType);

			// var protection = new KeyStore.PasswordProtection (password);

			try
			{
				// TODO: this isn't right, fix it
				using (var stream = context.OpenFileInput (serviceId))
				{
					keystore.Load (stream, password);
				}
			}
			catch (FileNotFoundException)
			{
				keystore.Load (null, password);
			}

			keyStoresCache [serviceId] = keystore;

			return keystore;
		}


		KeychainResult getItemFromKeychain (string service)
		{
			var context = Android.App.Application.Context;

			var password = service.ToCharArray ();

			var protection = new KeyStore.PasswordProtection (password);

			var keystore = getKeystore (service);

			var aliases = keystore.Aliases ();

			while (aliases.HasMoreElements)
			{
				var alias = aliases.NextElement ().ToString ();

				if (keystore.GetEntry (alias, protection) is KeyStore.SecretKeyEntry item)
				{
					var bytes = item.SecretKey.GetEncoded ();

					var serialized = System.Text.Encoding.UTF8.GetString (bytes);

					return new KeychainResult (alias, serialized);
				}
			}

			return new KeychainResult ();
		}


		bool saveItemToKeychain (string service, string account, string privateKey)
		{
			var context = Android.App.Application.Context;

			var password = service.ToCharArray ();

			var serviceId = $"{context.PackageName}-{service}";

			var keystore = getKeystore (service);

			var item = new KeychainItem (privateKey);

			var secretEntry = new KeyStore.SecretKeyEntry (item);

			keystore.SetEntry (account, secretEntry, new KeyStore.PasswordProtection (password));

			using (var stream = context.OpenFileOutput (serviceId, FileCreationMode.Private))
			{
				keystore.Store (stream, password);
			}

			return true;
		}


		bool removeItemFromKeychain (string service)
		{
			return true;
			//throw new NotImplementedException ();
		}


		class KeychainItem : Java.Lang.Object, ISecretKey
		{
			const string raw = "RAW";

			byte [] bytes;

			public KeychainItem (string data)
			{
				if (data == null) throw new ArgumentNullException ();

				bytes = System.Text.Encoding.UTF8.GetBytes (data);
			}

			public byte [] GetEncoded () => bytes;

			public string Algorithm => raw;

			public string Format => raw;
		}

#endif
	}
}
#endif