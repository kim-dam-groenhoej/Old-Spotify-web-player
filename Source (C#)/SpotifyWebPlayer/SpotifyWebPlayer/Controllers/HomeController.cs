using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpotifyWebPlayer.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestSong(string songURL)
        {
            // Cache request
            var data = new
            {
                data = new
                {
                    success = true,
                    rows = new[] {
                        new {
                            ID = Guid.NewGuid().ToString(), // Is requried for repeating the song again
                            SpotifyURL = songURL
                        }
                    }
                }
            };

            // Cache song
            ControllerContext.HttpContext.Cache.Insert("song", data);

            // return true for success of request
            var result = new {
                           data = new {
                                success = true
                           }
                       };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSong()
        {
            // get cached request
            var data = ControllerContext.HttpContext.Cache["song"];

            if (data == null)
            {
                // play default song
                data = new
                {
                    data = new
                    {
                        success = true,
                        rows = new[] {
                        new {
                            ID = "defaultSong", // Is requried for repeating the song again
                            SpotifyURL = "spotify:track:4hkZMpR9ptEQLqlIorFPeN"
                        }
                    }
                    }
                };
            }

            // Convert to JSON and return it as result for the request
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
