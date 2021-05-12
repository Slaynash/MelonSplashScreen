using MelonLoader;
using MelonSplashScreen.NativeUtils;
using MelonSplashScreen.NativeUtils.PEParser;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UnhollowerMini;
using UnityEngine;
using Windows;

[assembly: MelonInfo(typeof(MelonSplashScreen.MelonSplashScreenPlugin), "MelonSplashScreen", "0.1.0", "Slaynash")]
[assembly: MelonGame]
[assembly: MelonColor(ConsoleColor.Green)]
[assembly: MelonPriority(-1)]

namespace MelonSplashScreen
{
    public class MelonSplashScreenPlugin : MelonPlugin
    {
        private delegate void SetupPixelCorrectCoordinates(bool _1);
        private delegate void PresentFrame();
        private delegate IntPtr User32SetTimerDelegate(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        private const float logoRatio = 1.2353f;

#pragma warning disable 0649
        #region m_SetupPixelCorrectCoordinates Signatures
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 83 ec 60 53 56 57 e8 ?? ?? ?? ?? 8b d8 e8 ?? ?? ?? ?? ff 75 08 8d 4d f0", "2019.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 83 ec 60 53 56 57 e8 ?? ?? ?? ?? ff 75 08 8b d8 8d 45 f0 50 e8", "2017.3.0", "2018.1.0")]
        [NativeSignature(NativeSignatureFlags.X86, "55 8b ec 83 ec 60 56 e8 ?? ?? ?? ?? 8b f0 8b 45 08 50 8d 4d f0 51 e8", "2017.1.0")]
        [NativeSignature(NativeSignatureFlags.X64, "48 89 5c 24 08 57 48 81 ec a0 00 00 00 8b d9 e8 ?? ?? ?? ?? 48 8b f8 e8", "2017.1.0")]
        // 2019.4.18f1 armeabi-v7a nondev mono : "70 b9 a7 00 30 48 2d e9 70 d0 4d e2 00 50 a0 e1 19 26 04 eb 00 40 a0 e1 11 a9 f6 fa 00 10 a0 e1 20 00 8d e2"
        #endregion
        private static SetupPixelCorrectCoordinates m_SetupPixelCorrectCoordinates;

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
        private static PresentFrame m_PresentFrame;
#pragma warning restore 0649

        private Texture2D backgroundTexture;
        private Texture2D melonloaderLogoTexture;
        private Texture2D loadingbarOuterTexture;
        private Texture2D loadingbarInnerTexture;
        private Font font;
        private Mesh melonloaderversionTextmesh;
        private Mesh progressTextmesh;

        private IntPtr titleBarTimer;

        private bool generationDone = false;
        private float progress = 0f;
        private string progressText = "", progressTextCached;

        private static User32SetTimerDelegate user32SetTimerOriginal;
        private static bool nextSetTimerIsUnity = false;

        public override void OnApplicationEarlyStart()
        {
            if (!NativeSignatureResolver.Apply())
                return;

            ApplyUser32SetTimerPatch();

            InitTextures();
            // Load default font
            IntPtr fontPtr = Resources.GetBuiltinResource(Il2CppType.Of<Font>(), "Arial.ttf");
            font = fontPtr != IntPtr.Zero ? new Font(fontPtr) : null;

            TextGenerationSettings settings = new TextGenerationSettings();
            settings.textAnchor = TextAnchor.MiddleCenter;
            settings.color = new Color(1, 1, 1);
            settings.generationExtents = new Vector2(540, 47.5f);
            settings.richText = true;
            settings.font = font;
            settings.pivot = new Vector2(0.5f, 0.5f);
            settings.fontSize = 24;
            settings.fontStyle = FontStyle.Bold;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.scaleFactor = 1f;
            settings.lineSpacing = 1f;
            melonloaderversionTextmesh = TextMeshGenerator.Generate("<color=#78f764>Melon</color><color=#ff3c6a>Loader</color> " + BuildInfo.Version + " Alpha Pre-Release", settings);

            RefreshProgressTextmesh();

            RegisterMessageCallbacks();


            Render();

            StartUnhollowerThread();
            MainLoop();

            progress = 100f;
            progressText = "Starting game...";

            Render(); // Final render, to set the progress bar to 100%
            Render(); // Double buffering

            // TODO Patch ML Unhollower part
        }

        private unsafe void ApplyUser32SetTimerPatch()
        {
            IntPtr moduleAddress = IntPtr.Zero;

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName == "USER32.dll")
                {
                    moduleAddress = module.BaseAddress;
                    break;
                }
            }

            if (moduleAddress == IntPtr.Zero)
            {
                MelonLogger.Error($"Failed to find module \"USER32.dll\"");
                return;
            }

            IntPtr original = PEUtils.GetExportedFunctionPointerForModule(moduleAddress, "SetTimer");


            IntPtr detourPtr = typeof(MelonSplashScreenPlugin).GetMethod("User32SetTimerDetour", BindingFlags.NonPublic | BindingFlags.Static).MethodHandle.GetFunctionPointer();

            if (detourPtr == IntPtr.Zero)
            {
                MelonLogger.Error("Failed to find USER32.dll::SetTimer");
                return;
            }

            MelonLogger.Msg("SetTimer: 0x" + string.Format("{0:X}", (ulong)original));

            MelonUtils.NativeHookAttach((IntPtr)(&original), detourPtr);
            user32SetTimerOriginal = Marshal.GetDelegateForFunctionPointer<User32SetTimerDelegate>(original);
            MelonLogger.Msg("Applied USER32.dll::SetTimer patch");
        }

        private static IntPtr User32SetTimerDetour(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr timerProc)
        {
            if (nextSetTimerIsUnity)
            {
                nextSetTimerIsUnity = false;
                return IntPtr.Zero;
            }

            return user32SetTimerOriginal(hWnd, nIDEvent, uElapse, timerProc);
        }

