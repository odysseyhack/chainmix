using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Entity
{
  public class Invoice
  {
    public Hash Hash { get; set; }
    public byte[] Payload { get; set; }
    public string KvkNumber { get; set; }
  }
}