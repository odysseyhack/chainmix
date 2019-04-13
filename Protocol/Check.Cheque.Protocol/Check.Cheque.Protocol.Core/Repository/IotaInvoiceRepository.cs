using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    public async Task<TryteString> PublishInvoiceHashAsync(byte[] document)
    {
      var address = new Address(Seed.Random().Value);
      var hash = DocumentHash.Create(document);

      var bundle = new Bundle();
      bundle.AddTransfer(new Transfer
      {
        Address = address,
        Message = hash,
        Timestamp = Timestamp.UnixSecondsTimestamp,
        Tag = new Tag("CHECKCHEQUE")
      });

      bundle.Finalize();
      bundle.Sign();

      await this.IotaRepository.SendTrytesAsync(bundle.Transactions, 2);

      return bundle.Hash;
    }
  }
}
