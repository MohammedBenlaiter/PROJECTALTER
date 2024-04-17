using Microsoft.EntityFrameworkCore;

namespace PROJECTALTERAPI.Services;

public class UserService
{
    private readonly AlterDbContext _db;
    public UserService(AlterDbContext db)
    {
        _db = db;
    }
    public async Task<User?> Authenticate(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null && VerifyPasswordHash(password, user.Password))
        {
            return user;
        }
        return null;
    }
    private bool VerifyPasswordHash(string password, string storedHash)
    {
        return true;
    }
}
