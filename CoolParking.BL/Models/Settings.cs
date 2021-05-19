using System.Collections.Generic;

namespace CoolParking.BL.Models
{
    public static class Settings
    {
        public const decimal ParkingBalance = 0;
        public const int ParkingCapacity = 10;

        public const int WithdrawPeriod = 5000;
        public const int LoggingPeriod = 60000;

        public static Dictionary<VehicleType, decimal> VehicleTariffs = new Dictionary<VehicleType, decimal>()
        {
            [VehicleType.PassengerCar] = 2.0m,
            [VehicleType.Truck] = 5.0m,
            [VehicleType.Bus] = 3.5m,
            [VehicleType.Motorcycle] = 1.0m
        };

        public const decimal PenaltyRate = 2.5m;

        public const string LogPath = @"E:\repos\parking\CoolParking\CoolParking.BL\Transactions.log";
    }
}
