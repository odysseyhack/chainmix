using System.Security.Cryptography;

namespace Check.Cheque.Protocol.Core.Services
{
  public static class Encryption
  {
    public static CngKey Create()
    {
      var keyCreationParameters = new CngKeyCreationParameters
      {
        ExportPolicy = CngExportPolicies.AllowPlaintextExport, KeyUsage = CngKeyUsages.Signing
      };

      return CngKey.Create(CngAlgorithm.ECDsaP256, null, keyCreationParameters);
    }

    public static ECDsaCng CreateSignatureScheme(CngKey key)
    {
      return new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha256 };
    }
  }
}