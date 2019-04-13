using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Repository;
using Check.Cheque.Protocol.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Check.Cheque.Protocol.Core.Tests.Integration.Repository
{
  [TestClass]
  public class RestKvkRepositoryTests
  {
    [TestMethod]
    public async Task TestPublicKeyCanBePublishedAndIsReadCorrectlyFromResponse()
    {
      var client = new RestClient("https://localhost:44381");
      var repository = new RestKvkRepository(client);

      var key = Encryption.Create();
      await repository.RegisterCompanyPublicKeyAsync("242630600", key);

      var publicKey = await repository.GetCompanyPublicKeyAsync("242630600");

      Assert.IsTrue(publicKey.SequenceEqual(key.Export(CngKeyBlobFormat.EccFullPublicBlob)));
    }
  }
}
