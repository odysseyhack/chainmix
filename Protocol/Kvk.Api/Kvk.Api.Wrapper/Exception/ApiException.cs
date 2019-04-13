using RestSharp;

namespace Kvk.Api.Wrapper.Exception
{
  public class ApiException : System.Exception
  {
    public ApiException(IRestResponse response)
    {
      Response = response;
    }

    public IRestResponse Response { get; set; }
  }
}