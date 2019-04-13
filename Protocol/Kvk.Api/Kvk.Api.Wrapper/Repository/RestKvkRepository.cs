using System.Net;
using System.Threading.Tasks;
using Kvk.Api.Wrapper.Exception;
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
    public async Task<dynamic> GetByCityAsync(string city)
    {
      var request = new RestRequest($"api/Handelsregister/by-city/{city}", Method.GET);
      request.AddHeader("Authorization", "Token Ax&=^tx&5EM4$5jP");

      var response = await this.Client.ExecuteTaskAsync(request);

      if (response.StatusCode == HttpStatusCode.OK)
      {
        return JsonConvert.DeserializeObject<dynamic>(response.Content);
      }

      throw new ApiException(response);
    }

    /// <inheritdoc />
    public async Task<dynamic> GetByKvkNumberAsync(string kvkNumber)
    {
      var request = new RestRequest($"api/Handelsregister/by-kvknumber/{kvkNumber}", Method.GET);
      request.AddHeader("Authorization", "Token Ax&=^tx&5EM4$5jP");

      var response = await this.Client.ExecuteTaskAsync(request);

      if (response.StatusCode == HttpStatusCode.OK)
      {
        return JsonConvert.DeserializeObject<dynamic>(response.Content);
      }

      throw new ApiException(response);
    }
  }
}