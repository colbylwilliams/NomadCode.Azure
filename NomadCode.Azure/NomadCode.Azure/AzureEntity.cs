//#define JSON_NET

#if __MOBILE__
using System;
using Microsoft.WindowsAzure.MobileServices;

#if JSON_NET
using Newtonsoft.Json;
#endif

#else
using Microsoft.Azure.Mobile.Server;
#endif

namespace NomadCode.Azure
{
	public class AzureEntity
#if !__MOBILE__
	 : EntityData
#endif
	{
#if __MOBILE__

		public string Id { get; set; }

#if JSON_NET
		[JsonProperty (PropertyName = "Deleted")]
#endif
		[Deleted]
		public bool Deleted { get; set; }

#if JSON_NET
		[JsonProperty (PropertyName = "CreatedAt")]
#endif
		[CreatedAt]
		public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

#if JSON_NET
		[JsonProperty (PropertyName = "UpdatedAt")]
#endif
		[UpdatedAt]
		public DateTimeOffset? UpdatedAt { get; set; }

#if JSON_NET
		[JsonProperty (PropertyName = "Version")]
#endif
		[Version]
		public byte [] Version { get; set; }

#if JSON_NET
		[JsonIgnore]
#endif
		public bool HasId => !string.IsNullOrEmpty (Id);

#endif
	}
}