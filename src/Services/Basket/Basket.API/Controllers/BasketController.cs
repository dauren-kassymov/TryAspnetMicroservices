using System.Threading.Tasks;
using Basket.API.Entities;
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

        public BasketController(IBasketRepo basketRepo)
        {
            _basketRepo = basketRepo;
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
            // TODO: get discount info from Discount.Grpc
            // TODO: and calculate latest price of product
            
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