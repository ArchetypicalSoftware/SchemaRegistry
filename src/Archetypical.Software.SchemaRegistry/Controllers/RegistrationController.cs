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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Archetypical.Software.SchemaRegistry.Attributes;
using Archetypical.Software.SchemaRegistry.Models;
using Archetypical.Software.SchemaRegistry.Shared.Data;
using Archetypical.Software.SchemaRegistry.Shared.Enums;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Archetypical.Software.SchemaRegistry.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly Context _context;
        private readonly IEnumerable<ISchemaValidator> _validators;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(Context context, IEnumerable<ISchemaValidator> validators, ILogger<RegistrationController> logger)
        {
            _context = context;
            _validators = validators;
            _logger = logger;
        }

        /// <summary>
        /// Register schema
        /// </summary>
        /// <remarks>Register schema. If schema of specified name does not exist in specified group, schema is created at version 1. If schema of specified name exists already in specified group, schema is created at latest version + 1. If schema with identical content already exists, existing schema&#x27;s ID is returned.  </remarks>
        /// <param name="body">schema content</param>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId">schema id</param>
        /// <response code="200">OK</response>
        /// <response code="400">Invalid request</response>
        [HttpPost]
        [Route("/schemagroups/{groupId}/schemas/{schemaId}")]
        [ValidateModelState]
        [SwaggerOperation("CreateSchema")]
        [SwaggerResponse(statusCode: 200, type: typeof(Schema), description: "OK")]
        public virtual async Task<IActionResult> CreateSchema([FromRoute][Required] string groupId, [FromRoute][Required] string schemaId)
        {
            // Get the schema group

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var gr = _context.SchemaGroups.FirstOrDefault(x => x.Id == groupId);
            if (gr == null)
                return NotFound();

            var schemas = _context.Schemata.Where(x => x.SchemaGroupId == groupId).ToList()
                .Select(child => new InnerProjection { Format = gr.Format, ChildFormat = child.Format, Version = child.Version, Hash = child.Hash }).ToList();

            if (!schemas.Any())
                schemas.Add(new InnerProjection { Format = gr.Format });

            var any = schemas.First();
            var validator = _validators.FirstOrDefault(x => x.SchemaFormat == (any.Format ?? any.ChildFormat));
            if (validator == null)
                return Problem($"Invalid schema format {any.Format}");

            using var stream = new StreamReader(Request.Body);
            var body = await stream.ReadToEndAsync();

            var validation = validator.Validate(body);
            if (ValidationResult.Success != validation)
                return Problem(validation.ErrorMessage);

            var schema = new Schema()
            {
                Contents = body,
                CreateDateTimeUtc = DateTime.UtcNow,
                Id = schemaId,
                Format = (any.Format ?? any.ChildFormat),
                LastUpdateDateTimeUtc = DateTime.UtcNow,
                SchemaGroupId = groupId,
                Version = 1
            };

            // get the latest schema
            var latest = schemas.OrderByDescending(x => x.Version).First();

            // check for duplicates
            if (schema.Hash != latest.Hash)
            {
                schema.Version = (latest.Version ?? 0) + 1;
                await _context.Schemata.AddAsync(schema);
                await _context.SaveChangesAsync();
            }

            return new OkObjectResult(new { id = schemaId });
        }
    }
}