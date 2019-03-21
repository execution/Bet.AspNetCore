﻿using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Console = Colorful.Console;

namespace AppAuthentication
{
    [Command(Name = Constants.CLIToolName,
             Description = "Cli tool to help with Docker Container development with Azure MSI Identity.",
             ThrowOnUnexpectedArgument = false,
             AllowArgumentSeparator =true)]
    [Subcommand(typeof(RunCommand))]
    [HelpOption("-?")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    public class Program
    {
        private static async Task<int> Main(string[] args)
        {

            return await CommandLineApplication.ExecuteAsync<Program>(args);
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            Console.WriteAscii(Constants.CLIToolName, Colorful.FigletFont.Default);

            Console.WriteLine("You must specify at a subcommand.", Color.Red);
            app.ShowHelp();
            return 1;
        }

        private static string GetVersion()
        {
            return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}