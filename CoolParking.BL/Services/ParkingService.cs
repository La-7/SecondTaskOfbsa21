using System;
using CoolParking.BL.Interfaces;
using System.Collections.ObjectModel;
using CoolParking.BL.Models;

namespace CoolParking.BL.Services
{
    public class ParkingService : IParkingService
    {
        private readonly Parking parking;
        private readonly ITimerService _withdrawTimer;
        private readonly ITimerService _logTimer;
        private readonly ILogService _logService;

        public ParkingService(ITimerService withdrawTimer, ITimerService logTimer, ILogService logService)
        {
            parking = Parking.GetParking();
            _withdrawTimer = withdrawTimer;
            _logTimer = logTimer;
            _logService = logService;
        }

        public void AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return;

            if (parking.Vehicles != null)
            {
                if (parking.Vehicles.Exists(v => v.Id.Equals(vehicle.Id)))
                    throw new ArgumentException();
            }

            if (GetFreePlaces() > 0)
                parking.Vehicles.Add(vehicle);
            else
                throw new InvalidOperationException("Parking if full");
        }

        public void Dispose()
        {
            parking.Vehicles.Clear();
            parking.Balance = 0;
            parking.Transactions.Clear();
        }

        public decimal GetBalance()
        {
            return parking.Balance;
        }

        public int GetCapacity()
        {
            return parking.Vehicles.Capacity;
        }

        public int GetFreePlaces()
        {
            return parking.Vehicles.Capacity - parking.Vehicles.Count;
        }

        public TransactionInfo[] GetLastParkingTransactions()
        {
            return parking.Transactions.ToArray();
        }

        public ReadOnlyCollection<Vehicle> GetVehicles()
        {
            ReadOnlyCollection<Vehicle> collection = new ReadOnlyCollection<Vehicle>(parking.Vehicles);

            return collection;
        }

        public string ReadFromLog()
        {
            string result = _logService.Read();

            return result;
        }

        public void RemoveVehicle(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId))
                return;

            Vehicle requiredCar = parking.Vehicles.Find(v => v.Id.Equals(vehicleId));

            if (requiredCar != null)
            {
                if (parking.Balance >= 0)
                    parking.Vehicles.Remove(requiredCar);
                else
                    throw new InvalidOperationException("Balance must be positive");
            }
            else
            {
                throw new ArgumentException("There is no car with this id. Try again, please.");
            }
        }

        public void TopUpVehicle(string vehicleId, decimal sum)
        {
            if (sum < 1)
                throw new ArgumentException("Top up sum mustn't be negative");

            if (string.IsNullOrEmpty(vehicleId))
                return;

            var requiredCar = parking.Vehicles.Find(v => v.Id.Equals(vehicleId));

            if (requiredCar == null)
                throw new ArgumentException("There is no car with this id. Try again, please.");

            requiredCar.Balance += sum;
        }
    }
}
