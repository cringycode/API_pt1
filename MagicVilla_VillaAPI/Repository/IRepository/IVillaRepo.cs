using System.Linq.Expressions;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository.IRepository;

public interface IVillaRepo : IRepo<Villa>
{
    Task<Villa> UpdateAsync(Villa entity);
}