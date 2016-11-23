//#define OFFLINE_SYNC_ENABLED
#if __MOBILE__

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
#endif


namespace NomadCode.Azure
{
	public partial class AzureClient // Insert & Update
	{

#if OFFLINE_SYNC_ENABLED
		async Task insertAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task insertAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
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
					sync (table, where);
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
		async Task updateAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task updateAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
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
					sync (table, where);
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
				await updateAsync (table, item);
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
		Task insertOrUpdateAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		Task insertOrUpdateAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#endif
			where T : AzureEntity, new()
		{
			if (item.UpdatedAt.HasValue)
			{
				return updateAsync (table, item, where, pull);
			}

			return insertAsync (table, item, where, pull);
		}



#if OFFLINE_SYNC_ENABLED
		async Task insertOrUpdateAsync<T> (IMobileServiceSyncTable<T> table, List<T> items, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task insertOrUpdateAsync<T> (IMobileServiceTable<T> table, List<T> items, Expression<Func<T, bool>> where = null, bool pull = true)
#endif
			where T : AzureEntity, new()
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
				foreach (var item in items)
				{
					if (item.HasId && item.UpdatedAt.HasValue)
					{
						await updateAsync (table, item, null, false);
					}
					else
					{
						await insertAsync (table, item, null, false);
					}
				}
#if OFFLINE_SYNC_ENABLED
				if (pull)
				{
					sync (table, where);
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
	}
}
#endif