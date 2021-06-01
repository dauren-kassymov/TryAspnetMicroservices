using System;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repos;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepo _basketRepo;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepo basketRepo,
            DiscountGrpcService discountGrpcService,
            IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _basketRepo = basketRepo;
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            _mapper = mapper;
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _basketRepo.GetBasket(userName);
            basket ??= new ShoppingCart(userName);//if first basket of the user
            return Ok(basket);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach (var item in basket.Items)
            {
                //get discount
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                //and apply it
                item.Price -= coupon.Amount;
            }

            return Ok(await _basketRepo.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _basketRepo.DeleteBasket(userName);
            return Ok();
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //get basket for user
            var basket = await _basketRepo.GetBasket(basketCheckout.UserName);
            if (basket is null)
                return BadRequest();

            //create basket checkout event
            var checkoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            
            //set total price explicitly
            checkoutEvent.TotalPrice = basket.TotalPrice;

            //send event
            await _publishEndpoint.Publish(checkoutEvent);

            //remove old basket
            await _basketRepo.DeleteBasket(basket.UserName);
            return Accepted();
        }
    }
}