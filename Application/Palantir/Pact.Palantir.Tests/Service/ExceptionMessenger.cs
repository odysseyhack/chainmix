namespace Pact.Palantir.Tests.Service
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Threading.Tasks;

  using Pact.Palantir.Entity;
  using Pact.Palantir.Exception;
  using Pact.Palantir.Service;
  using Pact.Palantir.Usecase;

  using Tangle.Net.Entity;

  /// <summary>
  /// The exception messenger.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal class ExceptionMessenger : IMessenger
  {
    public ExceptionMessenger(Exception exception = null)
    {
      this.Exception = exception;
    }

    private Exception Exception { get; }

    /// <inheritdoc />
    public Task<List<Message>> GetMessagesByAddressAsync(Address address)
    {
      if (this.Exception != null)
      {
        throw this.Exception;
      }

      throw new MessengerException(ResponseCode.MessengerException);
    }

    /// <inheritdoc />
    public Task SendMessageAsync(Message message)
    {
      if (this.Exception != null)
      {
        throw this.Exception;
      }

      throw new MessengerException(ResponseCode.MessengerException);
    }
  }
}