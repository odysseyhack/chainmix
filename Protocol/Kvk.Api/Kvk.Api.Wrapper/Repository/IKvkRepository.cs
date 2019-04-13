using System.Threading.Tasks;

namespace Kvk.Api.Wrapper.Repository
{
  public interface IKvkRepository
  {
    Task<dynamic> GetByKvkNumberAsync(string kvkNumber);
  }
}