﻿
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FreeCourse.Services.Order.Application.Handlers
{
    public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Response<List<OrderDto>>>
    {
        private readonly OrderDbContext _context;

        public GetOrderByUserIdQueryHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders =await _context.Orders.Include(x=>x.OrderItems).Where(x=>x.BuyerId==request.UserId).ToListAsync();
            if (!orders.Any()) 
            {
                return Response<List<OrderDto>>.Success(new List<OrderDto>(), 200);
            }
            var orderDto = Application.Mapping.ObjectMapper.Mapper.Map<List<OrderDto>>(orders);
            
            return Response<List<OrderDto>>.Success(orderDto, 200);

        }
    }
}
