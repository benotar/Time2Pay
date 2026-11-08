using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DTO.WorkDay;
using Time2Pay.Shared;

namespace Time2Pay.API.Controllers;

[Route(Constants.WorkDaysRoute)]
public class WorkDayController : BaseController
{
    private readonly IWorkDayService _workDayService;

    public WorkDayController(IWorkDayService workDayService)
    {
        _workDayService = workDayService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _workDayService.GetWorkDayAsync(id, cancellationToken);

        return result.Match(model => Ok(model), ToProblem);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkDayModel request, CancellationToken cancellationToken)
    {
        var result = await _workDayService.CreateAsync(request, cancellationToken);

        return result.Match(model => CreatedAtAction(nameof(Get), new { id = model.Id }, model), ToProblem);
    }
}
