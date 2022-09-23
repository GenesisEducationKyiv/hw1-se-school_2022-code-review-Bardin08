namespace Core.Crypto.Abstractions;

public interface ICryptoProviderChainSegment
{
    ICryptoProvider SetNextProvider(ICryptoProvider nextProvider);
}