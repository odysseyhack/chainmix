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
      else
      {
        PublicKeyLookup.Remove(kvkNumber);
        PublicKeyLookup.Add(kvkNumber, publicKey);
      }
    }

    public static string GetPublicKey(string kvkNumber)
    {
      return !PublicKeyLookup.ContainsKey(kvkNumber)
        ? string.Empty
        : PublicKeyLookup.First(p => p.Key == kvkNumber).Value;
    }

    public static dynamic AddPublicKeyToCompany(dynamic parsedJson)
    {
      return parsedJson;
    }
  }
}
