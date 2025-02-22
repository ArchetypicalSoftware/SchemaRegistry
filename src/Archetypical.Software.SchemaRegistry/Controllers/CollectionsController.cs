using Archetypical.Software.SchemaRegistry.Shared.Data;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Archetypical.Software.Vega.Api.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.SchemaRegistry.Controllers;

public class CollectionsController(
    ILogger<GenericApiController<SchemaCollection, Context>> logger,
    Context context)
    : GenericApiController<
        SchemaCollection, Context>(logger, context);