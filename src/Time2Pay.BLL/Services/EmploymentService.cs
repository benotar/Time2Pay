using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Mapping.Employments;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.DAL;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services;

public sealed class EmploymentService : IEmploymentService
{
    private readonly Time2PayDbContext _dbContext;

    public EmploymentService(Time2PayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<EmploymentDto?> GetEmploymentAsync(string id, CancellationToken cancellationToken)
    {
        return _dbContext.Employments
            .Where(emp => emp.Id == id)
            .Select(EmploymentMapping.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmploymentsCollectionDto> GetEmploymentsAsync(CancellationToken cancellationToken)
    {
        var employmentDtoList = await _dbContext.Employments
            .Select(EmploymentMapping.ProjectToDto())
            .ToListAsync(cancellationToken);

        return new EmploymentsCollectionDto
        {
            Items = employmentDtoList
        };
    }

    public async Task<EmploymentDto> CreateAsync(CreateEmploymentDto dto, CancellationToken cancellationToken)
    {
        var employment = dto.ToEntity();

        _dbContext.Employments.Add(employment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return employment.ToDto();
    }
}
