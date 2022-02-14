using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Demo.DDD.WithEFCore.UnitTest
{
    public class GeneralTest
    {

        //[Fact]
        public void Test_FirstOrDefault_Case_Sensitivity()
        {
            // ARRANGE
            var stringCollection = new string[] { "One", "Two", "Three" };

            // ACT
            var one = stringCollection.FirstOrDefault(s => s.Equals("One", StringComparison.OrdinalIgnoreCase));

            // ASSERT
            Assert.Equal("one", one);
        }
    }
}
