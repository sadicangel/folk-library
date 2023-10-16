using FastEndpoints;

namespace FolkLibrary.Artists.DeleteArtist;

public sealed class DeleteArtistSummanry : Summary<DeleteArtistEndpoint>
{
    public DeleteArtistSummanry()
    {
        Summary = "Deletes an Artist";
        Description = "Deletes an Artist";
        ExampleRequest = new DeleteArtistRequest
        {
            ArtistId = Guid.Empty.ToString()
        };
        Response(200);
        Response<ErrorResponse>(400);
    }
}
