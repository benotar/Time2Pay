using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time2Pay.Api.Database;
using Time2Pay.Api.DTOs.Employments;

namespace Time2Pay.Api.Controllers;

[ApiController]
[Route(Constants.EmploymentsRoute)]
public sealed class EmploymentsController : ControllerBase
{
    private readonly Time2PayDbContext _dbContext;

    public EmploymentsController(Time2PayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<EmploymentsCollectionDto>> Get(CancellationToken cancellationToken)
    {
        var employmentDtoList = await _dbContext.Employments
            .Select(employment => new EmploymentDto
            {
                Id = employment.Id,
                Name = employment.Name,
                Description = employment.Description,
                StartDate = employment.StartDate,
                EndDate = employment.EndDate,
                CreatedAtUtc = employment.CreatedAtUtc,
                LastModifiedAtUtc = employment.LastModifiedAtUtc,
            })
            .ToListAsync(cancellationToken);

        var collectionDto = new EmploymentsCollectionDto
        {
            Items = employmentDtoList
        };

        return Ok(collectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmploymentDto>> Get(string id, CancellationToken cancellationToken)
    {
        var employmentDto = await _dbContext.Employments
            .Where(emp => emp.Id.Equals(id))
            .Select(employment => new EmploymentDto
            {
                Id = employment.Id,
                Name = employment.Name,
                Description = employment.Description,
                StartDate = employment.StartDate,
                EndDate = employment.EndDate,
                CreatedAtUtc = employment.CreatedAtUtc,
                LastModifiedAtUtc = employment.LastModifiedAtUtc,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (employmentDto is null)
        {
            return NotFound();
        }

        return Ok(employmentDto);
    }
}
