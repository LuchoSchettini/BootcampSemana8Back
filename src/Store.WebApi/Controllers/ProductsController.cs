using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Exceptions;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.Interfaces;
using Store.ApplicationCore.ResponsesApi;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Store.WebApi.Controllers;



[ApiVersion("1.0")] //como esta version es la predeterminada no hay que indicarle nada en la url o header.
//[ApiVersion("1.1")] //si soporta otra version tambien hay que ponerla, como en este caso (osea algun medodo es de esta version)
[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
//Para restringir los formatos de respuesta, aplique el filtro [Produces]. Al igual que la mayoría de los filtros, [Produces] se puede aplicar en la acción, el controlador o el ámbito global:
[Produces("application/json")]
//[Authorize(Roles = "Administrador")]
//[Authorize]
public class ProductsController : ControllerBase
{
    ////private readonly IProductRepository productRepository;

    ////public ProductsController(IProductRepository productRepository)
    ////{
    ////    this.productRepository = productRepository;
    ////}

    private readonly IUnitOfWork unitOfWork;

    public ProductsController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }



    /// <summary>
    /// Get all products
    /// </summary>
    /// <response code="200">Returns the products</response>
    /// <response code="400">If the data doesn't pass the validations</response>
    [HttpGet]
    //[MapToApiVersion("1.1")] //si este metodo pertenece a la vesion 1.1 hay que ponerle este atributo (en caso de ser la verion prederminada no hace falta ponerle nada)
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<IReadOnlyList<ProductResponse>> GetAll()
    {
        try
        {
            return Ok(unitOfWork.Products.GetAll());
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
    /// Get a product by id
    /// </summary>
    /// <param name="id">Product id</param>
    /// <response code="200">Returns the existing product</response>
    /// <response code="400">If the data doesn't pass the validations</response>
    /// <response code="404">If the product doesn't exist</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult GetById(int id)
    {
        try
        {
            var product = unitOfWork.Products.GetById(id);
            return Ok(product);
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
    /// Create a product
    /// </summary>
    /// <param name="request">Product data</param>
    /// <response code="201">Returns the created product</response>
    /// <response code="400">If the data doesn't pass the validations</response>
    /// <response code="404">If the product doesn't exist</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult Create(CreateProductRequest request)
    {
        try
        {
            var product = unitOfWork.Products.Create(request);
            //unitOfWork.SaveAsync();

            //////return Ok(product);
            return StatusCode(201, product);
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
    /// <param name="id">Product id</param>
    /// <param name="request">Product data</param>
    /// <response code="200">Returns the updated product</response>
    /// <response code="400">If the data doesn't pass the validations</response>
    /// <response code="404">If the product doesn't exist</response>
    /// <response code="409">Conflict with unique indexes or primary keys</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult UpdateById(int id, UpdateProductRequest request)
    {
        try
        {
            var product = unitOfWork.Products.UpdateById(id, request);
            //unitOfWork.SaveAsync();

            return Ok(product);
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
    /// Delete a product by id
    /// </summary>
    /// <param name="id">Product id</param>
    /// <response code="204">If the product was deleted</response>
    /// <response code="400">If the data doesn't pass the validations</response>
    /// <response code="404">If the product doesn't exist</response>
    [HttpDelete("{id:int}")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult DeleteById(int id)
    {
        try
        {
            unitOfWork.Products.DeleteById(id);
            //unitOfWork.SaveAsync();

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

    ///// <summary>
    ///// Get all products (Filters)
    ///// </summary>
    ///// <response code="200">Returns the products (Filters)</response>
    //[HttpGet("GetAllFilters")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<ActionResult<PagerResponse<ProductResponse>>> GetAllFilters([FromQuery] PagerRequest productParams)
    //{
    //    var resultado = await unitOfWork.Product
    //                                .GetAllFiltringAsync(productParams.PageIndex, productParams.PageSize,
    //                                productParams.Search);

    //    //var listaProductosDto = _mapper.Map<List<ProductoListDto>>(resultado.registros);

    //    Response.Headers.Add("X-InlineCount", resultado.totalRegistros.ToString());

    //    var respuesta = new PagerResponse<ProductResponse>(resultado.registros, resultado.totalRegistros,
    //        productParams.PageIndex, productParams.PageSize, productParams.Search);

    //    return Ok(respuesta);
    //}



    /// <summary>
    /// Get all products (Filters)
    /// </summary>
    /// <response code="200">Returns the products (Filters)</response>
    [HttpGet("GetAllFilters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PagerResponse<ProductResponse>>> GetAllFilters([FromQuery] PagerRequest pagerRequest)
    {
        try
        {
            var resultado = await unitOfWork.Products
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