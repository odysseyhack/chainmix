using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Check.Cheque.Protocol.Core.Repository
{
  public interface IKvkRepository
  {
    Task<byte[]> GetCompanyPublicKeyAsync(string kvkNumber);
  }
}
