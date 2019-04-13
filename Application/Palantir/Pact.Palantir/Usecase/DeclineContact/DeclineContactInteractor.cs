namespace Pact.Palantir.Usecase.DeclineContact
{
  using System;
  using System.Threading.Tasks;

  using Pact.Palantir.Repository;

  /// <summary>
  /// The decline contact interactor.
  /// </summary>
  public class DeclineContactInteractor : IUsecaseInteractor<DeclineContactRequest, DeclineContactResponse>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DeclineContactInteractor"/> class.
    /// </summary>
    /// <param name="contactRepository">
    /// The contact repository.
    /// </param>
    public DeclineContactInteractor(IContactRepository contactRepository)
    {
      this.ContactRepository = contactRepository;
    }

    /// <summary>
    /// Gets the contact repository.
    /// </summary>
    private IContactRepository ContactRepository { get; }

    /// <inheritdoc />
    public async Task<DeclineContactResponse> ExecuteAsync(DeclineContactRequest request)
    {
      try
      {
        await this.ContactRepository.AddContactAsync(request.ContactChatAddress.Value, false, request.UserPublicKeyAddress.Value);

        return new DeclineContactResponse { Code = ResponseCode.Success };
      }
      catch (Exception)
      {
        return new DeclineContactResponse { Code = ResponseCode.UnkownException };
      }
    }
  }
}