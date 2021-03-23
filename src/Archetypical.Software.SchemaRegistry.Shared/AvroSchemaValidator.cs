using System.ComponentModel.DataAnnotations;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Avro;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class AvroSchemaValidator : ISchemaValidator
    {
        public Format SchemaFormat { get; } = Format.Avro;

        public string ContentType { get; } = "text/json";

        public ValidationResult Validate(string schema)
        {
            try
            {
                var x = new Avro.CodeGen();
                x.AddSchema(Schema.Parse(schema));
                var y = x.GenerateCode();

                return ValidationResult.Success;
            }
            catch (System.Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
    }
}