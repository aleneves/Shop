using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


[Route("products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> Get(
        [FromServices] DataContext context
    )
    {
        var products = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .ToListAsync();

        return products;        
    }

    [HttpGet] 
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Product>> GetByID(int id,
            [FromServices] DataContext context)
    {
        var product = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return product;        
    }

    [HttpGet] //products/categories/1
    [Route("categories/{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> GetByCategory(int id,
            [FromServices] DataContext context)
    {
        var products = await context
            .Products
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(x => x.CategoryId == id)
            .ToListAsync();

        return products;        
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Product>>> Post(
        [FromBody]Product model,
        [FromServices] DataContext context)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            //Adicionando a categoria no BD
            context.Products.Add(model);

            //salvando as alterações de forma assincrona.
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch 
        {
            return BadRequest(new { message = "Não foi possível criar o produto." });
        }

    }
}
    
