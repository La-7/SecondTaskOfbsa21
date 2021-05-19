using System;
using System.Collections.Generic;

namespace CoolParking.BL.Models
{
    public class Parking
    {
        private static readonly Parking parking = new Parking();

        public List<TransactionInfo> Transactions { get; set; }
        public List<Vehicle> Vehicles { get; private set; }

        public decimal Balance { get; internal set; } = 10.5m;

        public Parking()
        {
            Balance = Settings.ParkingBalance;
            Vehicles = new List<Vehicle>();
            Vehicles.Capacity = Settings.ParkingCapacity;
            Transactions = new List<TransactionInfo>();
        }

        public static Parking GetParking()
        {
            return parking;
        }

        public decimal CalculatedAndWithdrawParkingPayment(Vehicle vehicle)
        {
            decimal tariff = Settings.VehicleTariffs[vehicle.VehicleType];
            decimal payment = 0;

            if (vehicle.Balance >= tariff)
            {
                vehicle.Balance -= tariff;
                Balance += tariff;
                parking.Transactions.Add(new TransactionInfo(vehicle.Id, tariff, DateTime.Now));
            }
            else if (vehicle.Balance <= 0)
            {
                payment = tariff * Settings.PenaltyRate;
                vehicle.Balance -= payment;
                Balance += payment;
                parking.Transactions.Add(new TransactionInfo(vehicle.Id, payment, DateTime.Now));
            }
            else
            {
                payment = vehicle.Balance + (vehicle.Balance - tariff) * Settings.PenaltyRate;
                Balance -= payment;
                parking.Transactions.Add(new TransactionInfo(vehicle.Id, payment, DateTime.Now));
            }

            return payment;
        }
    }
}