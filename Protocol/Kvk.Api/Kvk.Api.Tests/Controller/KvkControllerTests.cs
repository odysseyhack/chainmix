using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kvk.Api.Controllers;
using Kvk.Api.Wrapper.Repository;
using Kvk.Api.Wrapper.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kvk.Api.Tests.Controller
{
  [TestClass]
  public class KvkControllerTests
  {
    [TestMethod]
    public async Task TestGetByKvkNumberReturnsJsonResponse()
    {
      var repository = new Mock<IKvkRepository>();
      repository.Setup(r => r.GetByKvkNumberAsync(It.IsAny<string>())).ReturnsAsync("OK");
      var controller = new KvkController(repository.Object);

      var response = await controller.GetByKvkNumberAsync("123456789");

      Assert.AreEqual("OK", response.Value);
    }

    [TestMethod]
    public void TestAddPublicKey()
    {
      var controller = new KvkController(new Mock<IKvkRepository>().Object);
      controller.AddPublicKey("123456789", "asdfghjkl");

      Assert.AreEqual("asdfghjkl", PublicKeyResolver.GetPublicKey("123456789"));
    }
  }
}
