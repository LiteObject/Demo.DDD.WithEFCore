namespace Demo.DDD.WithEFCore.UnitTest
{
    using Demo.DDD.WithEFCore.API.DTO;
    using System;
    using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
    using Xunit;
    using Xunit.Abstractions;

    public class ModelStateValidationUnitTest
    {
        private readonly ITestOutputHelper _output;
        public ModelStateValidationUnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        [Fact]
        public void Order_DTO_Should_Files_Validation()
        {
            // ARRANGE
            var order = new Order() { Id = 123, Note = "Important note about the order", LineItems = new List<LineItem>() };
            var validationResultList = new List<ValidationResult>();

            // ACT
            var result = Validator.TryValidateObject(order, new ValidationContext(order), validationResultList, true);
            validationResultList.ForEach(v => _output.WriteLine(v.ErrorMessage));

            // ASSERT
            Assert.False(result);
            Assert.NotEmpty(validationResultList);
        }
    }
}
