﻿using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository,IDisposable
    {

        readonly IConfiguration _configuration;
        NpgsqlConnection connection;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }
        public void Dispose()
        {
            connection.Dispose();
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var affected =
                await connection.ExecuteAsync
                    ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            if (affected == 0)
                return false;

            return true;
        }

       

        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName=@ProductName", new { ProductName = productName });

            if (coupon == null)
            {
                return new Coupon()
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount Des"
                };
            }

            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var affected = await connection.ExecuteAsync
                   ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                           new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
