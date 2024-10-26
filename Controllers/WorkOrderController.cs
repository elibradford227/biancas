using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BiancasBikes.Data;
using Microsoft.EntityFrameworkCore;
using BiancasBikes.Models;
using BiancasBikes.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BiancasBikes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrderController : ControllerBase
{
  private BiancasBikesDbContext _dbContext;

  public WorkOrderController(BiancasBikesDbContext context)
  {
    _dbContext = context;
  }

  [HttpGet("incomplete")]
  [Authorize]
  public IActionResult GetIncompleteWorkOrders()
  {
    return Ok(_dbContext.WorkOrders
    .Include(wo => wo.Bike)
    .ThenInclude(b => b.Owner)
    .Include(wo => wo.Bike)
    .ThenInclude(b => b.BikeType)
    .Include(wo => wo.UserProfile)
    .Where(wo => wo.DateCompleted == null)
    .OrderBy(wo => wo.DateInitiated)
    .ThenByDescending(wo => wo.UserProfileId == null).ToList());
  }

  [HttpPost]
  [Authorize]
  public IActionResult CreateWorkOrder(WorkOrder workOrder)
  {
    workOrder.DateInitiated = DateTime.Now;
    _dbContext.WorkOrders.Add(workOrder);
    _dbContext.SaveChanges();
    return Created($"/api/workorder/{workOrder.Id}", workOrder);
  }
}