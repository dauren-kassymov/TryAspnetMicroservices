using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(
            IOrderRepo orderRepo, 
            IMapper mapper,
            ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepo.GetByIdAsync(request.Id);
            if (order is null)
            {
                _logger.LogWarning("Order {OrderId} not exist in db", request.Id);
                throw new NotFoundException(nameof(Order), request.Id);
            }

            _mapper.Map(request, order, typeof(UpdateOrderCommand), typeof(Order));
            await _orderRepo.UpdateAsync(order);
            
            return Unit.Value;
        }
    }
}