using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Treinamento.Angular.Api.Data;
using Treinamento.Angular.Api.Models;

namespace Treinamento.Angular.Api.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("v1/accounts")]
        public IActionResult Get()
        {
            return Ok(_context.Accounts.OrderBy(x => x.Date).ToList());
        }

        [HttpGet]
        [Route("v1/accounts/{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(_context.Accounts.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost]
        [Route("v1/accounts")]
        public IActionResult Post([FromBody]Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut]
        [Route("v1/accounts/{id}")]
        public IActionResult Put(Guid id, [FromBody]Account account)
        {
            var acc = _context.Accounts.Find(id);
            acc.AccountType = account.AccountType;
            acc.Date = account.Date;
            acc.Description = account.Description;
            acc.Value = account.Value;

            _context.Entry(acc).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("v1/accounts/{id}")]
        public IActionResult Delete(Guid id)
        {
            _context.Accounts.Remove(_context.Accounts.Find(id));
            _context.SaveChanges();
            return Ok();
        }
    }
}
