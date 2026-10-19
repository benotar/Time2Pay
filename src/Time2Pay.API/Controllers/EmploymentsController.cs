using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Employments;

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
    public async Task<ActionResult<EmploymentDto>> Get(string id, CancellationToken cancellationToken)
    {
        var employmentResult = await _employmentService.GetEmploymentAsync(id, cancellationToken);

        return employmentResult.Match(dto => Ok(dto), ToProblem);
    }

    [HttpGet]
    public async Task<ActionResult<EmploymentsCollectionDto>> GetCollection([FromQuery] EmploymentsQueryParameters query,
        CancellationToken cancellationToken)
    {
        var collectionResult = await _employmentService.GetEmploymentsAsync(query, cancellationToken);

        return collectionResult.Match(dto => Ok(dto), ToProblem);
    }

    [HttpPost]
    public async Task<ActionResult<EmploymentDto>> Create(CreateEmploymentDto createEmploymentDto,
        CancellationToken cancellationToken)
    {
        var employmentResult = await _employmentService.CreateAsync(createEmploymentDto, cancellationToken);

        return employmentResult.Match(dto => CreatedAtAction(nameof(Get), new { id = dto.Id }, dto), ToProblem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, UpdateEmploymentDto updateEmploymentDto,
        CancellationToken cancellationToken)
    {
        var result = await _employmentService.UpdateAsync(id, updateEmploymentDto, cancellationToken);

        return result.Match(NoContent, ToProblem);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _employmentService.DeleteAsync(id, cancellationToken);

        return result.Match(NoContent, ToProblem);
    }
}
