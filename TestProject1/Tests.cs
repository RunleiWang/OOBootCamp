using System;
using Xunit;

namespace TestProject1
{
    
    public class CustomPrint
    {
        public string GetValue(int num)
        {
            if (num % 3 == 0)
            {
                return "Fizz";
            }
            return String.Empty;
        }

        public void Print(int input)
        {
           Console.WriteLine(GetValue(input));
        }
    }
    
    public class Tests
    {
        [Fact]
        public void when_number_is_times_of_3_return_Fizz()
        {
            var printer = new CustomPrint();
            Assert.Equal("Fizz", printer.GetValue(3));
            Assert.NotEqual("Fizz", printer.GetValue(4));
        }

        [Fact]
        public void when_number_is_times_of_5_return_Buzz()
        {
            var printer = new CustomPrint();
            Assert.Equal("Buzz", printer.GetValue(10));
            Assert.NotEqual("Buzz", printer.GetValue(9));
        }
    }
}