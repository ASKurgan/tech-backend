using System.Reflection;

namespace SachkovTech.Issues.Contracts;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}