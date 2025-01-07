using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TouristSpotWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;  // �ޤJ EF Core ���R�W�Ŷ�

namespace TouristSpotWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Sightseeing_spotsContext _context; // �`�J DbContext

        // �`�J IHttpClientFactory �ӳЫ� HttpClient ���
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, Sightseeing_spotsContext context)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _context = context; // ���J ApplicationDbContext �H�i���Ʈw�ާ@
        }

        public IActionResult Index()
        {
            return View();
        }

        // ��ܴ��I�ԲӸ��
        public async Task<IActionResult> TouristSpotDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // �p�G id ���s�b�h��^ 404
            }

            // �Ы� HttpClient ���
            var client = _httpClientFactory.CreateClient();

            // �o�e GET �ШD�H������I�ԲӸ��
            var response = await client.GetAsync($"https://localhost:7015/api/TouristSpot/{id}");

            // �p�G�ШD���ѡA��^ 404 ����
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            // �ѪR API ��^�� JSON ���
            var spotData = await response.Content.ReadFromJsonAsync<TouristSpots>();

            // �N��ƶǻ�������
            return View(spotData);
        }

        // ��ܴ��I�޲z����
        //[Authorize] // �u���n�J���ϥΪ̥i�H�i�J����k
        public IActionResult Manage(string id)
        {
            // �ˬd�Τ�O�_�w�n�J
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User not logged in");
                // �p�G�S���n�J�A���w�V�ܵn�J����
                return RedirectToAction("Login", "Account");
            }

            var spot = _context.TouristSpots.FirstOrDefault(s => s.Id == id); // �q��Ʈw�d�䴺�I
            if (spot == null)
            {
                return NotFound();
            }
            return View(spot);
        }

        // ��s���I���
        [HttpPost]
        public IActionResult Update(TouristSpots spot)
        {
            if (ModelState.IsValid)
            {
                var existingSpot = _context.TouristSpots.FirstOrDefault(s => s.Id == spot.Id);
                if (existingSpot == null)
                {
                    return NotFound();
                }

                // ��s���I���ݩ�
                existingSpot.Name = spot.Name;
                existingSpot.Zone = spot.Zone;
                existingSpot.Toldescribe = spot.Toldescribe;
                existingSpot.Description = spot.Description;
                existingSpot.Tel = spot.Tel;
                existingSpot.Address = spot.Address;
                existingSpot.Zipcode = spot.Zipcode;
                existingSpot.Region = spot.Region;
                existingSpot.Town = spot.Town;
                existingSpot.Travellinginfo = spot.Travellinginfo;
                existingSpot.Opentime = spot.Opentime;
                existingSpot.Px = spot.Px;
                existingSpot.Py = spot.Py;
                existingSpot.Website = spot.Website;
                existingSpot.Parkinginfo = spot.Parkinginfo;
                existingSpot.Ticketinfo = spot.Ticketinfo;
                existingSpot.Remarks = spot.Remarks;

                _context.SaveChanges(); // �O�s��s

                return RedirectToAction("TouristSpotDetails", new { id = spot.Id }); // ��^�Բӭ�
            }

                return View("Manage", spot); // ���w��� "Manage" ���ϦӤ��O "Update"
        }

        //�j�M���I
        public IActionResult Search(string keyword, string region, string town)
        {
            // �w�]�j�M���G�� null �Ϊ�
            ViewBag.SearchResults = null;

            // ���o�ϰ�B����M��A�öǻ�������
            ViewBag.RegionList = _context.TouristSpots
                                          .Select(s => s.Region)
                                          .Distinct()
                                          .ToList();

            ViewBag.TownList = _context.TouristSpots
                                         .Select(s => s.Town)
                                         .Distinct()
                                         .ToList();

            // �u���b����r�B�ϰ�Ϋ����Ȫ����p�U�~����j�M
            if (!string.IsNullOrEmpty(keyword) || !string.IsNullOrEmpty(region) || !string.IsNullOrEmpty(town))
            {
                var query = _context.TouristSpots.AsQueryable();

                // �ھ�����r�L�o Description
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(s => s.Description.Contains(keyword));
                }

                // �ھڰϰ�L�o
                if (!string.IsNullOrEmpty(region))
                {
                    query = query.Where(s => s.Region.Contains(region));
                }

                //// �ھګ���L�o
                //if (!string.IsNullOrEmpty(town))
                //{
                //    query = query.Where(s => s.Town.Contains(town));
                //}

                // ����d�ߨ���o���G
                ViewBag.SearchResults = query.ToList();
                Console.WriteLine(ViewBag.SearchResults.Count);
            }

            return View();
        }








        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
