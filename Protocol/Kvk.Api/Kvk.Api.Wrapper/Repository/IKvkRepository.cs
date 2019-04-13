using System.Threading.Tasks;
using RestSharp;

namespace Kvk.Api.Wrapper.Repository
{
  public interface IKvkRepository
  {
    Task<JsonObject> GetByKvkNumberAsync(string kvkNumber);
  }
}