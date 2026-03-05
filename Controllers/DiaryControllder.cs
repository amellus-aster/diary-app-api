using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiaryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DiaryApi.Controller;

using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class DiaryController : ControllerBase
{
    private readonly DiaryDbContext _context;
    public DiaryController(DiaryDbContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetDiary()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var diaries = await _context.Diaries
    .Where(d => d.UserId == userId)
    .ToListAsync();
        return Ok(diaries);
    }
    [Authorize]
    [HttpPost("add")]
    public async Task<ActionResult<DiaryModel>> AddDiary(DiaryRequest diaryRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var entry = new DiaryModel
        {
            UserId = userId,
            Title = diaryRequest.Title,
            Content = diaryRequest.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Diaries.AddAsync(entry);
        await _context.SaveChangesAsync();
        return Ok(entry);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<DiaryModel>> UpdateDiary(int id, DiaryRequest diaryRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var entry = await _context.Diaries
        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (entry == null)
            return NotFound();
        entry.Title = diaryRequest.Title;
        entry.Content = diaryRequest.Content;
        entry.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(entry);
    }
    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteMany([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest();

        var diaries = await _context.Diaries
            .Where(d => ids.Contains(d.Id))
            .ToListAsync();

        _context.Diaries.RemoveRange(diaries);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}