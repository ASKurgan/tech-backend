using System.Reflection;

namespace SachkovTech.Accounts.Application;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}