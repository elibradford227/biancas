using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BiancasBikes.Data;
using Microsoft.EntityFrameworkCore;
using BiancasBikes.Models;
using BiancasBikes.Models.DTOs;

namespace BiancasBikes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikeController : ControllerBase
{
    private BiancasBikesDbContext _dbContext;

    public BikeController(BiancasBikesDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    [Authorize]
    [HttpGet]
    //[Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.Bikes.Include(b => b.Owner).ToList());
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        Bike bike = _dbContext
            .Bikes
            .Include(b => b.Owner)
            .Include(b => b.WorkOrders)
            .SingleOrDefault(b => b.Id == id);
        
        if (bike == null)
        {
            return NotFound();
        }

        return Ok(bike);
    }

    [HttpGet("inventory")]
    [Authorize]
    public IActionResult GetInventory()
    {
        int inventory = _dbContext
            .Bikes
            .Where(b => b.WorkOrders.Any(wo => wo.DateCompleted == null))
            .Count();

        if (inventory <= 0)
        {
            return NotFound("No bikes were present in inventory");
        }

        return Ok(inventory);
    }

}