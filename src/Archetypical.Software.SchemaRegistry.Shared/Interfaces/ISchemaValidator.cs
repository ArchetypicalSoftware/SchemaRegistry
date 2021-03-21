using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Archetypical.Software.SchemaRegistry.Shared.Interfaces
{
    public interface ISchemaValidator
    {
        string SchemaFormat { get; }
        string ContentType { get; }

        ValidationResult Validate(string schema);
    }
}