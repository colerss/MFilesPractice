using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameVaultApp.Models;
using GameVaultApp.Data;
using GameVaultApp.ViewModels;
using MFaaP.MFWSClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameVaultApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static string savedSort = "Title";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> SortBy(string sortOrder, TestViewModel viewModel)
        {

            viewModel.Objects = MFilesAPI.instance.GetAllObjectsWithProperties().ToList();
            if (sortOrder == savedSort)
            {
                sortOrder += "_desc";
            }

            savedSort = sortOrder;
            switch (savedSort)
            {
                case "Title_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.Title).ToList();
                    break;
                case "ID_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.DisplayID).ToList();
                    break;
                case "Class_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.Class).ToList();
                    break;
                case "Version_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.ObjVer.Version).ToList();
                    break;
                case "ID":
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.DisplayID).ToList();
                    break;
                case "Class":
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Class).ToList();
                    break;
                case "Version":
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.ObjVer.Version).ToList();
                    break;
                default:
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Title).ToList();
                    break;
            }

            ViewData["ObjectType"] = new SelectList(MFilesAPI.instance.GetAllObjectTypes(), "ID", "Name");
            return View("Index", viewModel);
        }
        public async Task<IActionResult> SortByClass(string sortOrder, TestViewModel viewModel)
        {

            viewModel.Objects = MFilesAPI.instance.GetAllObjectsWithProperties(objClass: MFilesAPI.Class.Videogame).ToList();
            if (sortOrder == savedSort)
            {
                sortOrder = "-" + sortOrder;
            }

            savedSort = sortOrder;
            if (sortOrder.Contains("Title"))
            {
                if (sortOrder == "Title")
                {
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Title).ToList();
                }
                else
                {
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.Title).ToList();
                }
            }
            else
            {
                int targetProp = viewModel.Objects[0].Properties.IndexOf(
                    viewModel.Objects[0].Properties.Where(x => x.PropertyDef == Math.Abs(int.Parse(sortOrder))).First()
                    );
                
                if (sortOrder.Contains('-'))
                {
                    
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.Properties[targetProp].TypedValue.DisplayValue).ToList();
                }
                else
                {
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Properties[targetProp].TypedValue.DisplayValue).ToList();
                }
            }

            ViewData["ObjectType"] = new SelectList(MFilesAPI.instance.GetAllObjectTypes(), "ID", "Name");
            return View("Games", viewModel);
        }
        public IActionResult Index()
        {
            TestViewModel viewModel = new TestViewModel();
            viewModel.Objects = MFilesAPI.instance.GetAllObjectsWithProperties().ToList();
            viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Title).ToList();
            ViewData["ObjectType"] = new SelectList(MFilesAPI.instance.GetAllObjectTypes(), "ID", "Name");
            return View(viewModel);
        }
        public IActionResult Games()
        {
            TestViewModel viewModel = new TestViewModel();
            viewModel.Objects = MFilesAPI.instance.GetAllObjectsWithProperties(objClass: MFilesAPI.Class.Videogame).ToList();
            viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Title).ToList();
            ViewData["ObjectType"] = new SelectList(MFilesAPI.instance.GetAllObjectTypes(), "ID", "Name");
            return View(viewModel);
        }
        public async Task<IActionResult> Search(TestViewModel viewModel)
        {
            viewModel.Objects = MFilesAPI.instance.GetAllObjectsWithProperties(viewModel.ObjectSearchTitle, viewModel.ObjectSearchType).ToList();
            switch (savedSort)
            {
                case "Title_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.Title).ToList();
                    break;
                case "ID_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.DisplayID).ToList();
                    break;
                case "Class_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.Class).ToList();
                    break;
                case "Version_desc":
                    viewModel.Objects = viewModel.Objects.OrderByDescending(obj => obj.ObjVer.Version).ToList();
                    break;
                case "ID":
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.DisplayID).ToList();
                    break;
                case "Class":
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Class).ToList();
                    break;
                case "Version":
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.ObjVer.Version).ToList();
                    break;
                default:
                    viewModel.Objects = viewModel.Objects.OrderBy(obj => obj.Title).ToList();
                    break;
            }

            ViewData["ObjectType"] = new SelectList(MFilesAPI.instance.GetAllObjectTypes(), "ID", "Name");
            
            return View("Index", viewModel);
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
