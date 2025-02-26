using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class VillaController : Controller
{
    #region DI

    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaController(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }

    #endregion

    #region INDEX

    [SuppressMessage("ReSharper.DPA", "DPA0011: High execution time of MVC action", MessageId = "time: 2468ms")]
    public async Task<IActionResult> IndexVilla()
    {
        List<VillaDTO> list = new();

        var response = await _villaService.GetAllAsync<APIResponse>();
        if (response != null && response.IsSucces)
        {
            list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
        }

        return View(list);
    }

    #endregion

    #region CREATE

    public async Task<IActionResult> CreateVilla()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaService.CreateAsync<APIResponse>(model);
            if (response != null && response.IsSucces)
            {
                TempData["success"] = "Villa created successfully!";
                return RedirectToAction(nameof(IndexVilla));
            }
        }

        TempData["error"] = "error occured!";
        return View(model);
    }

    #endregion

    #region UPDATE

    public async Task<IActionResult> UpdateVilla(int VillaId)
    {
        var response = await _villaService.GetAsync<APIResponse>(VillaId);
        if (response != null && response.IsSucces)
        {
            VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
            return View(_mapper.Map<VillaUpdateDTO>(model));
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
    {
        if (ModelState.IsValid)
        {
            TempData["success"] = "Villa updated successfully!";
            var response = await _villaService.UpdateAsync<APIResponse>(model);
            if (response != null && response.IsSucces)
            {
                return RedirectToAction(nameof(IndexVilla));
            }
        }

        TempData["error"] = "error occured!";
        return View(model);
    }

    #endregion

    #region DELETE

    public async Task<IActionResult> DeleteVilla(int VillaId)
    {
        var response = await _villaService.GetAsync<APIResponse>(VillaId);
        if (response != null && response.IsSucces)
        {
            VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
            return View(model);
        }


        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVilla(VillaDTO model)
    {
        var response = await _villaService.DeleteAsync<APIResponse>(model.Id);
        if (response != null && response.IsSucces)
        {
            TempData["success"] = "Villa deleted successfully!";
            return RedirectToAction(nameof(IndexVilla));
        }

        TempData["error"] = "error occured!";
        return View(model);
    }

    #endregion
}