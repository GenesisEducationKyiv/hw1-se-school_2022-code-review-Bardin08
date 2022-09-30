using Core.Contracts.Crypto.Abstractions;

namespace Integrations.Crypto.Abstractions;

public interface ICryptoProviderChainSegment
{
    ICryptoProvider SetNextProvider(ICryptoProvider nextProvider);
}