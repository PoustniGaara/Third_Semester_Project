﻿using WebApi.DTOs;

namespace WebAppMVC.ViewModels
{
    public class OrderIndexVM
    {

        public IEnumerable<OrderDto> Orders { get; set; }
    }
}