using CommandLine;
using FolkLibrary.Cli;

Parser.Default.ParseArguments<CopyArguments, object>(args)
    .WithParsed<CopyArguments>(CopyHandler.Handle);
