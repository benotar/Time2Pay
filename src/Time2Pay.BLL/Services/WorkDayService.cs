using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DTO.WorkDay;

namespace Time2Pay.BLL.Services;

public class WorkDayService : IWorkDayService
{
    private readonly Time2PayDbContext _dbContext;

    public WorkDayService(Time2PayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<WorkDayModel>> GetWorkDayAsync(string id, CancellationToken cancellationToken)
    {
        var workday = await _dbContext.WorkDays
            .AsNoTracking()
            .Where(wd => wd.Id == id)
            //.Select()
            .FirstOrDefaultAsync(cancellationToken);

        if (workday is null)
        {
            //return Result.Failure();
        }

        throw new NotImplementedException();
    }

    public Task<Result<WorkDayModel>> CreateAsync(CreateWorkDayModel request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
