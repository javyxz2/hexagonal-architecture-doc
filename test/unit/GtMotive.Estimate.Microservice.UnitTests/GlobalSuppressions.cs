// This file is used by Code Analysis to maintain SuppressMessage attributes.
// Test projects use the standard xUnit naming convention: MethodName_Condition_ExpectedResult,
// which requires underscores. CA1707 is suppressed project-wide for this reason.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "xUnit test methods follow the Arrange_Act_Assert naming convention.",
    Scope = "module")]
