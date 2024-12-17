using System.Reflection;

namespace SachkovTech.Issues.Presentation;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}