using MelonLoader;
using MelonSplashScreen.NativeUtils;
using MelonSplashScreen.NativeUtils.PEParser;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Windows;

[assembly: MelonInfo(typeof(MelonSplashScreen.MelonSplashScreenPlugin), "MelonSplashScreen", "0.1.0", "Slaynash")]
[assembly: MelonGame]
[assembly: MelonColor(ConsoleColor.Green)]
[assembly: MelonPriorityAttribute(1000)]

namespace MelonSplashScreen
{
    public class MelonSplashScreenPlugin : MelonPlugin
    {
        private delegate IntPtr User32SetTimerDelegate(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);



        private IntPtr titleBarTimer;

        private bool generationDone = false;

        private static User32SetTimerDelegate user32SetTimerOriginal;
        private static bool nextSetTimerIsUnity = false;

        private MethodInfo originalAGRun;
        private static int agRunResult;
        private static bool agRan;

        public override void OnApplicationEarlyStart()
        {
            if (!NativeSignatureResolver.Apply())
                return;

            ApplyUser32SetTimerPatch();

            SplashRenderer.Init();

            RegisterMessageCallbacks();

            SplashRenderer.Render(); // Initial render

            StartUnhollowerThread();
            MainLoop();

            SplashRenderer.UpdateMainProgress("Starting game...", 1f);
            SplashRenderer.Render(); // Final render, to set the progress bar to 100%
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


        private void RegisterMessageCallbacks()
        {
            Action<ConsoleColor, ConsoleColor, string, string> loghander = (meloncolor, txtcolor, namesection, msg_) =>
            {
                if (namesection == "EarlyGraphicsInit")
                    return;

                SplashRenderer.UpdateProgressFromLog(msg_);
            };

            MelonLogger.MsgCallbackHandler += loghander;
            MelonDebug.MsgCallbackHandler += loghander;
        }


        private void StartUnhollowerThread()
        {
            string agPath = Path.Combine(MelonUtils.GameDirectory, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator", "Il2CppAssemblyGenerator.dll");
            if (!File.Exists(agPath))
            {
                MelonLogger.Error("Failed to Find Il2CppAssemblyGenerator.dll!");
                return;
            }

            Assembly agAssembly = Assembly.LoadFile(agPath);

            originalAGRun = HarmonyInstance.Patch(agAssembly.GetType("MelonLoader.Il2CppAssemblyGenerator.Core").GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static),
                prefix: new HarmonyLib.HarmonyMethod(typeof(MelonSplashScreenPlugin).GetMethod("AssemblyGeneratorRunPrefix", BindingFlags.NonPublic | BindingFlags.Static)));

            new Thread(() =>
            {
                MelonLogger.Msg("Starting Assembly Generator");
                agRunResult = (int)originalAGRun.Invoke(null, null);
                MelonLogger.Msg("Done running Assembly Generator. Returned code: " + agRunResult);
                generationDone = true;
            })
            {
                IsBackground = true,
                Name = "UnhollowerThread"
            }.Start();
        }

        private static bool AssemblyGeneratorRunPrefix(ref int __result)
        {
            __result = 1;
            return false;

            if (!agRan)
            {
                agRan = true;
                return true;
            }

            __result = agRunResult;
            return false;
        }

        private void MainLoop()
        {
            User32.PeekMessage(out Msg msg, IntPtr.Zero, 0, 0, 0);
            while (!generationDone || SplashRenderer.IsRunning) // WM_QUIT
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
                    SplashRenderer.Render();

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
                        SplashRenderer.Render();

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
            SplashRenderer.Render();
        }




        /*
        private static unsafe void CopyLogToClipboard()
        {
            DropFile dropFile = new DropFile();
            dropFile.file = Path.Combine(MelonUtils.GameDirectory, "MelonLoader", "Dependencies", "Il2CppAssemblyGenerator") + "\0";
            User32.SetClipboardData(0x15 /* CF_HDROP * /, ref dropFile);
        }
        */
    }
}
