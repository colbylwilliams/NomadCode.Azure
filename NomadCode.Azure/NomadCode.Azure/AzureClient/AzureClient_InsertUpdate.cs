//#define OFFLINE_SYNC_ENABLED
#if __MOBILE__

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif


namespace NomadCode.Azure
{
	public partial class AzureClient // Insert & Update
	{

#if OFFLINE_SYNC_ENABLED
		async Task InsertAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task InsertAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#endif
			where T : AzureEntity, new()
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
				await table?.InsertAsync (item);

#if OFFLINE_SYNC_ENABLED
				if (pull)
				{
					Pull (table, where);
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
				LogDebug<T> (e);
				throw;
			}
			finally
			{
				sw.Stop ();
				LogDebug<T> (sw.ElapsedMilliseconds);
#endif
			}
		}


#if OFFLINE_SYNC_ENABLED
		async Task UpdateAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task UpdateAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#endif
			where T : AzureEntity, new()
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
				await table?.UpdateAsync (item);

#if OFFLINE_SYNC_ENABLED
				if (pull)
				{
					Pull (table, where);
				}
#endif
			}
			catch (MobileServicePreconditionFailedException<T> preconditionFailed)
			{
				Log ($"UpdateItemAsync for {typeof (T).Name} failed due to a conflict detected between the local and server version : {(preconditionFailed.InnerException ?? preconditionFailed).Message}");

				// To resolve the conflict, update the version of the item being committed. Otherwise, 
				// it will continue to throw MobileServicePreConditionFailedExceptions

				item.Version = preconditionFailed.Item.Version;

				// Updating recursively here just in case another change happened while the user was making a decision
				await UpdateAsync (table, item);
#if !DEBUG
			}
			catch (Exception)
			{
				throw;
#else
			}
			catch (Exception e)
			{
				LogDebug<T> (e);
				throw;
			}
			finally
			{
				sw.Stop ();
				LogDebug<T> (sw.ElapsedMilliseconds);
#endif
			}
		}


#if OFFLINE_SYNC_ENABLED
		Task InsertOrUpdateAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		Task InsertOrUpdateAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#endif
			where T : AzureEntity, new()
		{
			if (item.UpdatedAt.HasValue)
			{
				return UpdateAsync (table, item, where, pull);
			}

			return InsertAsync (table, item, where, pull);
		}
	}
}
#endif