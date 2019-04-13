namespace Pact.Palantir.Usecase.CreateUser
{
  using System;
  using System.Threading.Tasks;

  using Pact.Palantir.Encryption;
  using Pact.Palantir.Entity;
  using Pact.Palantir.Exception;
  using Pact.Palantir.Extensions;
  using Pact.Palantir.Service;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Signing;

  public class CreateUserInteractor : AbstractUserInteractor<CreateUserRequest, CreateUserResponse>
  {
    public CreateUserInteractor(
      IMessenger messenger,
      IAddressGenerator addressGenerator,
      IEncryption encryption,
      ISignatureFragmentGenerator signatureGenerator)
      : base(signatureGenerator)
    {
      this.Messenger = messenger;
      this.AddressGenerator = addressGenerator;
      this.Encryption = encryption;
    }

    private IMessenger Messenger { get; }

    private IAddressGenerator AddressGenerator { get; }

    private IEncryption Encryption { get; }

    /// <inheritdoc />
    public override async Task<CreateUserResponse> ExecuteAsync(CreateUserRequest request)
    {
      try
      {
        var publicKeyAddress = await this.AddressGenerator.GetAddressAsync(request.Seed, Constants.MessengerSecurityLevel, 0);
        var requestAddress = publicKeyAddress.DeriveRequestAddress();

        var ntruKeyPair = this.Encryption.CreateAsymmetricKeyPair(request.Seed.Value.ToLower(), publicKeyAddress.Value);
        var payload = await this.CreateSignedPublicKeyPayloadAsync(ntruKeyPair.PublicKey, requestAddress, publicKeyAddress.PrivateKey);

        await this.Messenger.SendMessageAsync(new Message(payload, publicKeyAddress));
        return new CreateUserResponse
                 {
                   Code = ResponseCode.Success,
                   NtruKeyPair = ntruKeyPair,
                   PublicKeyAddress = publicKeyAddress,
                   RequestAddress = requestAddress
                 };
      }
      catch (MessengerException exception)
      {
        return new CreateUserResponse { Code = exception.Code };
      }
      catch (Exception)
      {
        return new CreateUserResponse { Code = ResponseCode.UnkownException };
      }
    }
  }
}