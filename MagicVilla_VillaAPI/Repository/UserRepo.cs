using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepo : IUserRepo
{
    #region DI

    private readonly ApplicationDbContext _db;

    public UserRepo(ApplicationDbContext db)
    {
        _db = db;
    }

    #endregion

    #region IS UNIQUE

    public bool IsUniqueUser(string username)
    {
        var user = _db.LocalUsers.FirstOrDefault(u => u.UserName == username);
        if (user is null)
        {
            return true;
        }

        return false;
    }

    #endregion

    public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        throw new NotImplementedException();
    }

    public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        LocalUser user = new LocalUser()
        {
            UserName = registerationRequestDTO.UserName,
            Password = registerationRequestDTO.Password,
            Name = registerationRequestDTO.Name,
            Role = registerationRequestDTO.Role,
        };
        _db.LocalUsers.Add(user);
        await _db.SaveChangesAsync();
        user.Password = "";
        return user;
    }
}