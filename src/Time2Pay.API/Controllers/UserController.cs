

using Microsoft.AspNetCore.Mvc;
using Time2Pay.Shared;

namespace Time2Pay.API.Controllers;

[Route(Constants.UsersRoute)]
public class UserController : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        return Ok();
    }
}
