﻿using System;
using AutoMapper;
using DataAccessLayer.Model;
using WebApi.DTOs;

namespace WebApi.DtoProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                    .ForMember(dest => dest.Status, act => act.MapFrom(src => src.Status))
                    .ForMember(dest => dest.UserEmail, act => act.MapFrom(src => src.User.Email))
                    .ReverseMap();
        }
    }
}

