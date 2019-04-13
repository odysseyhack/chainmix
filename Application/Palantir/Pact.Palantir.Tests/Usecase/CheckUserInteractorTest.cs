namespace Pact.Palantir.Tests.Usecase
{
  using System;
  using System.Threading.Tasks;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Pact.Palantir.Exception;
  using Pact.Palantir.Tests.Repository;
  using Pact.Palantir.Tests.Service;
  using Pact.Palantir.Usecase;
  using Pact.Palantir.Usecase.CheckUser;

  using Tangle.Net.Entity;

  [TestClass]
  public class CheckUserInteractorTest
  {
    [DataTestMethod]
    [DataRow(typeof(Exception), ResponseCode.UnkownException)]
    [DataRow(typeof(MessengerException), ResponseCode.MessengerException)]
    public async Task TestExceptionIsHandledProperly(Type exceptionType, ResponseCode expectedCode)
    {
      var exception = exceptionType == typeof(MessengerException) ? new MessengerException(expectedCode) : new Exception();
      var interactor = new CheckUserInteractor(
        new ExceptionContactRepository(new MessengerException(ResponseCode.NoContactInformationPresent)),
        new ExceptionMessenger(exception),
        new InMemoryAddressGenerator(),
        new SignatureGeneratorStub());

      var response = await interactor.ExecuteAsync(
                       new CheckUserRequest
                         {
                           PublicKey = InMemoryContactRepository.NtruKeyPair.PublicKey,
                           PublicKeyAddress = new Address(Hash.Empty.Value),
                           RequestAddress = new Address(Hash.Empty.Value),
                           Seed = Seed.Random()
                         });

      Assert.AreEqual(expectedCode, response.Code);
    }

    [TestMethod]
    public async Task TestContactInformationDoesExistShouldReturnCodeSuccess()
    {
      var messenger = new InMemoryMessenger();
      var interactor = new CheckUserInteractor(
        new InMemoryContactRepository(),
        messenger,
        new InMemoryAddressGenerator(),
        new SignatureGeneratorStub());

      var response = await interactor.ExecuteAsync(
                       new CheckUserRequest
                         {
                           PublicKey = InMemoryContactRepository.NtruKeyPair.PublicKey,
                           PublicKeyAddress = new Address(Hash.Empty.Value),
                           RequestAddress = new Address(Hash.Empty.Value),
                           Seed = Seed.Random()
                         });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(0, messenger.SentMessages.Count);
    }

    [TestMethod]
    public async Task TestContactInformationDoesNotExistAnymoreShouldResend()
    {
      var messenger = new InMemoryMessenger();
      var interactor = new CheckUserInteractor(
        new ExceptionContactRepository(new MessengerException(ResponseCode.NoContactInformationPresent)),
        messenger,
        new InMemoryAddressGenerator(),
        new SignatureGeneratorStub());

      var response = await interactor.ExecuteAsync(
                       new CheckUserRequest
                         {
                           PublicKey = InMemoryContactRepository.NtruKeyPair.PublicKey,
                           PublicKeyAddress = new Address(Hash.Empty.Value),
                           RequestAddress = new Address(Hash.Empty.Value),
                           Seed = Seed.Random()
                         });

      Assert.AreEqual(ResponseCode.Success, response.Code);
      Assert.AreEqual(1, messenger.SentMessages.Count);
    }
  }
}