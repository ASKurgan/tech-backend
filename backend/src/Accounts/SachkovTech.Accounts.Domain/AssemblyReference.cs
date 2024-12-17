using System.Reflection;

namespace SachkovTech.Accounts.Domain;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}