using Microsoft.AspNetCore.Identity;

using WebStore.Domain.Entities.Identity;

namespace WebStore.Domain.DTO.Identity;

public abstract class UserDTO
{
    public User User { get; init; } = null!;
}

public class AddLoginDTO : UserDTO
{
    public UserLoginInfo UserLoginInfo { get; init; } = null!;
}

public class PasswordHashDTO : UserDTO
{
    public string Hash { get; init; } = null!;
}

// время окончания блокировки
public class SetLockOutDTO : UserDTO
{
    // время с учетом временного пояса
    public DateTimeOffset? LockoutEnd { get; init; }
}