using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Checkout()
        {
            var basket = await _basketService.Get();
            ViewBag.basket = basket;
            return View(new CheckoutInfoInput());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
        {

            //// 1. yöntem asenkron ileitişim
            // var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
            //if (!orderStatus.IsSuccessful)
            //{
            //    var basket = await _basketService.Get();
            //    ViewBag.basket = basket;
            //    ViewBag.error = orderStatus.Error;
            //    return View();
            //}
            // return RedirectToAction(nameof(SuccessfulCheckout), new {orderId=orderStatus.OrderId});


            // 2. yol asenktron iletişim. Rabbit MQ'ya gönderiyoruz
            // Order servis Rabbit MQ'yu dinleyip o şekilde orderi oluşturacak.
            var orderSuspend = await _orderService.SuspendOrder(checkoutInfoInput);
            if (!orderSuspend.IsSuccessful)
            {
                var basket = await _basketService.Get();
                ViewBag.basket = basket;
                ViewBag.error = orderSuspend.Error;
                return View();
            }
            // burada senkron yöntemde orderId dönüyoruz. asenkronda döndürmeğim için şimdilik 
            // bu şekilde randon değer ürettim. İleride Payment Id dönülebilir.
            return RedirectToAction(nameof(SuccessfulCheckout), new { orderId =new Random().Next(1,1000) });
        }

        public IActionResult SuccessfulCheckout(int orderId)
        {
            ViewBag.orderId = orderId;
            return View();
        }

        public async Task<IActionResult> CheckoutHistory()
        {
            
            return View(await _orderService.GetOrder());
        }

    }
}
