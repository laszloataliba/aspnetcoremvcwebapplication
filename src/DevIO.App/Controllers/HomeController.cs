using DevIO.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DevIO.App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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

        [Route("error/{id:length(3,3)}")]
        public IActionResult Errors(int id)
        {
            var errorModel = new ErrorViewModel();

            if (id == 500)
            {
                errorModel.Message = "Ocorreu um erro! Tente novamente mais tarde ou contate nosso suporte.";
                errorModel.Title = "Ocorreu um erro!";
                errorModel.ErrorCode = id;
            }
            else if (id == 404)
            {
                errorModel.Message = "A página que está procurando não existe! <br />Em caso de dúvidas entre em contato com nosso suporte.";
                errorModel.Title = "Ops! Página não encontrada.";
                errorModel.ErrorCode = id;
            }
            else if (id == 403)
            {
                errorModel.Message = "Você não tem permissão para fazer isto.";
                errorModel.Title = "Acesso Negado";
                errorModel.ErrorCode = id;
            }
            else
                return StatusCode(500);

            return View("Error", errorModel);
        }
    }
}
