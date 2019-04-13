using System.Net;
using System.Threading.Tasks;
using Kvk.Api.Wrapper.Exception;
using Kvk.Api.Wrapper.Services;
using Newtonsoft.Json;
using RestSharp;

namespace Kvk.Api.Wrapper.Repository
{
  public class RestKvkRepository : IKvkRepository
  {
    public RestKvkRepository(IRestClient client)
    {
      Client = client;
    }

    private IRestClient Client { get; }

    /// <inheritdoc />
    public async Task<JsonObject> GetByKvkNumberAsync(string kvkNumber)
    {
      var request = new RestRequest($"api/Handelsregister/by-kvknumber/{kvkNumber}", Method.GET);
      request.AddHeader("Authorization", "Token Ax&=^tx&5EM4$5jP");

      var response = await this.Client.ExecuteTaskAsync(request);

      if (response.StatusCode == HttpStatusCode.OK)
      {
        var result = JsonConvert.DeserializeObject<JsonObject>(response.Content);
        var publicKey = PublicKeyResolver.GetPublicKey(kvkNumber);

        if (!string.IsNullOrEmpty(publicKey))
        {
          result.Add("publicKey", publicKey);
        }

        return result;
      }

      throw new ApiException(response);
    }
  }
}