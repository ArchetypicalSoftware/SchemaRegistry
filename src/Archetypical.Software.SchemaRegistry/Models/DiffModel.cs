using System;
using Archetypical.Software.SchemaRegistry.Shared.Enums;

namespace Archetypical.Software.SchemaRegistry.Models
{
    public class DiffModel
    {
        public string GroupId { get; set; }
        public Guid SchemaId { get; set; }
        public int Version { get; set; }
        public Format Format { get; set; }
        public int PreviousVersion { get; set; }
    }
}