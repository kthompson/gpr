﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GitPullRequest;
using GitPullRequest.Commands;
using GitPullRequest.Commands.Navigation;
using GitPullRequest.Commands.Remote;
using GitPullRequest.Services;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var rootCommand = new RootCommand
{
    // repo-less commands
    new CloneCommand(),
    // Navigation commands
    new PrevCommand(),
    new NextCommand(), // switch stacks
    new UpCommand(),
    new DownCommand(),
    new TopCommand(),
    new BottomCommand(),
    // stack commands
    // absorb,

    // commit commands
    new AddCommand(),
    new CommitCommand(),
    new AmendCommand(),
    new StatusCommand(),
    // remote commands
    new PullRequestCommand(),
    new PullCommand(),
};

if (!Console.IsOutputRedirected && !Console.IsInputRedirected)
{
    Console.InputEncoding = System.Text.Encoding.UTF8;
    Console.OutputEncoding = System.Text.Encoding.UTF8;
}

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
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
    });

return builder.Build().Invoke(args);