using Integrations.Crypro.Contracts.Abstractions;

namespace Integrations.Crypto.Abstractions;

public interface ICryptoProviderChainSegment
{
    ICryptoProvider SetNextProvider(ICryptoProvider nextProvider);
}