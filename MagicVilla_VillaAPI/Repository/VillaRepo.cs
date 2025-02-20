using System.Linq.Expressions;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository;

public class VillaRepo : IVillaRepo
{
    #region DI

    private readonly ApplicationDbContext _db;

    public VillaRepo(ApplicationDbContext db)
    {
        _db = db;
    }

    #endregion

    public async Task<List<Villa>> GetAll(Expression<Func<Villa, bool>> filter = null)
    {
        IQueryable<Villa> query = _db.Villas;
        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync();
    }

    public async Task<Villa> Get(Expression<Func<Villa, bool>> filter = null, bool tracked = true)
    {
        IQueryable<Villa> query = _db.Villas;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task Create(Villa entity)
    {
        await _db.Villas.AddAsync(entity);
        await Save();
    }

    public async Task Remove(Villa entity)
    {
        _db.Villas.Remove(entity);
        await Save();
    }

    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }
}