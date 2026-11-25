using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.CompensationRates;
using Time2Pay.Shared;

namespace Time2Pay.API.Controllers;

[Authorize]
[Route(Constants.CompensationRatesRoute)]
public class CompensationRateController : BaseController
{
    private readonly ICompensationRateService _rateService;

    public CompensationRateController(ICompensationRateService rateService)
    {
        _rateService = rateService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompensationRateModel>> Get(string employmentId, string id,
        CancellationToken cancellationToken)
    {
        var result = await _rateService.GetByIdAsync(UserId, employmentId, id, cancellationToken);
        return result.Match(model => Ok(model), ToProblem);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<CompensationRateModel>>> GetCollection(string employmentId,
        [FromQuery] CommonQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _rateService.GetAsync(UserId, employmentId, query, cancellationToken);

        return result.Match(models => Ok(models), ToProblem);
    }

    [HttpPost]
    public async Task<ActionResult<CompensationRateModel>> Create(string employmentId, CreateCompensationRateModel request,
        CancellationToken cancellationToken)
    {
        var result = await _rateService.CreateAsync(UserId, employmentId, request, cancellationToken);
        return result.Match(model => CreatedAtAction(nameof(Get), new
                {
                    employmentId = model.EmploymentId,
                    id = model.Id
                },
                model),
            ToProblem);
    }
}
