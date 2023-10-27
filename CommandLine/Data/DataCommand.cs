using System.CommandLine;

namespace FolkLibrary.Data;
internal sealed class DataCommand : Command
{
    public DataCommand() : base("data", "Data operations") { }
}
