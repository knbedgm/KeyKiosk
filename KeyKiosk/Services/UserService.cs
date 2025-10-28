using KeyKiosk.Data;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;

    public UserService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users
            .OrderBy(u => u.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User> AddAsync(User newUser)
    {
        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return newUser;
    }

    public async Task UpdateAsync(int id, User updated)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return;

        u.Name = updated.Name ?? u.Name;
        u.Pin = updated.Pin ?? u.Pin;
        u.UserType = updated.UserType;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return;

        _db.Users.Remove(u);
        await _db.SaveChangesAsync();
    }
}
