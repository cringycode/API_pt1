using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepo : IUserRepo
{
    #region DI

    private readonly ApplicationDbContext _db;
    private string secretKey;

    public UserRepo(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
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

    #region LOGIN

    public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = _db.LocalUsers.FirstOrDefault
        (u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() &&
              u.Password == loginRequestDTO.Password);

        if (user is null)
        {
            return null;
        }
    }

    #endregion

    #region REGISTER

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

    #endregion
}