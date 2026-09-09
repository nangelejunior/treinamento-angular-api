using Microsoft.AspNetCore.Mvc;
using Training.Angular.Api.Application.Common;
using Training.Angular.Api.Application.Dtos;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Controllers;

[ApiController]
[Route("v1/accounts")]
public sealed class AccountsController(
    ICreateAccountUseCase create,
    IGetAccountsUseCase list,
    IGetAccountByIdUseCase getById,
    IUpdateAccountUseCase update,
    IDeleteAccountUseCase delete) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(int page, int pageSize, CancellationToken ct)
        => Ok(await list.ExecuteAsync(page, pageSize, ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var account = await getById.ExecuteAsync(id, ct);
        return account is null ? Content("null", "application/json") : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AccountRequest request, CancellationToken ct)
        => Ok(await create.ExecuteAsync(request, ct));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] AccountRequest request, CancellationToken ct)
    {
        await update.ExecuteAsync(id, request, ct);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await delete.ExecuteAsync(id, ct);
        return Ok();
    }
}
