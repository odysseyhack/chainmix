using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Services
{
  public static class DocumentHash
  {
    public static byte[] Create(byte[] document)
    {
      using (var sha256Hash = SHA256.Create())
      {
        return sha256Hash.ComputeHash(document);
      }
    }
  }
}
