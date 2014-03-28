// Guids.cs
// MUST match guids.h
using System;

namespace Company.PocoGenerator
{
    static class GuidList
    {
        public const string guidPocoGeneratorPkgString = "69e1f1e5-d937-4a65-8e04-c1b8b4f9d89d";
        public const string guidPocoGeneratorCmdSetString = "1663ade8-3ef3-46c7-a138-0fddc981e506";
        public const string guidToolWindowPersistanceString = "e1134778-7449-48d9-b269-d999c01d2a06";

        public static readonly Guid guidPocoGeneratorCmdSet = new Guid(guidPocoGeneratorCmdSetString);
    };
}