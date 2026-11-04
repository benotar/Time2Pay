using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.Employments;
using Time2Pay.Shared;

namespace Time2Pay.API.Controllers;

[Route(Constants.EmploymentsRoute)]
public sealed class EmploymentsController : BaseController
{
    private readonly IEmploymentService _employmentService;

    public EmploymentsController(IEmploymentService employmentService)
    {
        _employmentService = employmentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmploymentModel>> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _employmentService.GetEmploymentAsync(id, cancellationToken);

        return result.Match(model => Ok(model), ToProblem);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<EmploymentModel>>> GetCollection([FromQuery] EmploymentsQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _employmentService.GetEmploymentsAsync(query, cancellationToken);

        return result.Match(model => Ok(model), ToProblem);
    }

    [HttpPost]
    public async Task<ActionResult<EmploymentModel>> Create(CreateEmploymentModel request,
        CancellationToken cancellationToken)
    {
        var result = await _employmentService.CreateAsync(request, cancellationToken);

        return result.Match(model => CreatedAtAction(nameof(Get), new { id = model.Id }, model), ToProblem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, UpdateEmploymentModel request,
        CancellationToken cancellationToken)
    {
        var result = await _employmentService.UpdateAsync(id, request, cancellationToken);

        return result.Match(NoContent, ToProblem);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _employmentService.DeleteAsync(id, cancellationToken);

        return result.Match(NoContent, ToProblem);
    }
}
