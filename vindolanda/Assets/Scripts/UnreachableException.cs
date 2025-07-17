using System;
using System.Diagnostics;

/// <summary>
/// An exception for code paths that should never be reached, I miss Rust's match syntax
/// </summary>
public class UnreachableException : Exception
{
    public UnreachableException() : base("This should be unreachable") {
        Debug.Assert(false);
    }
}