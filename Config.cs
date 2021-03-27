using System;

namespace SPFFlatten
{
	public record Config
	{
		public string AutoSpfApiKey { get; init; }
		public string AutoSpfDomain { get; init; }
		public string DestinationZone { get; init; }
		public Uri PowerDnsServer { get; init; }
		public string PowerDnsApiKey { get; init; }
	}
}
