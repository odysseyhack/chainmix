namespace Pact.Palantir.Usecase.GetContacts
{
  using System.Collections.Generic;

  using Pact.Palantir.Entity;

  /// <summary>
  /// The get approved contacts response.
  /// </summary>
  public class GetContactsResponse : BaseResponse
  {
    /// <summary>
    /// All contacts that have been accepted. See Entities for more information about the Contact class
    /// </summary>
    public List<Contact> ApprovedContacts { get; set; }

    /// <summary>
    /// All contacts that have a open request. See Entities for more information about the Contact class
    /// </summary>
    public List<Contact> PendingContactRequests { get; set; }
  }
}