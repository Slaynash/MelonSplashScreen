using System.Runtime.InteropServices;

namespace MelonSplashScreen.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageDataDirectory
    {
        [FieldOffset(0)]
        public uint virtualAddress;
        [FieldOffset(4)]
        public uint size;
    }
}
