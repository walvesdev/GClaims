namespace GClaims.Core.Extensions;

public static class StreamExtensions
{
    public static byte[] GetAllBytes(this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        if (stream.CanSeek)
        {
            stream.Position = 0L;
        }

        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static async Task<byte[]> GetAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        if (stream.CanSeek)
        {
            stream.Position = 0L;
        }

        await CopyToAsync(stream, memoryStream, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    public static Task CopyToAsync(this Stream stream, Stream destination, CancellationToken cancellationToken)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0L;
        }

        return stream.CopyToAsync(destination, 81920, cancellationToken);
    }
}