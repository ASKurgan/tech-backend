using System.Reflection;

namespace SachkovTech.Accounts.Communication;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}