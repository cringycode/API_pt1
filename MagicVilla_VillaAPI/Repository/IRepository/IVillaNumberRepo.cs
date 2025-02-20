using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository.IRepostiory
{
    public interface IVillaNumberRepo : IRepo<VillaNumber>
    {
      
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
  
    }
}