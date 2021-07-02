using AutoMapper;
using Demo.DDD.WithEFCore.Data;
using Demo.DDD.WithEFCore.Data.Repositories;
using Demo.DDD.WithEFCore.Entities;
using Demo.DDD.WithEFCore.Entities.Enums;
using Demo.DDD.WithEFCore.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Demo.DDD.WithEFCore.UnitTest
{
    public class LineItemUnitTest
    {
        private readonly ITestOutputHelper _output;

        public LineItemUnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        [Fact]        
        public void LineItemUnitPriceCannotBeLessThenOrEqualToOne() 
        {
        }
    }
}
