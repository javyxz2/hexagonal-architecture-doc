// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "S6960:Controllers should not have too many responsibilities", Justification = "All actions are cohesive vehicle operations sharing the same resource.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Api.Controllers.VehiclesController")]
