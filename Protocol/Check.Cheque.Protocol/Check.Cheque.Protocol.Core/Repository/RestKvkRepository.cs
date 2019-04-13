using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Repository
{
  public class RestKvkRepository : IKvkRepository
  {
    private IRestClient Client { get; }

    public RestKvkRepository(IRestClient client)
    {
      this.Client = client;
    }

    /// <inheritdoc />
    public async Task<byte[]> GetCompanyPublicKeyAsync(string kvkNumber)
    {
      var request = new RestRequest($"api/Handelsregister/by-kvknumber/{kvkNumber}", Method.GET);
      var response = await this.Client.ExecuteTaskAsync(request);

      if (response.StatusCode != HttpStatusCode.OK)
      {
        return new byte[0];
      }

      var parsedResponse = JsonConvert.DeserializeObject<JsonObject>(response.Content);
      parsedResponse.TryGetValue("publicKey", out var publicKey);

      return publicKey != null ? new TryteString(publicKey.ToString()).ToBytes() : new byte[0];
    }
  }
}
