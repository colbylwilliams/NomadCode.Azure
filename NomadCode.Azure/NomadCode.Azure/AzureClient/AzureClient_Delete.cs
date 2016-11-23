//#define OFFLINE_SYNC_ENABLED
#if __MOBILE__

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
#else
using Microsoft.WindowsAzure.MobileServices;
#endif


namespace NomadCode.Azure
{
	public partial class AzureClient // Delete
	{




#if OFFLINE_SYNC_ENABLED
		async Task deleteAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task deleteAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#endif
			where T : AzureEntity, new()
		{
#if DEBUG
			var sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
#endif
			try
			{
				await table?.DeleteAsync (item);

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
		async Task deleteAsync<T> (IMobileServiceSyncTable<T> table, List<T> items, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task deleteAsync<T> (IMobileServiceTable<T> table, List<T> item, Expression<Func<T, bool>> where = null, bool pull = true)
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
					await table?.DeleteAsync (item);
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