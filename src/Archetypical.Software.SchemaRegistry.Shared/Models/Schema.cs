using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Archetypical.Software.Vega.Api.Abstractions;

namespace Archetypical.Software.SchemaRegistry.Shared.Models
{
    /// <summary>
    ///
    /// </summary>

    public partial class Schema : GuidKeyedEntity, IEquatable<Schema>
    {
        [DataMember(Name = "friendly_name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets Format
        /// </summary>
        [DataMember(Name = "format")]
        public Format? Format { get; set; }

        /// <summary>
        /// Gets or Sets GroupProperties
        /// </summary>
        [DataMember(Name = "groupProperties")]
        public Dictionary<string, string> SchemaProperties { get; set; }

        [JsonIgnore]
        public string SchemaPropertiesString
        {
            get => JsonSerializer.Serialize(SchemaProperties ?? new Dictionary<string, string>());
            set => SchemaProperties = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
        }

        [JsonIgnore]
        public SchemaCollection SchemaCollection { get; set; }

        [JsonIgnore]
        public Guid SchemaCollectionId { get; set; }

        [JsonIgnore]
        public List<SchemaVersion> Schemas { get; set; }

        [DataMember(Name = "Concurrency")]
        public int Concurrency
        {
            get => GetHashCode();
            set => _ = value;
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SchemaGroup {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  Createdtimeutc: ").Append(CreatedTimeUtc).Append("\n");
            sb.Append("  Updatedtimeutc: ").Append(ModifiedTimeUtc).Append("\n");
            sb.Append("  Format: ").Append(Format).Append("\n");
            sb.Append("  GroupProperties: ").Append(SchemaProperties).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Schema)obj);
        }

        /// <summary>
        /// Returns true if SchemaGroup instances are equal
        /// </summary>
        /// <param name="other">Instance of SchemaGroup to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Schema other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) &&
                (
                    Description == other.Description ||
                    Description != null &&
                    Description.Equals(other.Description)
                ) &&
                (
                    Format == other.Format ||
                    Format != null &&
                    Format.Equals(other.Format)
                ) &&
                (
                    SchemaProperties == other.SchemaProperties ||
                    SchemaProperties != null &&
                    SchemaProperties.SequenceEqual(other.SchemaProperties)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                if (Description != null)
                    hashCode = hashCode * 59 + Description.GetHashCode();
                if (CreatedTimeUtc != null)
                    hashCode = hashCode * 59 + CreatedTimeUtc.GetHashCode();
                if (ModifiedTimeUtc != null)
                    hashCode = hashCode * 59 + ModifiedTimeUtc.GetHashCode();
                if (Format != null)
                    hashCode = hashCode * 59 + Format.GetHashCode();
                if (SchemaProperties != null)
                    hashCode = hashCode * 59 + SchemaProperties.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(Schema left, Schema right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Schema left, Schema right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}