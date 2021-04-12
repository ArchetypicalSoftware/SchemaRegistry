using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Archetypical.Software.SchemaRegistry.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Format
    {
        [EnumMember(Value = "json-schema")]
        JsonSchema,

        Avro,
        XSD,
        Proto2,
        Proto3,
        WSDL,
        OpenApi
    }
}