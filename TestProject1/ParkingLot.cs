using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Serialization.Formatters;
using System.Security.Policy;
using System.Threading;
using Xunit;

namespace TestProject1
{
    public class NoAvailableException : Exception
    {
        public string GetExceptionMessage()
        {
            return "No Available";
        }
    }

    public class UnavailableTicketException : Exception
    {
        public string GetExceptionMessage()
        {
            return "Unavailable Ticket";
        }        
    }

    public class Ticket
    {
        public string Lisence;
        public Ticket(string lisence)
        {
            Lisence = lisence;
        }
    }

    public class Car
    {
        public string Lisence;

        public Car(string lisence)
        {
            Lisence = lisence;
        }
    }

    public class User
    {
        
    }

    public class ParkingBoy
    {
        public List<ParkingLot> WorkedParkingLots = new List<ParkingLot>();

        public ParkingBoy()
        {
            
        }
        
        public ParkingBoy(List<ParkingLot> parkingLots)
        {
            WorkedParkingLots = parkingLots;
        }
        
        public Ticket Park(Car car)
        {
            var firstAvailableParkingLot = WorkedParkingLots.FirstOrDefault(w => w.IsAvailable.Equals(true));
            if (firstAvailableParkingLot == null)
            {
                throw new NoAvailableException();
            }
            return firstAvailableParkingLot.Park(car);
        }
        
        public Car Take(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new UnavailableTicketException();
            }
            
            foreach (var parkingLot in WorkedParkingLots)
            {
                var car = parkingLot.Take(ticket);
                if (car != null)
                {
                    return car;
                }
            }
            return new Car("Not Your Car");
        }
    }
    
    public class ParkingLot
    {
        public bool IsAvailable;
        private List<string> CarStorage = new List<string>();
        
        public ParkingLot(bool available)
        {
            IsAvailable = available;
        }

        public void StoreCar(Car car)
        {
            CarStorage.Add(car.Lisence);
        }

        public Ticket Park(Car car)
        {
            if (IsAvailable == false)
            {
                throw new NoAvailableException();
            }
            StoreCar(car);
            return new Ticket(car.Lisence);
        }

        public Car Take(Ticket ticket)
        {
            if (!CarStorage.Contains(ticket.Lisence))
            {
                throw new UnavailableTicketException();
            }

            CarStorage.Remove(ticket.Lisence);
            return new Car(ticket.Lisence);
        }

        public bool hasCar(Car car)
        {
            if (CarStorage.Contains(car.Lisence))
            {
                return true;
            }

            return false;
        }
    }

    public class ParkingLotTest  
    {
        [Fact]
        void should_park_car_and_get_ticket_when_parking_boy_park_a_car()
        {
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
               new ParkingLot(true)
            });
            var car = new Car("lisence");
            var ticket = parkingBoy.Park(car);
            Assert.NotNull(ticket);
        }
        
        [Fact]
        void should_pick_car_using_valid_ticket_when_parking_boy_pick_a_car()
        {
            var parkingBoy = new ParkingBoy();
            var validTicket = new Ticket("lisence");
            var car = parkingBoy.Take(validTicket);
            Assert.NotNull(car);
        }
        
        [Fact]
        void should_park_car_in_A_given_parking_boy_is_in_parking_lot_A_and_parking_lot_B_when_he_park_car()
        {
            var car = new Car("lisence");
            var parkingLotA = new ParkingLot(true);
            var parkingLotB = new ParkingLot(true);
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
                parkingLotA,
                parkingLotB
            });
            var ticket = parkingBoy.Park(car);
            Assert.NotNull(ticket);
            Assert.True(parkingLotA.hasCar(car));
            Assert.False(parkingLotB.hasCar(car));
        }
        
        [Fact]
        void should_park_car_in_B_given_parking_boy_is_in_parking_lot_A_and_parking_lot_B_B_is_available_when_he_park_car()
        {
            var car = new Car("lisence");
            var parkingLotA = new ParkingLot(false);
            var parkingLotB = new ParkingLot(true);
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
                parkingLotA,
                parkingLotB
            });
            var ticket = parkingBoy.Park(car);
            Assert.NotNull(ticket);
            Assert.True(parkingLotB.hasCar(car));
            Assert.False(parkingLotA.hasCar(car));
        }
        
        [Fact]
        void should_user_get_the_car_given_parkingboy_parked_car_When_user_pick_the_car_with_valid_ticket()
        {
            var car = new Car("lisence");
            var parkingLot = new ParkingLot(true);            
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
                parkingLot
            });
            var availableTicket = parkingBoy.Park(car);
            var returnedCar = parkingLot.Take(availableTicket);
            Assert.NotNull(returnedCar);
            Assert.Equal(car.Lisence, returnedCar.Lisence);
        }
        
        [Fact]
        void should_parking_get_the_car_given_user_parked_car_When_parking_pick_the_car_with_valid_ticket()
        {
            var car = new Car("lisence");
            var parkingLot = new ParkingLot(true);
            var availableTicket = parkingLot.Park(car);
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
                parkingLot
            });
            var returnedCar = parkingBoy.Take(availableTicket);
            Assert.NotNull(returnedCar);
            Assert.Equal(car.Lisence, returnedCar.Lisence);       
        }

        [Fact]
        void should_not_parking_boy_take_the_car_given_parking_boy_work_in_parking_lot_when_user_take_car_with_invalid_ticket()
        {
            var car = new Car("lisence not exist");
            var invalidTicket = new Ticket(car.Lisence);
            var parkingLot = new ParkingLot(true);
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
                parkingLot
            });
            Assert.Throw<UnavailableTicketException>(() => parkingBoy.Take(invalidTicket))
        }

        [Fact]
        void should_not_parking_boy_take_the_car_give_user_parking_car_when_parking_boy_take_car_with_no_ticket()
        {
            var car = new Car("lisence");
            var parkingLot = new ParkingLot(true);
            var parkingBoy = new ParkingBoy(new List<ParkingLot>
            {
                parkingLot
            });
            Assert.Throw(UnavailableTicketException)(() => parkingBoy.Take());
        }
    }
}                     