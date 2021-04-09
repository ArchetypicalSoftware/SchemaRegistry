using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Google.Protobuf;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class Proto3SchemaValidator : ISchemaValidator
    {
        public Format SchemaFormat { get; } = Format.ProtoV3;

        public string ContentType { get; set; }

        public ValidationResult Validate(string schema)
        {
            try
            {
                var b64 = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(schema));
                var p = Google.Protobuf.Reflection.FileDescriptor.BuildFromByteStrings(new[]
                  {
                ByteString.FromBase64(b64)
            });
            }
            catch (System.Exception e)
            {
                return new ValidationResult(e.Message);
            }

            return ValidationResult.Success;
        }
    }
}