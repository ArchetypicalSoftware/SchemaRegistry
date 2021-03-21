using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Archetypical.Software.SchemaRegistry.Shared;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Xunit;

namespace Archetypical.Software.SchemaRegistry.Tests.SharedTests
{
    public class SchemaValidatorTests
    {
        [Theory]
        [InlineData("invalid-avro-schema.json", "avro")]
        [InlineData("invalid-json-schema.json", "json")]
        [InlineData("invalid-xsd-schema.xsd", "xsd")]
        public async Task InValid_Schema_Fails_Validation(string file, string validator)
        {
            var contents = await File.ReadAllTextAsync(@$"C:\projects\Archetypical.Software.SchemaRegistry\src\Archetypical.Software.SchemaRegistry.Tests\SharedTests\{file}");

            ISchemaValidator schemavalidator = null;
            switch (validator)
            {
                case "avro":
                    schemavalidator = new AvroSchemaValidator();
                    break;

                case "json":
                    schemavalidator = new JsonSchemaValidator();
                    break;

                case "xsd":
                    schemavalidator = new XsdSchemaValidator();
                    break;
            }
            var result = schemavalidator.Validate(contents);

            Assert.NotEqual(ValidationResult.Success, result);
        }

        [Theory]
        [InlineData("valid-avro-schema.json", "avro")]
        [InlineData("valid-json-schema.json", "json")]
        [InlineData("valid-xsd-schema.xsd", "xsd")]
        public async Task Valid_Schema_Passes_Validation(string file, string validator)
        {
            var contents = await File.ReadAllTextAsync(@$"C:\projects\Archetypical.Software.SchemaRegistry\src\Archetypical.Software.SchemaRegistry.Tests\SharedTests\{file}");

            ISchemaValidator schemavalidator = null;
            switch (validator)
            {
                case "avro":
                    schemavalidator = new AvroSchemaValidator();
                    break;

                case "json":
                    schemavalidator = new JsonSchemaValidator();
                    break;

                case "xsd":
                    schemavalidator = new XsdSchemaValidator();
                    break;
            }
            var result = schemavalidator.Validate(contents);

            Assert.Equal(ValidationResult.Success, result);
        }
    }
}