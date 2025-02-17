namespace SharedKernel;

public static partial class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            string label = name ?? "значение";
            return Error.Validation("value.is.invalid", $"{label} недействительно");
        }

        public static Error NotFound(Guid? id = null, string? name = null)
        {
            string forId = id == null ? string.Empty : $" по Id '{id}'";
            return Error.NotFound("record.not.found", $"{name ?? "запись"} не найдена{forId}");
        }

        public static Error ValueIsRequired(string? name = null)
        {
            string label = name == null ? string.Empty : " " + name + " ";
            return Error.Validation("length.is.invalid", $"поле{label}обязательно");
        }

        public static Error AlreadyExist()
        {
            return Error.Validation("record.already.exist", "Запись уже существует");
        }

        public static Error Failure()
        {
            return Error.Failure("server.failure", "Серверная ошибка");
        }
    }

    public static class Auth
    {
        public static Error InvalidCredentials()
        {
            return Error.Validation("credentials.is.invalid", "Неверные учетные данные");
        }

        public static Error InvalidRole()
        {
            return Error.Failure("role.is.invalid", "Неверная роль пользователя");
        }

        public static Error InvalidToken()
        {
            return Error.Validation("token.is.invalid", "Ваш токен недействителен");
        }

        public static Error ExpiredToken()
        {
            return Error.Validation("token.is.expired", "Ваш токен истек");
        }
    }
}