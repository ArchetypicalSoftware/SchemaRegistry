using System.Collections.Generic;
using Archetypical.Software.Vega.Api.Abstractions;

namespace Archetypical.Software.SchemaRegistry.Shared.Models;

public class SchemaCollection : GuidKeyedEntity
{
    public string Name { get; set; }
    public List<Schema> Schemas { get; set; }
}