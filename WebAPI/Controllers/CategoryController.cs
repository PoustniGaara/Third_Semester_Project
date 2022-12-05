﻿using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
            #region Properties and Constructor
            ICategoryDataAccess _categoryDataAccess;
            private readonly IMapper _mapper;

            public CategoryController(ICategoryDataAccess categoryDataAccess, IMapper mapper)
            {
                _categoryDataAccess = categoryDataAccess;
                _mapper = mapper;
            }
            #endregion
            #region Default Crud Actions
            // GET: api/orders
            [HttpGet]
            public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
            {
                IEnumerable<Category> categories = await _categoryDataAccess.GetAllAsync();
                IEnumerable<CategoryDto> categoriesDto = categories.Select(category => _mapper.Map<CategoryDto>(category));
                return Ok(categoriesDto);
            }
        #endregion
    }
}
