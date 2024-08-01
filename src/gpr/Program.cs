using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using GitPullRequest;
using GitPullRequest.Commands;
using GitPullRequest.Commands.Navigation;
using GitPullRequest.Commands.Remote;
using GitPullRequest.Services;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var rootCommand = new GitPullRequestCommand();

if (!Console.IsOutputRedirected && !Console.IsInputRedirected)
{
    Console.InputEncoding = System.Text.Encoding.UTF8;
    Console.OutputEncoding = System.Text.Encoding.UTF8;
}

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHelp(ctx =>
    {
        ctx.HelpBuilder.CustomizeLayout(_ =>
            HelpBuilder
                .Default.GetLayout()
                .Skip(1) // Skip the default command description section.
                .Prepend(_ => AnsiConsole.Write(new FigletText(rootCommand.Description!)))
        );
    })
    .UseDependencyInjection(services =>
    {
        services.AddScoped<IRepository>(provider =>
        {
            var cd = provider.GetRequiredService<IO>().GetCurrentDirectory();
            var repo = Repository.Discover(cd);
            return new Repository(repo);
        });

        services.AddSingleton(AnsiConsole.Console);
        services.AddScoped<INavigation, Navigation>();
        services.AddSingleton<IO, SystemIO>();
        services.AddSingleton<IGitHub, GitHub>();
    });

return builder.Build().Invoke(args);
