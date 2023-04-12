using FolkLibrary.Errors;
using OneOf;

namespace FolkLibrary;

[GenerateOneOf]
public sealed partial class Response<TSuccess> : OneOfBase<TSuccess, OneOf.Types.NotFound, AlreadyExists, Invalid> { }
