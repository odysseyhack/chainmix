using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kvk.Api.Wrapper.Exception;
using Kvk.Api.Wrapper.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;

namespace Kvk.Api.Wrapper.Tests.Repository
{
  [TestClass]
  public class RestKvkRepositoryTests
  {
    [TestMethod]
    [ExpectedException(typeof(ApiException))]
    public async Task TestClientDoesNotReturnOkResponseOnFindByKvkNumberShouldThrowException()
    {
      var response = new RestResponse {StatusCode = HttpStatusCode.BadRequest};

      var client = new Mock<IRestClient>();
      client.Setup(c => c.ExecuteTaskAsync(It.IsAny<IRestRequest>())).ThrowsAsync(new ApiException(response));

      var repository = new RestKvkRepository(client.Object);
      await repository.GetByKvkNumberAsync("401196200");
    }

    [TestMethod]
    public async Task TestClientReturnsOkResponseOnFindByKvkNumberShouldMapToDynamicType()
    {
      var response = new RestResponse {StatusCode = HttpStatusCode.OK, Content = "{ \"Json\": \"OK\" }"};

      var client = new Mock<IRestClient>();
      client.Setup(c => c.ExecuteTaskAsync(It.IsAny<IRestRequest>())).ReturnsAsync(response);

      var repository = new RestKvkRepository(client.Object);
      var responseJson = await repository.GetByKvkNumberAsync("401196200");

      Assert.AreEqual("OK", responseJson.Values.First().ToString());
    }
  }
}