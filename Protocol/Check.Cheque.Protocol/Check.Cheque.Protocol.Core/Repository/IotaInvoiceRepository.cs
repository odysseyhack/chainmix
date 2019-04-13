using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Entity;
using Check.Cheque.Protocol.Core.Services;
using Tangle.Net.Entity;
using Tangle.Net.Repository;
using Tangle.Net.Utils;

namespace Check.Cheque.Protocol.Core.Repository
{
  public class IotaInvoiceRepository : IInvoiceRepository
  {
    private IIotaRepository IotaRepository { get; }

    public IotaInvoiceRepository(IIotaRepository iotaRepository)
    {
      this.IotaRepository = iotaRepository;
    }

    /// <inheritdoc />
    public async Task<TryteString> PublishInvoiceHashAsync(byte[] document, CngKey key)
    {
      var signatureScheme = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha256 };
      var payload = new InvoicePayload(DocumentHash.Create(document), signatureScheme.SignData(document));

      var bundle = new Bundle();
      bundle.AddTransfer(new Transfer
      {
        Address = new Address(Seed.Random().Value),
        Message = payload.ToTryteString(),
        Timestamp = Timestamp.UnixSecondsTimestamp,
        Tag = new Tag("CHECKCHEQUE")
      });

      bundle.Finalize();
      bundle.Sign();

      await this.IotaRepository.SendTrytesAsync(bundle.Transactions, 2);

      return bundle.Hash;
    }

    /// <inheritdoc />
    public async Task<InvoicePayload> LoadInvoiceInformationAsync(Hash bundleHash)
    {
      var bundleTransactions = await this.IotaRepository.FindTransactionsByBundlesAsync(new List<Hash> {bundleHash});
      var bundle = await this.IotaRepository.GetBundlesAsync(bundleTransactions.Hashes, false);
      var rawPayload = bundle.First().AggregateFragments();

      return InvoicePayload.FromTrytePayload(rawPayload);
    }
  }
}
