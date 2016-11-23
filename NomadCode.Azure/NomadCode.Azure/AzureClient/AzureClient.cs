//#define OFFLINE_SYNC_ENABLED
#if __MOBILE__

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif


namespace NomadCode.Azure
{
	public partial class AzureClient
	{
		const string dbPath = @"nomad.db";

		static string appUri { get; set; }

		static AzureClient _shared;
		public static AzureClient Shared => _shared ?? (_shared = new AzureClient ());


		static MobileServiceClient _client;
		static MobileServiceClient Client => _client ?? (_client = new MobileServiceClient (appUri));



#if OFFLINE_SYNC_ENABLED

		static Dictionary<Type, IMobileServiceSyncTable> tables = new Dictionary<Type, IMobileServiceSyncTable> ();

		IMobileServiceSyncTable<T> getTable<T> ()
			where T : AzureEntity, new()
		{
			if (!Client.SyncContext.IsInitialized)
			{
				throw new Exception ("Client isn't Initialized.  Call RegisterTable on each table then call Initialize before perorming CRUD operations.");
			}

			var type = typeof (T);

			if (!tables.ContainsKey (type))
			{
				var table = Client.GetSyncTable<T> ();

				tables.Add (type, table);

				return table;
			}

			return tables [type] as IMobileServiceSyncTable<T>;
		}

#else

		static Dictionary<Type, IMobileServiceTable> tables = new Dictionary<Type, IMobileServiceTable> ();

		IMobileServiceTable<T> getTable<T> ()
			where T : AzureEntity, new()
		{
			var type = typeof (T);

			if (!tables.ContainsKey (type))
			{
				var table = Client.GetTable<T> ();

				tables.Add (type, table);

				return table;
			}

			return tables [type] as IMobileServiceTable<T>;
		}

#endif

		AzureClient ()
		{
			CurrentPlatform.Init ();
		}


#if OFFLINE_SYNC_ENABLED

		MobileServiceSQLiteStore store;

		public void RegisterTable<T> ()
			where T : AzureEntity
		{
			if (store == null)
			{
				store = new MobileServiceSQLiteStore (dbPath);
			}

			store.DefineTable<T> ();
		}


		public async Task InitializeAzync (string mobileAppUri)
		{
			appUri = mobileAppUri;

			if (!Client.SyncContext.IsInitialized)
			{
				// Uses the default conflict handler, which fails on conflict
				// To use a different conflict handler, pass a parameter to InitializeAsync.
				// For more details, see http://go.microsoft.com/fwlink/?LinkId=521416.
				await Client.SyncContext.InitializeAsync (store);
			}
			else
			{
				throw new Exception ("Client already Initialized.  Call RegisterTable on each table then call Initialize ONCE.");
			}
		}

#endif



#if OFFLINE_SYNC_ENABLED

		public async Task PullAllAsync<T> ()
			where T : AzureEntity, new()
		{
			await PullAsync (getTable<T> ());
		}

#endif

		public async Task<List<T>> GetAllAsync<T> ()
			where T : AzureEntity, new()
		{
			return await GetAsync (getTable<T> ());
		}


		public async Task SaveAsync<T> (T item)
			where T : AzureEntity, new()
		{
			await InsertOrUpdateAsync (getTable<T> (), item);
		}


		public async Task SaveAllAsync<T> (List<T> items)
			where T : AzureEntity, new()
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			var table = getTable<T> ();

			foreach (var item in items)
			{
				await InsertOrUpdateAsync (table, item, null, false);
			}

#if OFFLINE_SYNC_ENABLED
			Pull (table);
#endif

#if DEBUG
			sw.Stop ();
			LogDebug<T> (sw.ElapsedMilliseconds);
#endif
		}


#region Log Utility

		void Log (string message) => Console.WriteLine (message);

#if DEBUG
		void LogDebug (string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{GetType ().Name}] [{memberName}] [{sourceLineNumber}] :: {message}");
		}

		void LogDebug<T> (Exception ex, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			var message = $"{memberName} for type '{typeof (T).Name}' failed with error: {(ex.InnerException ?? ex).Message}";

			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{GetType ().Name}] [{memberName}] [{sourceLineNumber}] :: {message}");
		}

		void LogDebug<T> (long ms, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			var message = $"{memberName} for {typeof (T).Name} took {ms} milliseconds";

			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{GetType ().Name}] [{memberName}] [{sourceLineNumber}] :: {message}");
		}
#endif

#endregion
	}
}
#endif