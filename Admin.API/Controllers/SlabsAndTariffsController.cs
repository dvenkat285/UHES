using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MediatR;
using Admin.Infrastructure.Shared.Models;
using static Admin.Application.CommandQueries.Queries;
//using Admin.Application.Features;
using Admin.Application.CommandHandlers;



namespace Admin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlabsAndTariffsController : ControllerBase
    {
        private readonly IMediator mediator;

        public SlabsAndTariffsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet(Name = "GetAllSubcategoryWiseSlabTariffDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(SlabsDetailsModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<SlabsDetailsModel>>> GetAllSubcategoryWiseSlabTariffDetails()
        {
            var query = new GetAllSubcategoryWiseSlabTariffDetailsQuery();
            var result = await mediator.Send(query);

            if (result == null || !result.Any())
            {
                return NotFound("No records found.");
            }

            return this.Ok(result);

        }


        //get all categories for dropdown selection in insert function
        [HttpGet("GetCategories", Name = "GetCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await mediator.Send(new GetAllCategoriesQuery());
            if (result == null || !result.Any())
                return NotFound("No records found.");
            return Ok(result);
        }


        [HttpPost("CreateCategoryWithSubcategoryAndSlab", Name = "CreateCategoryWithSubcategoryAndSlab")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategoryWithSubcategoryAndSlab([FromBody] CategoryCreateDto dto)
        {
            var command = new CreateSlabCommand(dto);
            var result = await mediator.Send(command);

            if (result == null)
                return BadRequest("Creation failed.");

            //return CreatedAtRoute("GetCategoryallById", new { categoryId = result.CategoryId }, result);
            return Created("", result);

        }

        [HttpGet("GetSlabByCategorySubcategoryAndLines", Name = "GetSlabByCategorySubcategoryAndLines")]
        [ProducesResponseType(typeof(SlabsDetailsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SlabsDetailsModel>> GetSlab([FromQuery] int categoryId, [FromQuery] int subcategoryId, [FromQuery] string lines)
        {
            var query = new GetSlabByCategorySubcategoryAndLinesQuery(categoryId, subcategoryId, lines);
            var result = await mediator.Send(query);

            if (result == null)
                return NotFound($"Slab not found for CategoryId: {categoryId}, SubcategoryId: {subcategoryId}, Lines: {lines}");

            return Ok(result);
        }



        //Update Slab Tariff
        [HttpPut("UpdateCategoryWithSubcategoryAndSlab", Name = "UpdateCategoryWithSubcategoryAndSlab")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategoryWithSubcategoryAndSlab([FromBody] CategoryUpdateDto dtos)
        {
            var command = new UpdateSlabCommand(dtos);
            var result = await mediator.Send(command);

            if (result == null)
                return NotFound("Update failed or category not found.");

            return Ok(result);
        }


    }
}




