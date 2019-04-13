using System.Collections.Generic;
using System.Threading.Tasks;
using Kvk.Api.Wrapper.Repository;
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
    [Route("api/Handelsregister/by-city/{city}")]
    public async Task<IActionResult> GetByCityAsync(string city)
    {
      return new JsonResult(await this.Repository.GetByCityAsync(city));
    }

    [HttpGet]
    [Route("api/Handelsregister/by-kvknumber/{kvkNumber}")]
    public async Task<IActionResult> GetByKvkNumberAsync(string kvkNumber)
    {
      return new JsonResult(await this.Repository.GetByKvkNumberAsync(kvkNumber));
    }
  }
}