        private void InitTextures()
        {
            backgroundTexture = CreateColorTexture(new Color(0.08f, 0.09f, 0.10f));
            loadingbarOuterTexture = CreateColorTexture(new Color(0.47f, 0.97f, 0.39f));
            loadingbarInnerTexture = CreateColorTexture(new Color(1.00f, 0.23f, 0.42f));

            melonloaderLogoTexture = new Texture2D(2, 2);
            ImageConversion.LoadImage(melonloaderLogoTexture, Convert.FromBase64String(ImageDatas.MelonLogo), false);
        }

        private Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.SetPixels(new Color[] { color, color, color, color });
            texture.Apply();
            return texture;
        }


        private void RegisterMessageCallbacks()
        {
            Action<ConsoleColor, ConsoleColor, string, string> loghander = (meloncolor, txtcolor, namesection, msg_) =>
            {
                if (namesection == "EarlyGraphicsInit")
                    return;

                progress = ProgressParser.GetProgress(msg_, ref progressText, progress);
            };

            MelonLogger.MsgCallbackHandler += loghander;
            MelonDebug.MsgCallbackHandler += loghander;
        }


        private void StartUnhollowerThread()
        {
            new Thread(() =>
            {
                MelonLogger.Msg("Starting Assembly Generator");
                typeof(MelonHandler).Assembly.GetType("MelonLoader.Il2CppAssemblyGenerator").GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
                MelonLogger.Msg("Done running Assembly Generator");
                generationDone = true;
            })
            {
                IsBackground = true,
                Name = "UnhollowerThread"
            }.Start();
        }

        private void MainLoop()
        {
            User32.PeekMessage(out Msg msg, IntPtr.Zero, 0, 0, 0);
            while (!generationDone) // WM_QUIT
            {
                if (msg.message == WindowMessage.QUIT)
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                else if (!User32.PeekMessage(out msg, IntPtr.Zero, 0, 0, 1)) // If there is no pending message
                {
                    if (titleBarTimer != IntPtr.Zero)
                    {
                        User32.KillTimer(IntPtr.Zero, titleBarTimer);
                        titleBarTimer = IntPtr.Zero;
                    }
                    Render();

                    Thread.Sleep(16); // ~60fps
                }
                else
                {
                    if (msg.message == WindowMessage.NCLBUTTONDOWN || msg.message == (WindowMessage)0x242 /* NCPOINTERDOWN */)
                    {
                        if (titleBarTimer == IntPtr.Zero)
                            titleBarTimer = User32.SetTimer(IntPtr.Zero, IntPtr.Zero, 10, TitleBarTimerUpdateCallback);
                        nextSetTimerIsUnity = true;
                    }
                    else if (msg.message == WindowMessage.PAINT)
                        Render();

                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }
            }

            if (titleBarTimer != IntPtr.Zero)
            {
                User32.KillTimer(IntPtr.Zero, titleBarTimer);
                titleBarTimer = IntPtr.Zero;
            }
        }

        private void TitleBarTimerUpdateCallback(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime)
        {
            //Render(); // XXX Somehow this throws NPE, so we can't use it :///
        }


        private void RefreshProgressTextmesh()
        {
            if (progressTextCached == progressText)
                return;

            progressTextCached = progressText;

            TextGenerationSettings settings2 = new TextGenerationSettings();
            settings2.textAnchor = TextAnchor.MiddleCenter;
            settings2.color = new Color(1, 1, 1);
            settings2.generationExtents = new Vector2(540, 47.5f);
            settings2.richText = true;
            settings2.font = font;
            settings2.pivot = new Vector2(0.5f, 0.5f);
            settings2.fontSize = 16;
            settings2.fontStyle = FontStyle.Bold;
            settings2.verticalOverflow = VerticalWrapMode.Overflow;
            settings2.scaleFactor = 1f;
            settings2.lineSpacing = 1f;
            progressTextmesh = TextMeshGenerator.Generate(progressText, settings2);
        }

        private unsafe void Render()
        {
            RefreshProgressTextmesh();

            m_SetupPixelCorrectCoordinates(false);
            

            int sw = Screen.width;
            int sh = Screen.height;

            int logoHeight = (int)(sh * 0.4f);
            int logoWidth = (int)(logoHeight * logoRatio);

            Graphics.DrawTexture(new Rect(0, 0, sw, sh), backgroundTexture);
            Graphics.DrawTexture(new Rect((sw - logoWidth) / 2, sh - ((sh - logoHeight) / 2 - 46), logoWidth, -logoHeight), melonloaderLogoTexture);

            font.material.SetPass(0);
            Graphics.DrawMeshNow(melonloaderversionTextmesh, new Vector3(sw / 2, sh - (sh / 2 + (logoHeight / 2) - 35), 0), Quaternion.identity);

            RenderProgressBar((sw - 540) / 2, sh - ((sh - 36) / 2 + (logoHeight / 2) + 50), 540, 36, progress);

            m_PresentFrame();
        }

        private void RenderProgressBar(int x, int y, int width, int height, float progress)
        {
            Graphics.DrawTexture(new Rect(x, y, width, height), loadingbarOuterTexture);
            Graphics.DrawTexture(new Rect(x + 6, y + 6, width - 12, height - 12), backgroundTexture);
            Graphics.DrawTexture(new Rect(x + 9, y + 9, (int)((width - 18) * Math.Min(1.0f, progress)), height - 18), loadingbarInnerTexture);

            font.material.SetPass(0);
            Graphics.DrawMeshNow(progressTextmesh, new Vector3(x + width / 2, y + height / 2 + 2, 0), Quaternion.identity);
        }
    }
}
