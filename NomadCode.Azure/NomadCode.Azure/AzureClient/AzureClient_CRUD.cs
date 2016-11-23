//#define OFFLINE_SYNC_ENABLED
#if __MOBILE__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

#if OFFLINE_SYNC_ENABLED

		public async Task SyncAsync<T> ()
			where T : AzureEntity, new()
		{
			await syncAsync (getTable<T> ());
		}

#endif

		public async Task<T> GetAsync<T> (string itemId)
			where T : AzureEntity, new()
		{
			return await lookupItemAsync (getTable<T> (), itemId);
		}


		public async Task<List<T>> GetAsync<T> (Expression<Func<T, bool>> where = null)
			where T : AzureEntity, new()
		{
			return await getAsync (getTable<T> (), where);
		}


		public async Task<T> FirstOrDefault<T> (Expression<Func<T, bool>> where)
			where T : AzureEntity, new()
		{
			return await getFirstOrDefault (getTable<T> (), where);
		}


		public async Task SaveAsync<T> (T item)
			where T : AzureEntity, new()
		{
			await insertOrUpdateAsync (getTable<T> (), item);
		}


		public async Task SaveAsync<T> (List<T> items)
			where T : AzureEntity, new()
		{
			await insertOrUpdateAsync (getTable<T> (), items);
		}


		public async Task DeleteAsync<T> (string itemId)
			where T : AzureEntity, new()
		{
			var table = getTable<T> ();

			var item = await lookupItemAsync (table, itemId);

			await deleteAsync (table, item);
		}


		public async Task DeleteAsync<T> (T item)
			where T : AzureEntity, new()
		{
			await deleteAsync (getTable<T> (), item);
		}


		public async Task DeleteAsync<T> (List<T> items)
			where T : AzureEntity, new()
		{
			await deleteAsync (getTable<T> (), items);
		}


		public async Task DeleteAsync<T> (Expression<Func<T, bool>> where = null)
			where T : AzureEntity, new()
		{
			var table = getTable<T> ();

			var items = await getAsync (table, where);

			await deleteAsync (getTable<T> (), items);
		}
	}
}
#endif