using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Archetypical.Software.SchemaRegistry.Shared.Enums;

namespace Archetypical.Software.SchemaRegistry.Shared.Models
{
    public class Schema
    {
        public string Id { get; set; }
        public string Contents { get; set; }
        public int? Version { get; set; }

        /// <summary>
        /// This is either inherited from the schema group or applied when the schema group has a null value
        /// </summary>
        public Format? Format { get; set; }

        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime LastUpdateDateTimeUtc { get; set; }

        public string Hash
        {
            get
            {
                var Sb = new StringBuilder();

                using var hash = SHA256.Create();
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(Contents));
                foreach (var b in result)
                    Sb.Append(b.ToString("x2"));
                return Sb.ToString();
            }
            set { }
        }

        public string SchemaGroupId { get; set; }

        public SchemaGroup SchemaGroup { get; set; }
    }
}