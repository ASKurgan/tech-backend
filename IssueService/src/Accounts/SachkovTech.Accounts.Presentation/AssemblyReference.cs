using System.Reflection;

namespace SachkovTech.Accounts.Presentation;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}