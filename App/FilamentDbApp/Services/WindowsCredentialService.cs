using System.Runtime.InteropServices;
using System.Text;

namespace FilamentDbApp.Services;

public static class WindowsCredentialService
{
    private const int GenericCredential = 1;
    private const int LocalMachinePersistence = 2;

    public static void SavePassword(string host, string userName, string password)
    {
        var bytes = Encoding.Unicode.GetBytes(password);
        var blob = Marshal.AllocCoTaskMem(bytes.Length);
        try
        {
            Marshal.Copy(bytes, 0, blob, bytes.Length);
            var credential = new NativeCredential
            {
                Type = GenericCredential,
                TargetName = BuildTarget(host, userName),
                CredentialBlobSize = bytes.Length,
                CredentialBlob = blob,
                Persist = LocalMachinePersistence,
                UserName = userName
            };
            if (!CredWrite(ref credential, 0)) throw new InvalidOperationException("Windows could not save the FTPS credential.", new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
        }
        finally
        {
            Marshal.Copy(new byte[bytes.Length], 0, blob, bytes.Length);
            Marshal.FreeCoTaskMem(blob);
            Array.Clear(bytes);
        }
    }

    public static string ReadPassword(string host, string userName)
    {
        return ReadPasswordForTarget(BuildTarget(host, userName));
    }

    private static string ReadPasswordForTarget(string target)
    {
        if (!CredRead(target, GenericCredential, 0, out var pointer)) return string.Empty;
        try
        {
            var credential = Marshal.PtrToStructure<NativeCredential>(pointer);
            return credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize == 0
                ? string.Empty
                : Marshal.PtrToStringUni(credential.CredentialBlob, credential.CredentialBlobSize / 2) ?? string.Empty;
        }
        finally
        {
            CredFree(pointer);
        }
    }

    private static string BuildTarget(string host, string userName) =>
        $"3DPIceland.FilamentDbApp.FtpsPublish:{host.Trim().ToLowerInvariant()}:{userName.Trim().ToLowerInvariant()}";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeCredential
    {
        public int Flags;
        public int Type;
        public string TargetName;
        public string? Comment;
        public long LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public string? TargetAlias;
        public string UserName;
    }

    [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredWrite(ref NativeCredential credential, int flags);

    [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredRead(string target, int type, int flags, out IntPtr credential);

    [DllImport("advapi32.dll")]
    private static extern void CredFree(IntPtr credential);
}
