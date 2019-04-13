using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Entity
{
  public class InvoicePayload
  {
    public InvoicePayload(byte[] invoiceHash, byte[] invoiceSignature)
    {
      InvoiceHash = invoiceHash;
      InvoiceSignature = invoiceSignature;
    }

    public byte[] InvoiceHash { get; }
    public byte[] InvoiceSignature { get; }

    public TryteString ToTryteString()
    {
      return TryteString.FromBytes(InvoiceHash).Concat(TryteString.FromBytes(InvoiceSignature));
    }

    public static InvoicePayload FromTrytePayload(TryteString trytes)
    {
      return new InvoicePayload(trytes.GetChunk(0, 64).ToBytes(), trytes.GetChunk(64, 128).ToBytes());
    }
  }
}