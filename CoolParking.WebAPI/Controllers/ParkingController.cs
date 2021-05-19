using CoolParking.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoolParking.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
    public class ParkingController : ControllerBase
    {
		private ParkingService _parkingService;
		public ParkingController(ParkingService parkingService)
		{
			 _parkingService = parkingService;
		}

		[HttpGet("Balance")]
		public ActionResult Balance()
		{
			//var result = JsonConvert.SerializeObject(_parkingService.GetBalance());
			return Ok(_parkingService.GetBalance());
		}
    }
}