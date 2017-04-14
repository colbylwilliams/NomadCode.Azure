#if __MOBILE__

using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;


namespace NomadCode.Azure
{
	public partial class AzureClient // Authenticate
	{
		public Uri AlternateLoginHost { get; set; }

		public MobileServiceAuthenticationProvider AuthProvider { get; set; } = MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory;

		public event EventHandler<bool> AthorizationChanged;

#if DEBUG
		public bool Authenticated => Initialized && !string.IsNullOrEmpty (Client?.CurrentUser?.MobileServiceAuthenticationToken);
#else
		public bool Authenticated => Initialized && !string.IsNullOrEmpty (Client?.CurrentUser?.MobileServiceAuthenticationToken);
#endif


#if __IOS__
        public async Task<(bool Authenticated, string Sid)> AuthenticateAsync (UIKit.UIViewController view)
#else
		public async Task<(bool Authenticated, string Sid)> AuthenticateAsync (Android.App.Activity view)
#endif
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
#if DEBUG
				// local server can't process auth, so point to the real one
				if (AlternateLoginHost != null)
				{
					Client.AlternateLoginHost = AlternateLoginHost;
				}
#endif
				var creds = getItemFromKeychain (AuthProvider.ToString ());

				if (string.IsNullOrEmpty (creds.Account) || string.IsNullOrEmpty (creds.PrivateKey))
				{
					var user = await Client.LoginAsync (view, AuthProvider);

					if (!string.IsNullOrEmpty (user.UserId) && !string.IsNullOrEmpty (user.MobileServiceAuthenticationToken))
					{
						saveItemToKeychain (AuthProvider.ToString (), user.UserId, user.MobileServiceAuthenticationToken);
					}
				}
				else
				{
					Client.CurrentUser = new MobileServiceUser (creds.Account)
					{
						MobileServiceAuthenticationToken = creds.PrivateKey
					};
				}

				return (Authenticated, (Authenticated ? Client.CurrentUser.UserId : null));
#if !DEBUG
			}
			catch (Exception)
			{
				return (false, null);
#else
			}
			catch (Exception e)
			{
				logDebug ($"AuthenticateAsync failed : {(e.InnerException ?? e).Message}");
				return (false, null);
			}
			finally
			{
				sw.Stop ();
				logDebug ($"AuthenticateAsync took {sw.ElapsedMilliseconds} milliseconds");
#endif
			}
		}


		public async Task<(bool Authenticated, string Sid)> AuthenticateAsync (string token = null, string authorizationCode = null)
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
#if DEBUG
				// local server can't process auth, so point to the real one
				if (AlternateLoginHost != null)
				{
					Client.AlternateLoginHost = AlternateLoginHost;
				}
#endif
				var creds = getItemFromKeychain (AuthProvider.ToString ());

				if (string.IsNullOrEmpty (creds.Account) || string.IsNullOrEmpty (creds.PrivateKey))
				{
					// if token is null, the user is trying to use some stored
					//    credentials - don't bother with trying to auth
					if (!string.IsNullOrEmpty (token))
					{
						var authKeys = getClientAuthKeys ();

						var authNode = string.IsNullOrEmpty (authKeys.AuthKey) || string.IsNullOrEmpty (authorizationCode) ? string.Empty : $", '{authKeys.AuthKey}':'{authorizationCode}'";

						var user = await Client.LoginAsync (AuthProvider, JObject.Parse ($"{{'{authKeys.TokenKey}':'{token}'{authNode}}}"));

						if (!string.IsNullOrEmpty (user.UserId) && !string.IsNullOrEmpty (user.MobileServiceAuthenticationToken))
						{
							saveItemToKeychain (AuthProvider.ToString (), user.UserId, user.MobileServiceAuthenticationToken);
						}
					}
				}
				else
				{
					Client.CurrentUser = new MobileServiceUser (creds.Account)
					{
						MobileServiceAuthenticationToken = creds.PrivateKey
					};
				}

				return (Authenticated, (Authenticated ? Client.CurrentUser.UserId : null));
#if !DEBUG
        	}
        	catch (Exception)
        	{
        		return (false, null);
#else
			}
			catch (Exception e)
			{
				logDebug ($"AuthenticateAsync failed : {(e.InnerException ?? e).Message}");
				return (false, null);
			}
			finally
			{
				sw.Stop ();
				logDebug ($"AuthenticateAsync took {sw.ElapsedMilliseconds} milliseconds");
#endif
			}
		}


		async Task<bool> ReAuthenticateAsync ()
		{
#if DEBUG
			logDebug ("Attempting to ReAuthenticate user...");

			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
#if DEBUG
				// local server can't process auth, so point to the real one
				if (AlternateLoginHost != null)
				{
					Client.AlternateLoginHost = AlternateLoginHost;
				}
#endif
				removeItemFromKeychain (AuthProvider.ToString ());

				var refreshed = await Client.RefreshUserAsync ();

				if (refreshed != null)
				{
					saveItemToKeychain (AuthProvider.ToString (), refreshed.UserId, refreshed.MobileServiceAuthenticationToken);

					return true;
				}

				AthorizationChanged?.Invoke (this, false);

				return false;
#if !DEBUG
            }
            catch (Exception)
            {
                return false;
#else
			}
			catch (Exception e)
			{
				logDebug ($"ReAuthenticateAsync failed : {(e.InnerException ?? e).Message}");
				return false;
			}
			finally
			{
				sw.Stop ();
				logDebug ($"ReAuthenticateAsync took {sw.ElapsedMilliseconds} milliseconds");
#endif
			}
		}


		public async Task LogoutAsync ()
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
				resetPreferenceStore ();

				removeItemFromKeychain (AuthProvider.ToString ());

				await Client.LogoutAsync ();

#if OFFLINE_SYNC_ENABLED

				foreach (var table in tables.Values)
				{
					await table.PurgeAsync ();
				}
#endif

#if !DEBUG
			}
			catch (Exception)
			{
				throw;
#else
			}
			catch (Exception e)
			{
				logDebug ($"LogoutAsync failed : {(e.InnerException ?? e).Message}");
				throw;
			}
			finally
			{
				sw.Stop ();
				logDebug ($"LogoutAsync took {sw.ElapsedMilliseconds} milliseconds");
#endif
			}
		}

		(string TokenKey, string AuthKey) getClientAuthKeys ()
		{
			switch (AuthProvider)
			{
				case MobileServiceAuthenticationProvider.Facebook:
				case MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory:
					return ("access_token", null);
				case MobileServiceAuthenticationProvider.Google:
					return ("id_token", "authorization_code");
				case MobileServiceAuthenticationProvider.Twitter:
					return ("access_token", "access_token_secret");
				case MobileServiceAuthenticationProvider.MicrosoftAccount:
					throw new NotImplementedException ("AuthProvider MobileServiceAuthenticationProvider.MicrosoftAccount does not support client-managed authenticaiton flow.");
				default:
					throw new NotImplementedException ("AuthProvider must be set to Facebook, WindowsAzureActiveDirectory, Google, or Twitter to use client-managed authenticaiton flow.");
			}
		}
	}
}
#endif