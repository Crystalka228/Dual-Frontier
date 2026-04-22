using System;
using System.Collections.Generic;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Aggregate result of <see cref="ContractValidator.Validate"/>. Immutable.
/// <see cref="IsValid"/> is derived from the emptiness of <see cref="Errors"/>.
/// </summary>
/// <param name="IsValid">True when no blocking errors were reported.</param>
/// <param name="Errors">Errors that prevent the pipeline from proceeding.</param>
/// <param name="Warnings">Advisories that do not prevent loading.</param>
public sealed record ValidationReport(
    bool IsValid,
    IReadOnlyList<ValidationError> Errors,
    IReadOnlyList<ValidationWarning> Warnings)
{
    /// <summary>
    /// Factory for the success case: no errors, no warnings, valid.
    /// </summary>
    public static ValidationReport Ok()
        => new(true, Array.Empty<ValidationError>(), Array.Empty<ValidationWarning>());
}
