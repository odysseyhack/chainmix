using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Services
{
  public static class DocumentHash
  {
    public static TryteString Create(byte[] document)
    {
      using (var sha256Hash = SHA256.Create())
      {
        var bytes = sha256Hash.ComputeHash(document);
        return TryteString.FromBytes(bytes);
      }
    }
  }
}
