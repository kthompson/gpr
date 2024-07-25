// ReSharper disable once CheckNamespace
namespace System.CommandLine;

public interface ICommandOptionsHandler<in TOptions>
{
    Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken);
}