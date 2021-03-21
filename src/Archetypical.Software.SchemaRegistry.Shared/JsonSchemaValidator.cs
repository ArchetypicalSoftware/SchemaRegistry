using System.ComponentModel.DataAnnotations;
using Json.Schema;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class JsonSchemaValidator : ISchemaValidator
    {
        public string SchemaFormat { get; } = "json-schema";
        public string ContentType { get; } = "text/json";

        public ValidationResult Validate(string schema)
        {
            try
            {
                var x = JsonSchema.FromText(schema);
                return ValidationResult.Success;
            }
            catch (System.Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
    }
}