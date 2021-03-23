using System.ComponentModel.DataAnnotations;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Archetypical.Software.SchemaRegistry.Shared.Interfaces
{
    public interface ISchemaValidator
    {
        Format SchemaFormat { get; }
        string ContentType { get; }

        ValidationResult Validate(string schema);
    }
}