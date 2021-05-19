using System;

namespace CoolParking.BL.Models
{
    public class TransactionInfo
    {
        public string VehicleId { get; set; }
        public decimal Sum { get; set; }
        public DateTime Time { get; set; }

        public TransactionInfo(string vehicleId, decimal sum, DateTime time)
        {
            VehicleId = vehicleId;
            Sum = sum;
            Time = time;
        }
    }
}
