using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    #region DI

    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public VillaAPIController(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    #endregion

    #region GET ALL

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        IEnumerable<Villa> VillaList = await _db.Villas.ToListAsync();
        return Ok(_mapper.Map<List<VillaDTO>>(VillaList));
    }

    #endregion

    #region GET

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<VillaDTO>(villa));
    }

    #endregion

    #region CREATE

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
        if (createDTO is null)
        {
            return BadRequest(createDTO);
        }

        Villa model = _mapper.Map<Villa>(createDTO);


        await _db.Villas.AddAsync(model);
        await _db.SaveChangesAsync();
        return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
    }

    #endregion

    #region DELETE

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa is null)
        {
            return NotFound();
        }

        _db.Villas.Remove(villa);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    #endregion

    #region UPDATE

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
    {
        if (updateDTO == null || id != updateDTO.Id)
        {
            return BadRequest();
        }

        Villa model = _mapper.Map<Villa>(updateDTO);

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    #endregion

    #region UPDATE - PATCH

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

        if (villa is null)
        {
            return BadRequest();
        }

        patchDTO.ApplyTo(villaDTO, ModelState);
        Villa model = _mapper.Map<Villa>(villaDTO);

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return NoContent();
    }

    #endregion
}