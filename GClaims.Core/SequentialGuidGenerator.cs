﻿using System.Security.Cryptography;

namespace GClaims.Core;

public enum SequentialGuidType
{
    /// <summary>
    /// Best Performance for MySql and Postgres (Preferred)
    /// </summary>
    SequentialAsString,

    /// <summary>
    /// Best Performance for MySql, Oracle and Postgres
    /// </summary>
    SequentialAsBinary,

    /// <summary>
    /// Best Performance for SQL Server
    /// </summary>
    SequentialAtEnd
}

public static class SequentialGuidGenerator
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public static Guid NewSequentialGuid(SequentialGuidType guidType = SequentialGuidType.SequentialAtEnd)
    {
        var randomBytes = new byte[10];
        _rng.GetBytes(randomBytes);

        var timestamp = DateTime.UtcNow.Ticks / 10000L;
        var timestampBytes = BitConverter.GetBytes(timestamp);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestampBytes);
        }

        var guidBytes = new byte[16];

        switch (guidType)
        {
            case SequentialGuidType.SequentialAsString:
            case SequentialGuidType.SequentialAsBinary:
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                // If formatting as a string, we have to reverse the order
                // of the Data1 and Data2 blocks on little-endian systems.
                if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                {
                    Array.Reverse(guidBytes, 0, 4);
                    Array.Reverse(guidBytes, 4, 2);
                }

                break;

            case SequentialGuidType.SequentialAtEnd:
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(guidType), guidType, null);
        }

        return new Guid(guidBytes);
    }
}