using System;

namespace NomadCode.Azure
{
	public partial class AzureClient
	{
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
	}
}
