using System;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Archetypical.Software.SchemaRegistry.Shared.Models;

namespace Archetypical.Software.SchemaRegistry.Models
{
    public class DiffModel
    {
        public Guid GroupId { get; set; }
        public Guid SchemaId { get; set; }
        public int Version { get; set; }
        public Format Format { get; set; }
        public int PreviousVersion { get; set; }
    }

    public class HomeViewModel
    {
        public SchemaCollection Collection { get; set; }
        public int SchemaCount => Collection?.Schemas.Count ?? 0;
    }
}