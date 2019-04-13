using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    Task<TryteString> PublishInvoiceHashAsync(byte[] document);
  }
}
