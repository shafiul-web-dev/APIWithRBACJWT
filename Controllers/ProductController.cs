using APIWithRBACJWT.Data;
using APIWithRBACJWT.DTO;
using APIWithRBACJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace APIWithRBACJWT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
	private readonly AppDbContext _context;

	public ProductController(AppDbContext context)
	{
		_context = context;
	}

	// Public for both Admin & User
	[Authorize(Roles = "Admin,User")]
	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var products = await _context.Products.ToListAsync();
		return Ok(products);
	}

	// Admin only
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IActionResult> Create(ProductDto dto)
	{
		var product = new Product
		{
			Name = dto.Name,
			Description = dto.Description,
			Price = dto.Price
		};

		_context.Products.Add(product);
		await _context.SaveChangesAsync();

		return Ok(product);
	}

	// Admin only
	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, ProductDto dto)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null) return NotFound();

		product.Name = dto.Name;
		product.Description = dto.Description;
		product.Price = dto.Price;

		await _context.SaveChangesAsync();
		return Ok(product);
	}

	// Admin only
	[Authorize(Roles = "Admin")]
	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null) return NotFound();

		_context.Products.Remove(product);
		await _context.SaveChangesAsync();
		return NoContent();
	}
}