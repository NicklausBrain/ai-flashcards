using System;
using System.Collections.Generic;

namespace My1kWordsEe.Models
{
    public record WordSet
    {
        public required string Id { get; init; }

        public required string UserId { get; init; }

        public required string Name { get; init; }

        public required List<string> Words { get; init; } = new List<string>();

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    }
}
