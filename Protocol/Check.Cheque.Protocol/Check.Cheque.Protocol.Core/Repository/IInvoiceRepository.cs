using System.Security.Cryptography;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Entity;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Repository
{
  public interface IInvoiceRepository
  {
    /// <summary>
    /// Creates and pushes the hash of the given invoice
    /// </summary>
    /// <param name="document">
    /// Invoice converted to bytes
    /// </param>
    /// <returns>
    /// The bundle hash of the published invoice hash.
    /// Needs to be attached to the invoice to be able to find it on the DLT
    /// </returns>
    Task<TryteString> PublishInvoiceHashAsync(byte[] document, CngKey key);

    /// <summary>
    /// Reads the hashed invoide and its signature from the tangle
    /// </summary>
    /// <param name="bundleHash">
    /// Location of the bundle
    /// </param>
    /// <returns>
    /// The parsed payload of the bundle in byte form
    /// </returns>
    Task<InvoicePayload> LoadInvoiceInformationAsync(Hash bundleHash);
  }
}
