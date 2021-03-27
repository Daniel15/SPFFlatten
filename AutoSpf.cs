using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SPFFlatten
{
	public class AutoSpf
	{
		private readonly HttpClient _client = new()
		{
			BaseAddress = new Uri("https://api.autospf.com/v1/")
		};

		public AutoSpf(string apiKey)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
		}
		
		public async Task<Domain> GetDomainAsync(string domain)
		{
			return await _client.GetFromJsonAsync<Domain>($"domains/{domain}");
		}
		
		public record Domain
		{
			[JsonPropertyName("flattened_includes")]
			public List<FlattenedInclude> FlattenedIncludes { get; init; }
		}

		public record FlattenedInclude
		{
			[JsonPropertyName("dname")]
			public string DName { get; init; }
			[JsonPropertyName("value")]
			public string Value { get; init; }
		}
	}
}
