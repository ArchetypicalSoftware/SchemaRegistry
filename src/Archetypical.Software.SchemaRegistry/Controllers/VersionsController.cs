using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Archetypical.Software.SchemaRegistry.Attributes;
using Archetypical.Software.SchemaRegistry.Shared.Data;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Archetypical.Software.Vega.Api.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Archetypical.Software.SchemaRegistry.Controllers
{
    /// <summary>
    ///
    /// </summary>

    public class VersionsController(
        Context context,
        IEnumerable<ISchemaValidator> validators,
        ILogger<VersionsController> logger)
        : GenericApiController<SchemaVersion, Context>(logger, context)
    {
        private readonly ILogger<VersionsController> _logger = logger;

        /// <summary>
        /// Delete specified version of schema
        /// </summary>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId">schema id</param>
        /// <param name="versionNumber">version number</param>
        /// <response code="204">OK no content</response>
        [HttpDelete]
        [Route("/collections/{groupId}/schemas/{schemaId}/versions/{versionnumber}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteSchemaVersion")]
        public virtual IActionResult DeleteSchemaVersion([FromRoute][Required] string groupId, [FromRoute][Required] string schemaId, [FromRoute][Required] int? versionNumber)
        {
            //TODO: Uncomment the next line to return response 204 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(204);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get latest version of schema
        /// </summary>
        /// <remarks>Get latest version of schema.</remarks>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId">schema id</param>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("/collections/{groupId}/schemas/{schemaId}")]
        [ValidateModelState]
        [SwaggerOperation("GetLatestSchema")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "OK")]
        public virtual async Task<IActionResult> GetLatestSchema([FromRoute][Required] Guid groupId,
            [FromRoute][Required] Guid schemaId)
        {
            var schema = await context
                .Versions
                .Where(x => x.SchemaGroupId == groupId && x.Id == schemaId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync();

            if (schema == null)
                return NotFound();
            var group = await context.Schemas.FirstAsync(x => x.Id == groupId);
            Response.Headers.Add("Location",
                Url.Action("GetSchemaVersion", "Versions", new { groupId, schemaId, versionnumber = schema.Version }));
            Response.Headers.Add("Schema-Id", schemaId.ToString());
            Response.Headers.Add("Schema-Id-Location",
                Url.Action("GetLatestSchema", "Schemas", new { groupId, schemaId }));
            Response.Headers.Add("Schema-Version", schema.Version.ToString());

            return new ContentResult()
            {
                Content = schema.Contents,
                ContentType = validators.First(x => x.SchemaFormat == group.Format).ContentType,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Get specified version of schema
        /// </summary>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId">schema id</param>
        /// <param name="versionNumber">version number</param>
        /// <response code="200">OK</response>
        /// <response code="404">Specified schema not found</response>
        [HttpGet]
        [Route("/collections/{groupId}/schemas/{schemaId}/versions/{versionnumber}")]
        [ValidateModelState]
        [SwaggerOperation("GetSchemaVersion")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "OK")]
        public virtual async Task<IActionResult> GetSchemaVersion([FromRoute][Required] Guid groupId,
            [FromRoute][Required] Guid schemaId, [FromRoute][Required] int? versionNumber)
        {
            var schema = await context.Versions.Where(x => x.SchemaGroupId == groupId && x.Id == schemaId && x.Version == versionNumber)
                .FirstOrDefaultAsync();

            if (schema == null)
                return NotFound();
            var group = await context.Schemas.FirstAsync(x => x.Id == groupId);
            Response.Headers.Add("Location",
                Url.Action("GetSchemaVersion", "Versions", new { groupId, schemaId, versionnumber = schema.Version }));
            Response.Headers.Add("Schema-Id", schemaId.ToString());
            Response.Headers.Add("Schema-Id-Location",
                Url.Action("GetLatestSchema", "Schemas", new { groupId, schemaId }));
            Response.Headers.Add("Schema-Version", schema.Version.ToString());

            return new ContentResult()
            {
                Content = schema.Contents,
                ContentType = validators.First(x => x.SchemaFormat == group.Format).ContentType,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Get list of versions
        /// </summary>
        /// <remarks>Get list of versions for specified schema</remarks>
        /// <param name="groupId">schema group</param>
        /// <param name="schemaId">schema id</param>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("/collections/{groupId}/schemas/{schemaId}/versions")]
        [ValidateModelState]
        [SwaggerOperation("GetSchemaVersions")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<int?>), description: "OK")]
        public virtual async Task<IActionResult> GetSchemaVersions([FromRoute][Required] Guid groupId,
            [FromRoute][Required] Guid schemaId)
        {
            var schema = await context.Versions
                .Where(x => x.SchemaGroupId == groupId && x.Id == schemaId)
                .Select(x => x.Version)
                .ToListAsync();

            return Ok(schema);
        }
    }
}