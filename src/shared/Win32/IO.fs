module Shared.Internal.Win32

open System
open System.Runtime.InteropServices

[<DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)>]
extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes)

let createHardLink (newLinkPath: string, existingFilePath: string) =
    let success = CreateHardLink(newLinkPath, existingFilePath, IntPtr.Zero)

    if not success then
        let errorCode = Marshal.GetLastWin32Error()
        failwithf "Failed to create hard link. Win32 Error Code: %d" errorCode
    else
        printfn "Hard link created successfully: %s -> %s" newLinkPath existingFilePath
