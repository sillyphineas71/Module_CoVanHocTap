using Microsoft.AspNetCore.Mvc;
using ApiCoVanHocTap.Services;

namespace ApiCoVanHocTap.Controllers
{
    [Route("api/phan-cong")]
    public class PhanCongCoVanController : BaseController
    {
        public PhanCongCoVanController(IServiceWrapper serviceWrapper) : base(serviceWrapper)
        { }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] string? id_cb,
            [FromQuery] string? id_sv
        )
        {
            var response = await _serviceWrapper.PhanCongCoVan.GetList(id_cb, id_sv);
            return response.ToActionResult();
        }
    }
}