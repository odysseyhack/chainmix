using System;
using System.Collections.Generic;
using System.Text;
using Kvk.Api.Wrapper.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kvk.Api.Wrapper.Tests.Services
{
  [TestClass]
  public class PublicKeyResolverTests
  {
    [TestMethod]
    public void TestAddingKeyToLookupShouldAddIfNonExistantAndIgnoreIfAlreadyExistant()
    {
      PublicKeyResolver.Add("123456789", "asdfghjkl");
      Assert.AreEqual(1, PublicKeyResolver.PublicKeyLookup.Count);

      PublicKeyResolver.Add("123456789", "asdfghjkl");
      Assert.AreEqual(1, PublicKeyResolver.PublicKeyLookup.Count);
    }

    [TestMethod]
    public void TestReadingKeyShouldReturnKeyIfFound()
    {
      PublicKeyResolver.Add("123456789", "asdfghjkl");
      Assert.AreEqual("asdfghjkl", PublicKeyResolver.GetPublicKey("123456789"));
    }
  }
}
