using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Production.Api.Data;
using Production.Api.Exceptions;
using Production.Api.Models;

namespace Production.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly AdventureWorksContext _context;

        public ProductsController(AdventureWorksContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [EnableQuery]
        public IQueryable<Product> GetAll()
        {
            return _context.Product.AsNoTracking();
        }

        /// <summary>
        /// Get single product.
        /// </summary>
        /// <response code="200">OK.</response>
        /// <response code="404">Not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Route("{key}")]
        [EnableQuery]
        public async Task<SingleResult<Product>> GetSingle([FromODataUri][Required] int key)
        {
            var productExists = await _context.Product.AnyAsync(p => p.ProductId == key);
            if (!productExists)
            {
                throw new EntryNotFoundException(key);
            }

            return new SingleResult<Product>(
                _context.Product
                    .Where(p => p.ProductId == key));
        }

        /// <summary>
        /// Update product entry.
        /// </summary>
        /// <response code="204">No content.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put([FromBody] ProductUpdateModel model, [FromRoute][Required] int id)
        {
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                throw new EntryNotFoundException(id);
            }

            _mapper.Map(model, product);

            product.ModifiedDate = DateTime.UtcNow;
            _context.Update(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Create new product.
        /// </summary>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreateModel model)
        {
            var product = _mapper.Map<Product>(model);

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return Created(HttpContext.Request.Path, product.ProductId);
        }
    }
}
