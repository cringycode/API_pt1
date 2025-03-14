﻿using System.Linq.Expressions;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Repository;

public class Repo<T> : IRepo<T> where T : class
{
    #region DI

    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repo(ApplicationDbContext db)
    {
        _db = db;
        // _db.VillaNumbers.Include(u => u.Villa).ToList();
        this.dbSet = _db.Set<T>();
    }

    #endregion

    public async Task CreateAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        await SaveAsync();
    }

    public async Task<T> GetAsync
        (Expression<Func<T, bool>> filter = null, bool tracked = true, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync
        (Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.ToListAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        dbSet.Remove(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}