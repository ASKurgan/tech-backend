using SharedKernel;

namespace AccountService.Domain;

public static class UserErrors
{
    public static ErrorList UserAlreadyExist()
    {
        return Error.Validation("user.already.exists", "Пользователь с таким именем пользователя уже существует.");
    }
}