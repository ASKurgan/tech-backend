using System.Reflection;

namespace SachkovTech.Issues.Application;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}