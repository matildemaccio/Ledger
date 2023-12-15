using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : Controller
    {
        protected IMediator Mediator { get; }

        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}
