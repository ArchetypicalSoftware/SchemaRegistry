using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class Proto3SchemaValidator : ISchemaValidator
    {
        public Format SchemaFormat { get; } = Format.Proto3;

        public string ContentType { get; set; }

        public ValidationResult Validate(string schema)
        {
            try
            {
                var rest = Froto.Parser.Parse.fromString(schema);
            }
            catch (System.Exception e)
            {
                return new ValidationResult(e.Message);
            }

            return ValidationResult.Success;
        }
    }
}