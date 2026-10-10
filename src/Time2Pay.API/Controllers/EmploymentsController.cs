using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.DTO.Employments;

namespace Time2Pay.API.Controllers;

[ApiController]
[Route(Constants.EmploymentsRoute)]
public sealed class EmploymentsController : ControllerBase
{
    private readonly IEmploymentService _employmentService;

    public EmploymentsController(IEmploymentService employmentService)
    {
        _employmentService = employmentService;
    }

    [HttpGet]
    public async Task<ActionResult<EmploymentsCollectionDto>> GetCollection(CancellationToken cancellationToken)
    {
        var collectionDto = await _employmentService.GetEmploymentsAsync(cancellationToken);

        return Ok(collectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmploymentDto>> Get(string id, CancellationToken cancellationToken)
    {
        var employmentDto = await _employmentService.GetEmploymentAsync(id, cancellationToken);

        if (employmentDto is null)
        {
            return NotFound();
        }

        return Ok(employmentDto);
    }

    [HttpPost]
    public async Task<ActionResult<EmploymentDto>> CreateAsync([FromBody] CreateEmploymentDto request,
        CancellationToken cancellationToken)
    {
        var employmentDto = await _employmentService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = employmentDto.Id }, employmentDto);
    }
}
