using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspNetCore.MongoDB;
using Sample.Models;

namespace Sample.Controllers
{
    [Produces("application/json")]
    [Route("api/Sample")]
    public class SampleController : Controller
    {
        IMongoOperation<SampleModel> _operation;

       
        public SampleController(IMongoOperation<SampleModel> operation)
        {
            _operation = operation;
        }

        [HttpPost("save")]
        [Authorize]
        public async Task<JsonResult> SaveSample([FromBody] SampleModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    await _operation.SaveAsync(model);
                    return new JsonResult("Save Successful");
                }

                return new JsonResult("Model is invalid") { StatusCode = 401 };

            }
            catch (Exception ex)
            {
                return new JsonResult(ex) { StatusCode = 401 };
            }
        }

        [HttpGet("get")]
        public async Task<IEnumerable<SampleModel>> GetAll()
        {
            return await _operation.GetAllAsync();
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _operation.GetByIdAsync(id);

            if (model == null)
            {
                return new JsonResult("Data not found") { StatusCode = 401 };
            }

            return Ok(model);
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SampleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;

                    await _operation.UpdateAsync(id,model);
                    return new JsonResult("Save Successful");
                }

                return new JsonResult("Model is invalid") { StatusCode = 400 };

            }
            catch (Exception ex)
            {
                return new JsonResult(ex) { StatusCode = 400 };
            }
        }
    }
}