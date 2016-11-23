//#define OFFLINE_SYNC_ENABLED
#if __MOBILE__

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
#else
using Microsoft.WindowsAzure.MobileServices;
#endif


namespace NomadCode.Azure
{
	public partial class AzureClient // Delete
	{

#if OFFLINE_SYNC_ENABLED
		async Task DeleteAsync<T> (IMobileServiceSyncTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
#else
		async Task DeleteAsync<T> (IMobileServiceTable<T> table, T item, Expression<Func<T, bool>> where = null, bool pull = true)
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
	}
}
#endif