using CoolParking.BL.Models;
using CoolParking.BL.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace CoolParking.Interface
{
	public static class Interface
	{
		private static HttpClientHandler _clientHandler;
		private static HttpClient _client;
		private static readonly ParkingService _parkingService;
		private static readonly TimerService _withdrawTimer;
		private static readonly TimerService _logTimer;
		private static readonly LogService _logService;

		static Interface()
		{
			_clientHandler = new HttpClientHandler();
			_clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyError) => { return true; };
			_client = new HttpClient(_clientHandler);
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_withdrawTimer = new TimerService();
			_logTimer = new TimerService();
			_logService = new LogService(Settings.LogPath);
			_parkingService = new ParkingService(_withdrawTimer, _logTimer, _logService);
		}

		public static void MainMenu()
		{
			Console.WriteLine("What do you need to do?!\n" +
				 "\n1. List of cars in the parking now \n" +
				 "2. Show current parking balance \n" +
				 "3. Number of free parking spaces \n" +
				 "4. Add a vehicle in the the parking \n" +
				 "5. Top up the vehicle balance \n" +
				 "6. Remove a vehicle from parking \n" +
				 "7. Display the amount of money earned for the current period \n" +
				 "8. Display all parking transactions for the current period\n" +
				 "9. Dispay all transaction history \n" +
				 "10. Exit");

			int result = 0;

			Console.WriteLine("Enter the number corresponding to the required action: ");

			while (!int.TryParse(Console.ReadLine(), out result) || result < 1 || result > 10)
			{
				Console.WriteLine("Invalid value. Try again, please.");
			}

			NextSteps(result);
		}

		private static void NextSteps(int result)
		{
			switch (result)
			{
				case 1:
					Interface.ShowListOfVehicle();
					break;
				case 2:
					Interface.ShowParkingBalance();
					break;
				case 3:
					Interface.ShowNumberOfFreeParkingSpaces();
					break;
				case 4:
					Interface.Add();
					break;
				case 5:
					Interface.TopUp();
					break;
				case 6:
					Interface.Remove();
					break;
				case 7:
					Interface.AmountOfMoneyForPeriod();
					break;
				case 8:
					Interface.DisplayAllRecentlyTransaction();
					break;
				case 9:
					Interface.DisplayAllTransactionHistory();
					break;
				case 10:
					Interface.Close();
					break;
			}
		}

		private static void Close()
		{
			Environment.Exit(0);
		}

		private static void DisplayAllTransactionHistory()
		{
			string result = _parkingService.ReadFromLog();

			Console.WriteLine(result);
		}

		private static void DisplayAllRecentlyTransaction()
		{
			var result = _parkingService?.GetLastParkingTransactions();

			if (result == null)
			{
				Console.WriteLine("There is no transactions");
				return;
			}

			foreach (var trans in result)
			{
				Console.WriteLine($"{trans.VehicleId} | {trans.Time} | {trans.Sum}");
			}

			MainMenu();
		}

		private static void AmountOfMoneyForPeriod()
		{
			var result = _parkingService?.GetLastParkingTransactions();
			decimal sum = 0;

			if (result == null)
			{
				Console.WriteLine("Parking is empty");
				return;
			}

			foreach (var trans in result)
			{
				sum += trans.Sum;
			}

			Console.WriteLine("Amount money earned for the current period: " + sum);

			MainMenu();
		}

		private static void ShowListOfVehicle()
		{
			var result = _parkingService.GetVehicles();

			if (result.Count == 0)
			{
				Console.WriteLine("Parking is empty");
			}

			foreach (var vehicle in result)
			{
				Console.WriteLine(vehicle.Id + '|' + vehicle.VehicleType + '|' + vehicle.Balance);
			}

			MainMenu();
		}

		private static void ShowParkingBalance()
		{

			//HttpMethod httpMethod = new HttpMethod("GET");
			//HttpRequestMessage request = new HttpRequestMessage(httpMethod, "https://localhost:5001/api/weatherforecast/");
			//var response = _client.Send(request);


			//var result = await _client.GetAsync("https://localhost:5001/api/weatherforecast/");
			//Console.WriteLine(result.Content);
			//var response = _client.GetStringAsync("api/parking/balance");
			//Console.WriteLine(response.StatusCode + "|" + response.Content.ToString());
			//Console.WriteLine("Parking balance: " + _parkingService.GetBalance() + "\n");
			MainMenu();
		}

		private static void TopUp()
		{
			if (_parkingService.GetVehicles().Count == 0)
			{
				Console.WriteLine("Parking is empty");
				return;
			}

			decimal sum = 0;
			string id = "";

			Console.WriteLine("Enter the top-up amount, please: ");

			while (!decimal.TryParse(Console.ReadLine(), out sum) || sum < 1 || sum > 10000)
			{
				Console.WriteLine("Invalid value. Try againg, please.");
			}

			do
			{
				Console.WriteLine("Enter the vehicle id: ");
				id = Console.ReadLine();
			} while (!Vehicle.IsId(id));

			_parkingService.TopUpVehicle(id, sum);

			MainMenu();
		}

		private static void Remove()
		{
			string id = "";

			if (_parkingService.GetVehicles().Count == 0)
			{
				Console.WriteLine("Parking is empty");
				return;
			}

			do
			{
				Console.WriteLine("Enter the vehicle id: ");
				id = Console.ReadLine();
			} while (!Vehicle.IsId(id));

			_parkingService.RemoveVehicle(id);

			MainMenu();
		}

		private static void ShowNumberOfFreeParkingSpaces()
		{
			var result = _parkingService?.GetFreePlaces();

			Console.WriteLine($"{result} parking spaces are busy out of 10");
		}

		private static void Add()
		{
			int vehicleType = 0;
			decimal balance = 0;
			string id = Vehicle.GenerateRandomRegistrationPlateNumber();

			Console.WriteLine("Chose Vehicle Type: \n" +
								 "0. Passenger Car \n" +
								 "1. Truck \n" +
								 "2. Bus \n" +
								 "3. Motorcycle");

			while (!int.TryParse(Console.ReadLine(), out vehicleType) || vehicleType < 0 || vehicleType > 3)
			{
				Console.WriteLine("Invalid value. Try againg, please.");
			}

			Console.WriteLine("How much money will be on the balance?");

			while (!decimal.TryParse(Console.ReadLine(), out balance) || balance < 1 || balance > 10000)
			{
				Console.WriteLine("Invalid value.Try againg, please.");
			}

			_parkingService?.AddVehicle(new Vehicle(id, (VehicleType)vehicleType, balance));

			Console.WriteLine($"The vehicle added successfully. This is vehicle id: {id}\n");

			_withdrawTimer.FireElapsedEvent();

			MainMenu();
		}
	}
}
