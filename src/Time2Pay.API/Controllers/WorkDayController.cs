using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.WorkDays;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkDayModel>> Get(string employmentId, string id, CancellationToken cancellationToken)
    {
        var result = await _workDayService.GetByIdAsync(UserId, employmentId, id, cancellationToken);
        return result.Match(model => Ok(model), ToProblem);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<WorkDayModel>>> GetCollection(string employmentId,
        [FromQuery] CommonQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _workDayService.GetAsync(UserId, employmentId, query, cancellationToken);

        return result.Match(models => Ok(models), ToProblem);
    }

    [HttpPost]
    public async Task<ActionResult<WorkDayModel>> Create(string employmentId, CreateWorkDayModel request,
        CancellationToken cancellationToken)
    {
        var result = await _workDayService.CreateAsync(UserId, employmentId, request, cancellationToken);
        return result.Match(model => CreatedAtAction(nameof(Get), new
                {
                    employmentId = model.EmploymentId,
                    id = model.Id
                },
                model),
            ToProblem);
    }
}
