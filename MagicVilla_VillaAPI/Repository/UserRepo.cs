using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepo : IUserRepo
{
    #region DI

    private readonly ApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private string secretKey;

    public UserRepo(ApplicationDbContext db, IConfiguration configuration, UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
        _userManager = userManager;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }

    #endregion

    #region IS UNIQUE

    public bool IsUniqueUser(string username)
    {
        var user = _db.AppUsers.FirstOrDefault(u => u.UserName == username);
        if (user is null)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region LOGIN

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = _db.AppUsers.FirstOrDefault
            (u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (user is null || isValid is false)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = null
            };
        }

        // if user was found generate JWT Token 

        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = _mapper.Map<UserDTO>(user),
            Role = roles.FirstOrDefault()
        };
        return loginResponseDTO;
    }

    #endregion

    #region REGISTER

    public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        AppUser user = new()
        {
            UserName = registerationRequestDTO.UserName,
            Email = registerationRequestDTO.UserName,
            NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
            Name = registerationRequestDTO.Name
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "admim");
                var userToReturn = _db.AppUsers.FirstOrDefault
                    (u => u.UserName == registerationRequestDTO.UserName);
                return _mapper.Map<UserDTO>(userToReturn);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new UserDTO();
    }

    #endregion
}