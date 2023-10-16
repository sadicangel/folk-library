using CommandLine;
using FolkLibrary;

Parser.Default.ParseArguments<CopyArguments, object>(args)
    .WithParsed<CopyArguments>(CopyHandler.Handle);
