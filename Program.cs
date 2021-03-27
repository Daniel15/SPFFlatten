using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PowerDns.Client;
using PowerDns.Client.Models;
using TypedRest.Endpoints.Generic;

namespace SPFFlatten
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var configFile = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

			var config = configFile.Get<Config>();

			var autoSpfData = await new AutoSpf(config.AutoSpfApiKey)
				.GetDomainAsync(config.AutoSpfDomain);

			var powerDns = new PowerDnsClient(config.PowerDnsServer, config.PowerDnsApiKey);
			var zoneEndpoint = powerDns.Servers["localhost"].Zones[config.DestinationZone];
			var zone = await zoneEndpoint.ReadAsync();

			foreach (var flattenedInclude in autoSpfData.FlattenedIncludes)
			{
				await UpdateRecord(flattenedInclude, config, zone, zoneEndpoint);
			}
		}

		private static async Task UpdateRecord(
			AutoSpf.FlattenedInclude flattenedInclude,
			Config config,
			Zone zone,
			IElementEndpoint<Zone> zoneEndpoint
		)
		{
			var name = flattenedInclude.DName.Replace(config.AutoSpfDomain, config.DestinationZone);
			var newValue = flattenedInclude.Value.Replace(config.AutoSpfDomain, config.DestinationZone);
			if (!newValue.EndsWith("\""))
			{
				newValue += "\"";
			}

			if (!newValue.StartsWith("\""))
			{
				newValue = "\"" + newValue;
			}

			var existingRecord = zone.RecordSets.FirstOrDefault(x => x.Name.Equals(name));
			if (existingRecord != null)
			{
				var needsUpdate =
					existingRecord.Records.Count != 1 ||
					existingRecord.Records[0].Content != newValue;
				if (needsUpdate)
				{
					Console.WriteLine($"Updating {name}");
					Console.WriteLine($"{newValue}");

					existingRecord.ChangeType = ChangeType.Replace;
					existingRecord.Records.Clear();
					existingRecord.Records.Add(new Record(newValue));
					await zoneEndpoint.PatchRecordSetAsync(existingRecord);
					Console.WriteLine("Done.");
					Console.WriteLine();
				}
				else
				{
					Console.WriteLine($"{name} is already up-to-date");
				}
			}
			else
			{
				Console.WriteLine($"Creating {name}");
				Console.WriteLine($"{newValue}");

				var record = new RecordSet
				{
					ChangeType = ChangeType.Replace,
					Name = name,
					Records = new List<Record> {new(newValue)},
					Ttl = 60,
					Type = RecordType.TXT,
				};
				await zoneEndpoint.PatchRecordSetAsync(record);
				Console.WriteLine("Done.");
				Console.WriteLine();
			}
		}
	}
}
