module Shared.Internal.Win32

open System
open System.Runtime.InteropServices

[<DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)>]
extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes)

open System
open System.IO
open System.Runtime.InteropServices

[<Struct; StructLayout(LayoutKind.Sequential)>]
type BY_HANDLE_FILE_INFORMATION =
    val mutable FileAttributes: uint32
    val mutable CreationTime: int64
    val mutable LastAccessTime: int64
    val mutable LastWriteTime: int64
    val mutable VolumeSerialNumber: uint32
    val mutable FileSizeHigh: uint32
    val mutable FileSizeLow: uint32
    val mutable NumberOfLinks: uint32
    val mutable FileIndexHigh: uint32
    val mutable FileIndexLow: uint32

[<DllImport("kernel32.dll", SetLastError = true)>]
extern bool GetFileInformationByHandle(IntPtr hFile, BY_HANDLE_FILE_INFORMATION& lpFileInformation)



let createHardLink (existingFilePath: string, newLinkPath: string) =
    let success = CreateHardLink(newLinkPath, existingFilePath, IntPtr.Zero)

    if not success then
        let errorCode = Marshal.GetLastWin32Error()
        failwithf "Failed to create hard link. Win32 Error Code: %d" errorCode
    else
        printfn "Hard link created successfully: %s -> %s" newLinkPath existingFilePath

let getFileInformation (filePath: string) =
    use fs =
        new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

    let handle = fs.SafeFileHandle.DangerousGetHandle()
    let mutable fileInfo = Unchecked.defaultof<BY_HANDLE_FILE_INFORMATION>

    if GetFileInformationByHandle(handle, &fileInfo) then
        Some fileInfo
    else
        None


let areHardLinks (path1: string) (path2: string) =
    match getFileInformation path1, getFileInformation path2 with
    | Some info1, Some info2 ->
        info1.VolumeSerialNumber = info2.VolumeSerialNumber
        && info1.FileIndexHigh = info2.FileIndexHigh
        && info1.FileIndexLow = info2.FileIndexLow
    | _ -> false
