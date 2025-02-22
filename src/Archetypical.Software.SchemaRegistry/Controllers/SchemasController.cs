using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Archetypical.Software.SchemaRegistry.Attributes;
using Archetypical.Software.SchemaRegistry.Shared.Data;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Archetypical.Software.Vega.Api.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace Archetypical.Software.SchemaRegistry.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    public class SchemasController(
        Context context,
        IEnumerable<ISchemaValidator> validators,
        ILogger<SchemasController> logger)
        : GenericApiController<Schema, Context>(logger, context)
    {
        private readonly ILogger<SchemasController> _logger = logger;

        /// <summary>
        /// Create schema in a collection
        /// </summary>
        /// <remarks>Create schema group with specified format in registry namespace.</remarks>
        /// <param name="body">schema group description</param>
        /// <param name="groupId">schema group</param>
        /// <response code="201">Created</response>
        /// <response code="409">Schema group already exists</response>
        [HttpPost]
        [Route("/collections/{groupId}/schema")]
        [ValidateModelState]
        public async Task<IActionResult> Post([FromBody] Schema body, [FromRoute][Required] Guid groupId)
        {
            // check to see if the collection exists
            if (!context.Collections.Where(x => x.Id == groupId).Select(x => true).FirstOrDefault())
                return NotFound();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // we have a valid schema and a collection, lets check to see if this already exists
            if (!context.Schemas.Where(x => x.SchemaCollection.Id == groupId && x.Concurrency == body.GetHashCode()).Select(x => true).FirstOrDefault())
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            body.SchemaCollectionId = groupId;

            if (validators.All(x => x.SchemaFormat != body.Format))
                return Problem(
                    $"{body.Format} schema is not supported. Valid schemas are {string.Join(",", validators.Select(v => v.SchemaFormat))}");

            var resultSchema = await context.Schemas.AddAsync(body);
            var result = await context.SaveChangesAsync();
            return Created(Url.Action("Get", new { body.Id }), body);
        }

        /// <summary>
        /// Delete schema group
        /// </summary>
        /// <remarks>Delete schema group in schema registry namespace.</remarks>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId"></param>
        /// <response code="204">OK no content</response>
        /// <response code="404">Specified group not found</response>
        [HttpDelete]
        [Route("/collections/{groupId}/schema/{schemaId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteSchema")]
        public virtual async Task<IActionResult> Delete([Required] Guid groupId, [Required] Guid schemaId)
        {
            // check to see if the collection exists
            if (!context.Collections.Where(x => x.Id == groupId).Select(x => true).FirstOrDefault())
                return NotFound();
            // check to see if that schema exists with that group id and return it
            var schema = await context.Schemas.FirstOrDefaultAsync(x => x.SchemaCollection.Id == groupId && x.Id == schemaId);
            if (schema == null)
                return NotFound();

            // iterate all the schema versions and delete them
            schema.Schemas = (await context.Versions.Where(x => x.SchemaGroupId == schema.Id).ToListAsync());

            foreach (var groupSchema in schema.Schemas)
            {
                context.Versions.Remove(groupSchema);
            }
            context.Schemas.Remove(schema);
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Delete schema group
        /// </summary>
        /// <remarks>Delete schema group in schema registry namespace.</remarks>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId"></param>
        /// <response code="204">OK no content</response>
        /// <response code="404">Specified group not found</response>
        [HttpPut]
        [Route("/collections/{groupId}/schema/{schemaId}")]
        [ValidateModelState]
        public virtual async Task<IActionResult> Put([Required] Guid groupId, [Required] Guid schemaId, [FromBody] SchemaVersion schema)
        {
            // check to see if the collection exists
            if (!context.Collections.Where(x => x.Id == groupId).Select(x => true).FirstOrDefault())
                return NotFound();
            // check to see if that schema exists with that group id and return it
            var schemata = await context.Schemas.FirstOrDefaultAsync(x => x.SchemaCollection.Id == groupId && x.Id == schemaId);
            if (schema == null)
                return NotFound();

            // iterate all the schema versions and delete them
            schemata.Schemas = (await context.Versions.Where(x => x.SchemaGroupId == schema.Id).ToListAsync());

            // create a new schema version and add it to the list
            var newSchema = new SchemaVersion
            {
                Contents = schema.Contents,
                Format = schema.Format,
                Name = schema.Name,
                Version = schemata.Schemas.Count + 1,
                SchemaGroupId = schema.Id
            };
            await context.Versions.AddAsync(newSchema);
            await context.SaveChangesAsync();
            return Created(Url.Action("Get", "Versions", new { newSchema.Id }), newSchema);
        }

        /// <summary>
        /// List schemas for group id
        /// </summary>
        /// <remarks>Returns schema by group id.</remarks>
        /// <param name="groupId">schema group</param>
        /// <response code="200">OK</response>
        /// <response code="404">Group not found</response>
        [HttpGet]
        [Route("/collections/{groupId}/schemas")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(List<string>), description: "OK")]
        public virtual async Task<IActionResult> List([FromRoute][Required] Guid groupId)
        {
            var group = await context
                .Schemas
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return NotFound();

            group.Schemas = (await context.Versions.Where(x => x.SchemaGroupId == group.Id).ToListAsync());
            return Ok(group.Schemas.GroupBy(x => x.Id).Select(x => x.Key).OrderBy(x => x));
        }
    }
}