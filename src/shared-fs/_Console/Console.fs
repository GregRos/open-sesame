[<AutoOpen>]
module Shared.Console

open System
open System.Runtime.InteropServices

module ConsoleHelper =
    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern bool AllocConsole()

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern bool FreeConsole()

// Allocate a console window
let initializeConsole () =
    if ConsoleHelper.AllocConsole() then
        Console.WriteLine("Console window allocated.")
    else
        let errorCode = Marshal.GetLastWin32Error()
        Console.WriteLine($"Failed to allocate console. Error code: {errorCode}")
