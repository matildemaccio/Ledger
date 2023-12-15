using Ledger.Api.Controllers;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Api.Tests
{
    public class BaseControllerTests
    {
        protected Mock<IMediator> mediator;
        protected AccountsController accountsController;
        protected TransactionsController transactionsController;

        [SetUp]
        public void Setup()
        {
            mediator = new Mock<IMediator>();
            accountsController = new AccountsController(mediator.Object);
            transactionsController = new TransactionsController(mediator.Object);
        }
    }
}
