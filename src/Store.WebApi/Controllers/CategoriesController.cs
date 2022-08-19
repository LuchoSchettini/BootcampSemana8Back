using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Exceptions;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.Interfaces;
using Store.ApplicationCore.ResponsesApi;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        ////private readonly ICategoryRepository categoryRepository;

        ////public CategoriesController(ICategoryRepository categoryRepository)
        ////{
        ////    this.categoryRepository = categoryRepository;
        ////}

        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;

            _logger.LogInformation("******Prueba -> Hola desde Serilog en el Controller de CategoriesController******");
        }



        /// <summary>
        /// Get all categories
        /// </summary>
        /// <response code="200">Returns the categories</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryResponse>))]
        //public ActionResult<IReadOnlyList<CategoryResponse>> GetAll()
        public ActionResult GetAll()
        {
            try
            {
                //antes devoliva un List

                //return Ok(new ApiResponseOk(200, unitOfWork.Categories.GetAll()));
                //return Ok(new ApiResponseOk(200));
                return Ok(unitOfWork.Categories.GetAll());
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponseError(404, ex.Message));
            }
            catch (ConflictDbException ex)
            {
                return Conflict(new ApiResponseError(409, ex.Message));
            }
        }



        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <response code="200">Returns the existing category</response>
        /// <response code="404">If the category doesn't exist</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetById(int id)
        {
            try
            {
                var category = unitOfWork.Categories.GetById(id);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponseError(404, ex.Message));
            }
            catch (ConflictDbException ex)
            {
                return Conflict(new ApiResponseError(409, ex.Message));
            }
        }


        /// <summary>
        /// Create a category
        /// </summary>
        /// <param name="request">Category data</param>
        /// <response code="201">Returns the created category</response>
        /// <response code="400">If the data doesn't pass the validations</response>
        /// <response code="404">If the category doesn't exist</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategoryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Create(CreateCategoryRequest request)
        {
            try
            {
                var category = unitOfWork.Categories.Create(request);


                //  Prefiero que se haga el SaveChange del context en cada metodo del Repository
                //  asi mantenemos que cada cosa se haga en su respectivo lugar, por ejemplo si
                //  hay que actualizar mas de un cosa en el la bbdd, lo hacemos en el metodo del
                //  repository y asi toda la logica se queda en el metodo
                //unitOfWork.SaveAsync();

                /////return Ok(category);
                return StatusCode(201, category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponseError(404, ex.Message));
            }
            catch (ConflictDbException ex)
            {
                return Conflict(new ApiResponseError(409, ex.Message));
            }
        }




        /// <summary>
        /// Update a product by id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <param name="request">Category data</param>
        /// <response code="200">Returns the updated category</response>
        /// <response code="400">If the data doesn't pass the validations</response>
        /// <response code="404">If the category doesn't exist</response>
        /// <response code="409">Conflict with unique indexes or primary keys</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult UpdateById(int id, UpdateCategoryRequest request)
        {
            try
            {
                var category = unitOfWork.Categories.UpdateById(id, request);

                //  Prefiero que se haga el SaveChange del context en cada metodo del Repository
                //  asi mantenemos que cada cosa se haga en su respectivo lugar, por ejemplo si
                //  hay que actualizar mas de un cosa en el la bbdd, lo hacemos en el metodo del
                //  repository y asi toda la logica se queda en el metodo
                //unitOfWork.SaveAsync();

                //return Ok(category);
                //20220817 1
                //return StatusCode(201, category);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponseError(404, ex.Message));
            }
            catch (ConflictDbException ex)
            {
                return Conflict(new ApiResponseError(409, ex.Message));
            }
        }



        /// <summary>
        /// Delete a category by id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <response code="204">If the category was deleted</response>
        /// <response code="404">If the category doesn't exist</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteById(int id)
        {
            try
            {
                unitOfWork.Categories.DeleteById(id);

                //  Prefiero que se haga el SaveChange del context en cada metodo del Repository
                //  asi mantenemos que cada cosa se haga en su respectivo lugar, por ejemplo si
                //  hay que actualizar mas de un cosa en el la bbdd, lo hacemos en el metodo del
                //  repository y asi toda la logica se queda en el metodo
                //unitOfWork.SaveAsync();


                //return NoContent();
                return Ok("Registro eliminado correctamente.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponseError(404, ex.Message));
            }
            catch (ConflictDbException ex)
            {
                return Conflict(new ApiResponseError(409, ex.Message));
            }
        }



        /// <summary>
        /// Get all categories (Filters)
        /// </summary>
        /// <response code="200">Returns the categories (Filters)</response>
        [HttpGet("GetAllFilters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagerResponse<CategoryResponse>>> GetAllFilters([FromQuery] PagerRequest pagerRequest)
        {
            try
            {
                var resultado = await unitOfWork.Categories
                            .GetAllFiltringAsync(pagerRequest);

                Response.Headers.Add("X-InlineCount", resultado.Total.ToString());

                return Ok(resultado);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponseError(404, ex.Message));
            }
            catch (ConflictDbException ex)
            {
                return Conflict(new ApiResponseError(409, ex.Message));
            }
        }


    }
}