using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CoolParking.BL.Models
{
    public class Vehicle
    {
        public string Id { get; }
        public VehicleType VehicleType { get; }
        public decimal Balance { get; internal set; }

        public Vehicle(string id, VehicleType vehicleType, decimal balance)
        {
            if (IsId(id))
            {
                Id = id;
            }

            if (balance > 0)
                Balance = balance;
            else
                throw new ArgumentException("Balance must be positive");

            VehicleType = vehicleType;
        }

        public static bool IsId(string id)
        {
            if (id == null)
                return false;

            id = id.Replace(" ", "")
                   .ToUpper();

            if (!Regex.IsMatch(id, @"^[A-Z]{2}-\d{4}-[A-Z]{2}$"))
                throw new ArgumentException("Invalid id");
            else
                return true;
        }

        public static string GenerateRandomRegistrationPlateNumber()
        {
            string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNNM";
            int position = 0;
            Random rnd = new Random();
            StringBuilder result = new StringBuilder(7);

            for (int i = 0; i < 8; i++)
            {
                if (!(i > 1 && i < 6))
                {
                    position = rnd.Next(0, alphabet.Length - 1);
                    result.Append(alphabet[position]);
                }
                else
                    result.Append(rnd.Next(0, 10));
            }

            result.Insert(2, '-')
                  .Insert(7, '-');

            return result.ToString();
        }
    }
}
