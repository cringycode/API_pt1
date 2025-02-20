using System.Linq.Expressions;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository;

public class VillaRepo : Repo<Villa>, IVillaRepo
{
    #region DI

    private readonly ApplicationDbContext _db;

    public VillaRepo(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    #endregion

    public async Task<Villa> UpdateAsync(Villa entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _db.Villas.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}