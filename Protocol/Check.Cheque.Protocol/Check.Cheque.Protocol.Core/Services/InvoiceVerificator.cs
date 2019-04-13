using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Entity;
using Check.Cheque.Protocol.Core.Repository;

namespace Check.Cheque.Protocol.Core.Services
{
  public class InvoiceVerificator
  {
    private IInvoiceRepository InvoiceRepository { get; }
    private IKvkRepository KvkRepository { get; }

    public InvoiceVerificator(IInvoiceRepository invoiceRepository, IKvkRepository kvkRepository)
    {
      this.InvoiceRepository = invoiceRepository;
      this.KvkRepository = kvkRepository;
    }

    public async Task<bool> IsValid(Invoice invoice)
    {
      var hashedInvoice = DocumentHash.Create(invoice.Payload);
      var invoiceEntry = await this.InvoiceRepository.LoadInvoiceInformationAsync(invoice.Hash);

      if (!hashedInvoice.SequenceEqual(invoiceEntry.InvoiceHash))
      {
        return false;
      }

      var companyPublicKey = await this.KvkRepository.GetCompanyPublicKeyAsync(invoice.KvkNumber);
      var key = CngKey.Import(companyPublicKey, CngKeyBlobFormat.EccFullPublicBlob);
      var signatureScheme = Encryption.CreateSignatureScheme(key);

      return signatureScheme.VerifyData(invoice.Payload, invoiceEntry.InvoiceSignature);
    }
  }
}
