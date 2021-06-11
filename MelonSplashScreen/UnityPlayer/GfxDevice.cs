using MelonSplashScreen.NativeUtils;
using System;

namespace UnityPlayer
{
    internal class GfxDevice
    {
        private delegate void PresentFrameDelegate();

        #region m_PresentFrame Signatures
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 51 56 e8 ?? ?? ?? ?? 8b f0 8b ce e8 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? e8", "2018.4.18", "2019.3.0", "2020.1.0", "2021.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 51 e8 ?? ?? ?? ?? 85 c0 74 15 e8 ?? ?? ?? ?? 8b c8 8b 10 8b 82 ?? 00 00 00 ff d0", "2018.4.9", "2019.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 51 e8 ?? ?? ?? ?? 85 c0 74 12 e8 ?? ?? ?? ?? 8b c8 8b 10 8b 42 ?? ff d0 84 c0 75", "2018.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "e8 ?? ?? ?? ?? 85 c0 74 12 e8 ?? ?? ?? ?? 8b ?? 8b ?? 8b 42 70 ff d0 84 c0 75", "2017.1.0", "5.6.0", "2017.1.0")]
        [NativeSignature(NativeSignatureFlags.X64, "40 53 48 83 ec 20 e8 ?? ?? ?? ?? 48 8b c8 48 8b d8 e8 ?? ?? ?? ?? e8 ?? ?? ?? ?? 48 85 c0 74", "2018.4.18", "2019.3.0", "2020.1.0")]
        [NativeSignature(NativeSignatureFlags.X64, "48 83 ec 28 e8 ?? ?? ?? ?? 48 85 c0 74 15 e8 ?? ?? ?? ?? 48 8b c8 48 8b 10 ff 92 ?? ?? 00 00 84 c0", "2018.3.0", "2019.1.0")] // We can't use this one too early, else we match multiple functions
        [NativeSignature(NativeSignatureFlags.X64, "48 83 ec 28 e8 ?? ?? ?? ?? 48 85 c0 74 15 e8 ?? ?? ?? ?? 48 8b c8 48 8b 10 ff 92 e0 00 00 00 84 c0", "5.6.0", "2017.1.0")]
        // 2019.4.18f1 armeabi-v7a nondev mono : "10 b5 f5 f2 ae eb 04 46 f6 f2 ce ee d9 f7 20 fd 38 b1 d9 f7 ld fd 01 68 d1 f8 9c 10"
        #endregion
        private static PresentFrameDelegate m_PresentFrame;

        public static void PresentFrame() =>
            m_PresentFrame();
    }
}
