using System.Threading.Tasks;

namespace Kvk.Api.Wrapper.Repository
{
  public interface IKvkRepository
  {
    Task<dynamic> GetByCityAsync(string city);
    Task<dynamic> GetByKvkNumberAsync(string kvkNumber);
  }
}