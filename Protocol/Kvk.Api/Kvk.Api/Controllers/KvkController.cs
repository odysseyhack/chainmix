using System.Collections.Generic;
using System.Threading.Tasks;
using Kvk.Api.Wrapper.Repository;
using Kvk.Api.Wrapper.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kvk.Api.Controllers
{
  [ApiController]
  public class KvkController : Controller
  {
    private IKvkRepository Repository { get; }

    public KvkController(IKvkRepository repository)
    {
      this.Repository = repository;
    }

    [HttpGet]
    [Route("api/Handelsregister/by-kvknumber/{kvkNumber}")]
    public async Task<JsonResult> GetByKvkNumberAsync(string kvkNumber)
    {
      return new JsonResult(await this.Repository.GetByKvkNumberAsync(kvkNumber));
    }

    [HttpPost]
    [Route("api/PublicKeys/add/{kvkNumber}")]
    public IActionResult AddPublicKey(string kvkNumber, [FromForm] string publicKey)
    {
      PublicKeyResolver.Add(kvkNumber, publicKey);
      return new OkResult();
    }
  }
}
