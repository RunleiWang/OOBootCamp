using System;
using Xunit;

namespace TestProject1
{
    public class Ticket
    {

    }

    public class Car
    {
        
    }
    
    public class ParkingLot
    {
        private bool IsAvailable;

        public ParkingLot(bool available)
        {
            IsAvailable = available;
        }

        public Ticket Park(Car car)
        {
            return new Ticket();
        }
    }

    public class ParkingLotTest
    {
        [Fact]
        public void when_parking_lot_has_available_space_and_user_comes_to_park_then_user_gets_a_ticket()
        {
            var parkingLot = new ParkingLot(true);
            var car = new Car();
            var ticket = parkingLot.Park(car);
            Assert.Equal("TestProject1.Ticket", ticket.GetType().ToString());
        }
    }
}