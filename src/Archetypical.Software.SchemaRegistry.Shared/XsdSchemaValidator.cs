using System.ComponentModel.DataAnnotations;
using System.IO;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;

namespace Archetypical.Software.SchemaRegistry.Shared
{
    public class XsdSchemaValidator : ISchemaValidator
    {
        public string SchemaFormat { get; } = "xsd";
        public string ContentType { get; } = "text/xml";

        public ValidationResult Validate(string schema)
        {
            try
            {
                var result = ValidationResult.Success;
                var xsd = System.Xml.Schema.XmlSchema.Read(new MemoryStream(System.Text.Encoding.Default.GetBytes(schema)),
                    (sender, args) =>
                    {
                        result = new ValidationResult(args.Message);
                    });
                return result;
            }
            catch (System.Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
    }
}