﻿using System;
namespace DataAccessLayer.Model
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public Status Status { get; set; }
        public string Note { get; set; }
        public User User { get; set; }
        public List<LineItem> Items { get; set; }

        public Order(int id, DateTime date, decimal totalPrice, Status status, string note, User user, List<LineItem> items)
        {
            Id = id;
            Date = date;
            TotalPrice = totalPrice;
            Status = status;
            Note = note;
            User = user;
            Items = items;
        }
    }

    public enum Status
    {
        CART, PLACED
    }
}

