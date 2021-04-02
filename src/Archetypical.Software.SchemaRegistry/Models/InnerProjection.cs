using Archetypical.Software.SchemaRegistry.Shared.Enums;

namespace Archetypical.Software.SchemaRegistry.Models
{
    internal class InnerProjection
    {
        public Format? Format { get; set; }
        public Format? ChildFormat { get; set; }
        public int? Version { get; set; }
        public string Hash { get; set; }
    }
}