﻿using RestSharp;
using WebApiClient.DTOs;
using WebApiClient.Interfaces;

namespace WebApiClient.RestSharpClientImplementation
{
    public class ProductClient : IProductClient
    {
        RestClient _client;
        public ProductClient(string restUrl) => _client = new RestClient(restUrl);

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var request = new RestRequest($"{id}");
            return await _client.GetAsync<ProductDto?>(request);
        }

        public async Task<IEnumerable<ProductDto>?> GetAllAsync(string? category)
        {
            var request = new RestRequest().AddQueryParameter("category", category);
            return await _client.GetAsync<IEnumerable<ProductDto>>(request);

            //var response = await _client.ExecuteGetAsync<IEnumerable<ProductDto>>(new RestRequest());
            //if (!response.IsSuccessful)
            //{
            //    throw new Exception($"Error in retrieving all products. Message was {response.Content}");
            //}
            //return response.Data;
        }
    }
}
