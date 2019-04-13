using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Entity;
using Check.Cheque.Protocol.Core.Repository;
using Check.Cheque.Protocol.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Tests.Unit.Services
{
  [TestClass]
  public class InvoiceVerificatorTests
  {
    [TestMethod]
    public async Task TestInvoiceHashMismatchShouldReturnFalse()
    {
      var dltPayload = new InvoicePayload(Encoding.UTF8.GetBytes("Somebody once told me"),
        Encoding.UTF8.GetBytes("the world is gonna roll me"));

      var invoiceRepository = new Mock<IInvoiceRepository>();
      invoiceRepository.Setup(i => i.LoadInvoiceInformationAsync(It.IsAny<Hash>())).ReturnsAsync(dltPayload);

      var verificator = new InvoiceVerificator(invoiceRepository.Object, new Mock<IKvkRepository>().Object);
      Assert.IsFalse(await verificator.IsValid(new Invoice
        {Hash = Hash.Empty, KvkNumber = "123456789", Payload = Encoding.UTF8.GetBytes("Somebody once")}));
    }

    [TestMethod]
    public async Task TestSignatureMismatchShouldReturnFalse()
    {
      var invoice = new Invoice
        { Hash = Hash.Empty, KvkNumber = "123456789", Payload = Encoding.UTF8.GetBytes("Somebody once told me") };
      var signatureScheme = Encryption.CreateSignatureScheme(Encryption.Create());

      var dltPayload = new InvoicePayload(DocumentHash.Create(invoice.Payload),
        signatureScheme.SignData(Encoding.UTF8.GetBytes("Somebody once told me the world is gonna roll me")));

      var invoiceRepository = new Mock<IInvoiceRepository>();
      invoiceRepository.Setup(i => i.LoadInvoiceInformationAsync(It.IsAny<Hash>())).ReturnsAsync(dltPayload);

      var kvkRepository = new Mock<IKvkRepository>();
      kvkRepository.Setup(k => k.GetCompanyPublicKeyAsync(It.IsAny<string>()))
        .ReturnsAsync(signatureScheme.Key.Export(CngKeyBlobFormat.EccFullPublicBlob));

      var verificator = new InvoiceVerificator(invoiceRepository.Object, kvkRepository.Object);

      Assert.IsFalse(await verificator.IsValid(invoice));
    }

    [TestMethod]
    public async Task TestSignatureMatchShouldReturnTrue()
    {
      var invoice = new Invoice
        { Hash = Hash.Empty, KvkNumber = "123456789", Payload = Encoding.UTF8.GetBytes("Somebody once told me") };
      var signatureScheme = Encryption.CreateSignatureScheme(Encryption.Create());

      var dltPayload = new InvoicePayload(DocumentHash.Create(invoice.Payload),
        signatureScheme.SignData(invoice.Payload));

      var invoiceRepository = new Mock<IInvoiceRepository>();
      invoiceRepository.Setup(i => i.LoadInvoiceInformationAsync(It.IsAny<Hash>())).ReturnsAsync(dltPayload);

      var kvkRepository = new Mock<IKvkRepository>();
      kvkRepository.Setup(k => k.GetCompanyPublicKeyAsync(It.IsAny<string>()))
        .ReturnsAsync(signatureScheme.Key.Export(CngKeyBlobFormat.EccFullPublicBlob));

      var verificator = new InvoiceVerificator(invoiceRepository.Object, kvkRepository.Object);

      Assert.IsTrue(await verificator.IsValid(invoice));
    }
  }
}
