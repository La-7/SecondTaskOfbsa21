using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;
using System.Timers;

namespace CoolParking.BL.Services
{
    public class TimerService : ITimerService
    {
        public event ElapsedEventHandler Elapsed;

        private static Parking parking = Parking.GetParking();
        private static ILogService _logService;

        public static Timer Timer { get; set; }
        public static Timer LogTimer { get; set; }

        public double Interval { get; set; }

        public TimerService()
        {
            _logService = new LogService(Settings.LogPath);

            SetTimer();
            Elapsed = WithdrawTariff;
        }

        public void FireElapsedEvent()
        {
            Elapsed?.Invoke(this, null);
        }

        public void Dispose()
        {
            Timer.Dispose();
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

        private static void SetTimer()
        {
            Timer = new Timer(Settings.WithdrawPeriod);
            Timer.AutoReset = true;
            Timer.Elapsed += WithdrawTariff;
            Timer.Start();

            LogTimer = new Timer(Settings.LoggingPeriod);
            LogTimer.Elapsed += Loginformation;
            LogTimer.AutoReset = true;
            LogTimer.Start();
        }

        private static void WithdrawTariff(object sender, ElapsedEventArgs e)
        {
            foreach (var vehicle in parking.Vehicles)
            {
                parking.CalculatedAndWithdrawParkingPayment(vehicle);
            }
        }

        private static void Loginformation(object sender, ElapsedEventArgs e)
        {
            foreach (var trans in parking.Transactions)
            {
                _logService.Write($"From {trans.VehicleId} at {trans.Time}");
            }

            parking.Transactions.Clear();
        }
    }
}

