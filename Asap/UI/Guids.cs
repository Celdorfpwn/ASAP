// Guids.cs
// MUST match guids.h
using System;

namespace SushiPikant.UI
{
    static class GuidList
    {
        public const string guidUIPkgString = "bcb6574e-4c03-4e30-97be-bd8fcb269156";
        public const string guidUICmdSetString = "82681bbd-b3be-49e3-8b75-da08d3fd8426";
        public const string guidToolWindowPersistanceString = "0ff5a854-4dbd-4591-9b98-4b403fa14d12";

        public static readonly Guid guidUICmdSet = new Guid(guidUICmdSetString);
    };
}