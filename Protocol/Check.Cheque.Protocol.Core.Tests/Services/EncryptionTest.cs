using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Check.Cheque.Protocol.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Core.Tests.Services
{
  [TestClass]
  public class EncryptionTest
  {
    [TestMethod]
    public void TestPublicKeyIsUnique()
    {
      var currentKey = string.Empty;
      for (int i = 0; i < 10; i++)
      {
        var key = Encryption.Create();
        var publicKey = key.Export(CngKeyBlobFormat.EccFullPublicBlob);
        var publicKeyString = TryteString.FromBytes(publicKey);

        Assert.AreNotEqual(currentKey, publicKeyString.Value);
        currentKey = publicKeyString.Value;
      }
    }
  }
}
