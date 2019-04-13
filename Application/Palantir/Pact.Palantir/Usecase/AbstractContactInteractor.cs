namespace Pact.Palantir.Usecase
{
  using System.Text;
  using System.Threading.Tasks;

  using Pact.Palantir.Encryption;
  using Pact.Palantir.Entity;
  using Pact.Palantir.Extensions;
  using Pact.Palantir.Repository;
  using Pact.Palantir.Service;

  using Tangle.Net.Entity;

  using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;

  /// <inheritdoc />
  public abstract class AbstractContactInteractor<TIn, T> : AbstractChatInteractor<TIn, T>
    where T : BaseResponse
  {
    protected AbstractContactInteractor(IContactRepository repository, IMessenger messenger, IEncryption keyEncryption)
      : base(messenger, keyEncryption)
    {
      this.Repository = repository;
      this.Messenger = messenger;
    }

    protected IMessenger Messenger { get; }

    protected IContactRepository Repository { get; }

    protected async Task ExchangeKey(Contact requesterDetails, IAsymmetricKey ntruKey, string chatPasSalt)
    {
      var encryptedChatPasSalt = NtruEncryption.Key.Encrypt(ntruKey, Encoding.UTF8.GetBytes(chatPasSalt));

      await this.Messenger.SendMessageAsync(
        new Message(new TryteString(encryptedChatPasSalt.EncodeBytesAsString() + Constants.End), new Address(requesterDetails.ChatKeyAddress)));
    }

    protected async Task SendContactDetails(TryteString payload, ContactInformation receiverInformation)
    {
      await this.Messenger.SendMessageAsync(new Message(payload, receiverInformation.ContactAddress));
    }
  }
}