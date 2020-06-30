using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("v1/categories")]
public class CategoryController : ControllerBase 
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult<List<Category>>> Get(
        [FromServices] DataContext context
    )
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);        
    }

    [HttpGet]
    [Route("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetByID(int id,
            [FromServices] DataContext context)
    {
        var categories = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return Ok(categories);        
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody]Category model,
        [FromServices] DataContext context)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            //Adicionando a categoria no BD
            context.Categories.Add(model);

            //salvando as alterações de forma assincrona.
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch 
        {
            return BadRequest(new { message = "Não foi possível criar a categoria." });
        }

    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Put(int id, 
        [FromBody]Category model,
        [FromServices] DataContext context)
    {
        if (model.Id == id)
            return NotFound(new { message = "Categoria não encontrada"});
        
        try
        {            
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //Informa que o estado do model é modificado.
            //Assim o framework ja verifica qual campo foi atualizado e persiste no banco de dados.
            context.Entry<Category>(model).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return Ok(model);        
        }
        catch(DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado."});
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível atualizar a categoria."});
        }

    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Delete(int id, 
        [FromServices] DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if(category == null)
            return NotFound(new { message = "Categoria não encontrada."});


        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(category);
        }
        catch
        {
            return BadRequest(new{message = "Não foi possível remover a categoria."});
        }

    }
}
