/*
 * Cloud Native Data Schema Registry
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 0.1
 *
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Archetypical.Software.SchemaRegistry.Shared.Models
{
    public class Schema
    {
        public string Id { get; set; }
        public string Contents { get; set; }
        public int? Version { get; set; }

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