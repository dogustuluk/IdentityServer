using IdentityServer.API2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IdentityServer.API2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetPicture()
        {
            var pictures = new List<Picture>()
            {
                new Picture() {Id=1,Name="resim 1", Url="resim1.jpg"},
                new Picture() {Id=2,Name="resim 2", Url="resim2.jpg"},
                new Picture() {Id=3,Name="resim 3", Url="resim3.jpg"},
                new Picture() {Id=4,Name="resim 4", Url="resim4.jpg"},
                new Picture() {Id=5,Name="resim 5", Url="resim5.jpg"}
            };
            return Ok(pictures);
        }
    }
}
