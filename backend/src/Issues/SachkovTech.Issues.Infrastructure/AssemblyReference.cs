using System.Reflection;

namespace SachkovTech.Issues.Infrastructure;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}