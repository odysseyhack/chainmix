namespace Chiota.Messenger.Cucumber.Repository
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Pact.Palantir.Entity;
  using Pact.Palantir.Repository;
  using Pact.Palantir.Service;

  using Tangle.Net.Cryptography.Signing;

  public class EmptyContactRepository : AbstractTangleContactRepository
  {
    /// <inheritdoc />
    public EmptyContactRepository(IMessenger messenger, ISignatureValidator signatureValidator)
      : base(messenger, signatureValidator)
    {
    }

    /// <inheritdoc />
    public async override Task AddContactAsync(string address, bool accepted, string publicKeyAddress)
    {
    }

    /// <inheritdoc />
    public async override Task<List<Contact>> LoadContactsAsync(string publicKeyAddress)
    {
      return new List<Contact>();
    }
  }
}