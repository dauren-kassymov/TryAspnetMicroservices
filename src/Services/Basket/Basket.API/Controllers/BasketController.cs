using System;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repos;
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

        public BasketController(IBasketRepo basketRepo, DiscountGrpcService discountGrpcService)
        {
            _basketRepo = basketRepo;
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
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
    }
}