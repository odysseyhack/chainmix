using System.Net;
using System.Security.Cryptography;
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

    /// <inheritdoc />
    public async Task RegisterCompanyPublicKeyAsync(string kvkNumber, CngKey key)
    {
      var publicKey = key.Export(CngKeyBlobFormat.EccFullPublicBlob);
      var publicKeyPayload = TryteString.FromBytes(publicKey);

      var request = new RestRequest($"api/PublicKeys/add/{kvkNumber}", Method.POST);
      request.AddParameter("publicKey", publicKeyPayload.Value);

      await this.Client.ExecuteTaskAsync(request);
    }
  }
}
