using System.Timers;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;
using CoolParking.BL.Services;

namespace CoolParking.BL.Tests
{
    public class FakeTimerService :  ITimerService
    {
        public event ElapsedEventHandler Elapsed;

        private static Parking parking = Parking.GetParking();
        private static ILogService _logService;

        private static Timer Timer { get; set; }
        public double Interval { get; set; }


        public FakeTimerService()
        {
            _logService = new LogService(Settings.LogPath);

            SetTimer(Settings.LoggingPeriod);
            Interval = Settings.WithdrawPeriod; 

            Elapsed += WithdrawTariff;
        }

        public void FireElapsedEvent()
        {
            Elapsed?.Invoke(this, null);
        }

        public void Start()
        {
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
            Dispose();
        }

        public void Dispose()
        {
           Timer.Dispose();
        }

        private static void SetTimer(double interval)
        {
            Timer = new Timer();
            Timer.Interval = 60000;
            Timer.Elapsed += LogInformation;
            Timer.Start();
        }

        private static void LogInformation(object sender, ElapsedEventArgs e)
        {
            foreach (var trans in parking.Transactions)
            {
                _logService.Write($"{trans.Sum} from {trans.VehicleId} at {trans.Time}");
            }

            parking.Transactions.Clear();
        }

        private static void WithdrawTariff(object sender, ElapsedEventArgs e)
        {
            foreach (var vehicle in parking.Vehicles)
            {
                parking.CalculatedAndWithdrawParkingPayment(vehicle);
            }
        }

    }
}
