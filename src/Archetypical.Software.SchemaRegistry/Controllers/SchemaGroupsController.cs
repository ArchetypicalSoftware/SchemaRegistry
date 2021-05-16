using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Archetypical.Software.SchemaRegistry.Attributes;
using Archetypical.Software.SchemaRegistry.Models;
using Archetypical.Software.SchemaRegistry.Shared.Data;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Archetypical.Software.SchemaRegistry.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Authorize]
    public class SchemaGroupsController : ControllerBase
    {
        private readonly Context _context;
        private readonly IEnumerable<ISchemaValidator> _validators;
        private readonly ILogger<SchemaGroupsController> _logger;

        public SchemaGroupsController(Context context, IEnumerable<ISchemaValidator> validators, ILogger<SchemaGroupsController> logger)
        {
            _context = context;
            _validators = validators;
            _logger = logger;
        }

        /// <summary>
        /// Create schema group
        /// </summary>
        /// <remarks>Create schema group with specified format in registry namespace.</remarks>
        /// <param name="body">schema group description</param>
        /// <param name="groupId">schema group</param>
        /// <response code="201">Created</response>
        /// <response code="409">Schema group already exists</response>
        [HttpPut]
        [Route("/schemagroups/{groupId}")]
        [ValidateModelState]
        [SwaggerOperation("CreateGroup")]
        public async Task<IActionResult> CreateGroup([FromBody] SchemaGroup body, [FromRoute][Required] string groupId)
        {
            if (_context.SchemaGroups.Where(x => x.Id == groupId).Select(x => true).FirstOrDefault())
                return StatusCode(409);
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (body.Id != groupId)
                return Problem("Group Id in payload must match route");

            if (_validators.All(x => x.SchemaFormat != body.Format))
                return Problem(
                    $"{body.Format} schema is not supported. Valid schemas are {string.Join(",", _validators.Select(v => v.SchemaFormat))}");

            await _context.SchemaGroups.AddAsync(body);
            await _context.SaveChangesAsync();
            return Created(Url.Action("GetGroup", new { groupId }), body);
        }

        /// <summary>
        /// Delete schema group
        /// </summary>
        /// <remarks>Delete schema group in schema registry namespace.</remarks>
        /// <param name="groupId">schema group</param>
        /// <response code="204">OK no content</response>
        /// <response code="404">Specified group not found</response>
        [HttpDelete]
        [Route("/schemagroups/{groupId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteGroup")]
        public virtual async Task<IActionResult> DeleteGroup([FromRoute][Required] string groupId)
        {
            var group = await _context
                .SchemaGroups
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return NotFound();

            group.Schemas = (await _context.Schemata.Where(x => x.SchemaGroupId == group.Id).ToListAsync());

            foreach (var groupSchema in group.Schemas)
            {
                _context.Schemata.Remove(groupSchema);
            }
            _context.SchemaGroups.Remove(group);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes all schemas in group
        /// </summary>
        /// <remarks>Deletes all schemas under specified group id.</remarks>
        /// <param name="groupId">schema group</param>
        /// <response code="204">OK no content</response>
        /// <response code="404">Group not found</response>
        [HttpDelete]
        [Route("/schemagroups/{groupId}/schemas")]
        [ValidateModelState]
        [SwaggerOperation("DeleteSchemasByGroup")]
        public virtual async Task<IActionResult> DeleteSchemasByGroup([FromRoute][Required] string groupId)
        {
            var group = await _context
                .SchemaGroups
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return NotFound();

            group.Schemas = (await _context.Schemata.Where(x => x.SchemaGroupId == group.Id).ToListAsync());

            foreach (var groupSchema in group.Schemas)
            {
                _context.Schemata.Remove(groupSchema);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get schema group
        /// </summary>
        /// <remarks>Get schema group description in registry namespace.</remarks>
        /// <param name="groupId">schema group</param>
        /// <response code="200">OK</response>
        /// <response code="404">Specified group not found</response>
        [HttpGet]
        [Route("/schemagroups/{groupId}")]
        [ValidateModelState]
        [SwaggerOperation("GetGroup")]
        [SwaggerResponse(statusCode: 200, type: typeof(SchemaGroup), description: "OK")]
        public virtual async Task<IActionResult> GetGroup([FromRoute][Required] string groupId)
        {
            var group = await _context.SchemaGroups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return NotFound();

            return Ok(group);
        }

        /// <summary>
        /// Get list of schema groups
        /// </summary>
        /// <remarks>Get all schema groups in namespace.</remarks>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("/schemagroups")]
        [ValidateModelState]
        [SwaggerOperation("GetGroups")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<string>), description: "OK")]
        [AllowAnonymous]
        public virtual IActionResult GetGroups()
        {
            return Ok(_context.SchemaGroups.Select(x => x.Id));
        }

        /// <summary>
        /// List schemas for group id
        /// </summary>
        /// <remarks>Returns schema by group id.</remarks>
        /// <param name="groupId">schema group</param>
        /// <response code="200">OK</response>
        /// <response code="404">Group not found</response>
        [HttpGet]
        [Route("/schemagroups/{groupId}/schemas")]
        [ValidateModelState]
        [SwaggerOperation("GetSchemasByGroup")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<string>), description: "OK")]
        public virtual async Task<IActionResult> GetSchemasByGroup([FromRoute][Required] string groupId)
        {
            var group = await _context
                .SchemaGroups
                .FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return NotFound();

            group.Schemas = (await _context.Schemata.Where(x => x.SchemaGroupId == group.Id).ToListAsync());
            return Ok(group.Schemas.GroupBy(x => x.Id).Select(x => x.Key).OrderBy(x => x));
        }
    }
}