using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreRestPermutations.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ArrayController : Controller
    {
        private ArrayPermutations _arraylogic;

        // POST api/array/Permutations
        [HttpPost]
        public JsonResult Permutations(string arr)
        {
            if (string.IsNullOrEmpty(arr))
                return Json(new List<ReturnValue>());
            
            var array = JArray.Parse(arr).Select(p=> p.ToString()).ToList();

            _arraylogic = new ArrayPermutations();
            return Json(_arraylogic.GetPermutationList(array).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            _arraylogic?.Dispose();
            base.Dispose(disposing);
        }
    }
}
