namespace Pact.Palantir.Repository
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Pact.Palantir.Entity;

  using Tangle.Net.Entity;

  /// <summary>
  /// The ContactRepository interface.
  /// </summary>
  public interface IContactRepository
  {
    Task AddContactAsync(string address, bool accepted, string publicKeyAddress);

    Task<ContactInformation> LoadContactInformationByAddressAsync(Address address);

    Task<List<Contact>> LoadContactsAsync(string publicKeyAddress);
  }
}