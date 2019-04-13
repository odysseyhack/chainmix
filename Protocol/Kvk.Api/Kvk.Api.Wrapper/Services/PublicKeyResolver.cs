using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kvk.Api.Wrapper.Services
{
  public static class PublicKeyResolver
  {
    public static Dictionary<string, string> PublicKeyLookup { get; set; }

    static PublicKeyResolver()
    {
      PublicKeyLookup = new Dictionary<string, string>();
    }

    public static void Add(string kvkNumber, string publicKey)
    {
      if (!PublicKeyLookup.ContainsKey(kvkNumber))
      {
        PublicKeyLookup.Add(kvkNumber, publicKey);
      }
    }

    public static string GetPublicKey(string kvkNumber)
    {
      if (!PublicKeyLookup.ContainsKey(kvkNumber))
      {
        return string.Empty;
      }

      return PublicKeyLookup.First(p => p.Key == kvkNumber).Value;
    }
  }
}
