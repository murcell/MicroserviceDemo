﻿namespace FreeCourse.Web.Models.Orders
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        // Ödeme geçmişimde adress alanına ihtiyaç yok
        // public AddressDto Address { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
