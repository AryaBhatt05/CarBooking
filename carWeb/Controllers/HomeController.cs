using carWeb.Models;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FireSharp.Response;
using Google.Apis.Auth.OAuth2;

namespace carWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

		IFirebaseConfig config = new FirebaseConfig
		{
			AuthSecret = "oQzNmo5wGiZpBSdo9gQzO9v2qWvuMdRa3vtt1Hbd",
			BasePath = "https://vehicledb-48bf7-default-rtdb.asia-southeast1.firebasedatabase.app"

		};
		IFirebaseClient client;

		public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<CarModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<CarModel>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }

        public IActionResult Add()
        {
            return View();
        }
		[HttpPost]
		public IActionResult Add(CarModel data)
		{
			try
			{
				client = new FireSharp.FirebaseClient(config);
				PushResponse response = client.Push("Users/", data);
				data.Id = response.Result.name;
				SetResponse setResponse = client.Set("Users/" + data.Id, data);
				if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
				{
					ModelState.AddModelError(string.Empty, "Added Succesfully");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Something went wrong!!");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
			}
            return RedirectToAction("Index");
        }

        public IActionResult Gallery()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + id);
            CarModel data = JsonConvert.DeserializeObject<CarModel>(response.Body);
            return View(data);
        }


        [HttpPost]
        public ActionResult Edit(CarModel model)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Users/" + model.Id, model);
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Users/" + id);
            return RedirectToAction("Index");
        }


    } 
}