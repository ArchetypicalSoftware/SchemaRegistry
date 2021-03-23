using System.ComponentModel.DataAnnotations;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Json.Schema;
using Format = Archetypical.Software.SchemaRegistry.Shared.Enums.Format;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class JsonSchemaValidator : ISchemaValidator
    {
        public Format SchemaFormat { get; } = Format.JsonSchema;
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