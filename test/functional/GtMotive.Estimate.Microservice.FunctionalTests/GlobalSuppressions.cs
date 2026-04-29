// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "For avoid xUnit1027.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.CompositionRootCollectionFixture")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "xUnit requires public test fixtures and base classes.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.FunctionalTestBase")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "xUnit requires public test fixtures and base classes.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.CompositionRootTestFixture")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "xUnit requires public test classes.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Specs.Vehicles.AddVehicleFunctionalTests")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "xUnit test methods follow the Arrange_Act_Assert naming convention.", Scope = "module")]
