using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Microsoft.OpenApi.Readers;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class OpenApiSchemaValidator : ISchemaValidator
    {
        public Format SchemaFormat { get; } = Format.OpenApi;
        public string ContentType { get; } = "text/json";

        public ValidationResult Validate(string schema)
        {
            var reader = new OpenApiStreamReader();
            var ms = new MemoryStream(Encoding.Default.GetBytes(schema));
            reader.Read(ms, out var diagnostic);
            if (diagnostic.Errors.Count == 0)
                return ValidationResult.Success;
            return new ValidationResult(string.Join(Environment.NewLine, diagnostic.Errors.Select(x => x.Message)));
        }
    }
}