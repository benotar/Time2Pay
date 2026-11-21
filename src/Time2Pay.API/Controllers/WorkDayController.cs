using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DTO.WorkDay;
using Time2Pay.Shared;

namespace Time2Pay.API.Controllers;

[Authorize]
[Route(Constants.WorkDaysRoute)]
public class WorkDayController : BaseController
{
    private readonly IWorkDayService _workDayService;

    public WorkDayController(IWorkDayService workDayService)
    {
        _workDayService = workDayService;
    }

    [HttpGet("{id}/{employmentId}")]
    public async Task<ActionResult<WorkDayModel>> Get(string id, string employmentId, CancellationToken cancellationToken)
    {
        var result = await _workDayService.GetWorkDayAsync(UserId, employmentId, id, cancellationToken);
        return result.Match(model => Ok(model), ToProblem);
    }

    [HttpPost]
    public async Task<ActionResult<WorkDayModel>> Create(CreateWorkDayModel request, CancellationToken cancellationToken)
    {
        var result = await _workDayService.CreateAsync(UserId, request, cancellationToken);
        return result.Match(model => CreatedAtAction(nameof(Get), new
                {
                    id = model.Id,
                    employmentId = model.EmploymentId
                },
                model),
            ToProblem);
    }
}
