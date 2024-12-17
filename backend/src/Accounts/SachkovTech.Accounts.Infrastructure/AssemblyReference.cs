using System.Reflection;

namespace SachkovTech.Accounts.Infrastructure;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}