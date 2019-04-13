using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Repository;
using Check.Cheque.Protocol.Core.Services;
using Check.Cheque.Protocol.Image.Recognition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Tangle.Net.Entity;
using Tangle.Net.ProofOfWork.Service;
using Tangle.Net.Repository;
using Tangle.Net.Repository.Client;

namespace Check.Cheque.Protocol.Integration.Tests
{
  [TestClass]
  public class FlowTest
  {
    [TestMethod]
    public async Task SetupAsync()
    {
      var invoiceRepository = new IotaInvoiceRepository(IotaRepository);
      var kvkRepository = new RestKvkRepository(new RestClient("https://localhost:44381"));

      var parsedInvoice = await ImageParser.Parse("C:\\Projects\\Odyssey\\IMG_20190413_192812.jpg");

      var invoiceVerificator = new InvoiceVerificator(invoiceRepository, kvkRepository);
      var result = await invoiceVerificator.IsValid(parsedInvoice);
    }

    private static RestIotaRepository IotaRepository
    {
      get
      {
        var iotaRepository = new RestIotaRepository(
          new FallbackIotaClient(
            new List<string>
            {
              "https://trinity.iota-tangle.io:14265",
              "https://nodes.thetangle.org:443",
              "http://iota1.heidger.eu:14265",
              "https://nodes.iota.cafe:443",
              "https://potato.iotasalad.org:14265",
              "https://durian.iotasalad.org:14265",
              "https://turnip.iotasalad.org:14265",
              "https://nodes.iota.fm:443",
              "https://tuna.iotasalad.org:14265",
              "https://iotanode2.jlld.at:443",
              "https://node.iota.moe:443",
              "https://wallet1.iota.town:443",
              "https://wallet2.iota.town:443",
              "http://node03.iotatoken.nl:15265",
              "https://node.iota-tangle.io:14265",
              "https://pow4.iota.community:443",
              "https://dyn.tangle-nodes.com:443",
              "https://pow5.iota.community:443",
              "http://node04.iotatoken.nl:14265",
              "http://node05.iotatoken.nl:16265",
            },
            5000),
          new PoWSrvService());
        return iotaRepository;
      }
    }
  }
}
