using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Graphics : Il2CppObjectBase
    {
        private static IntPtr m_DrawTexture;

        static Graphics()
        {
            Il2CppClassPointerStore<Graphics>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Graphics");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Graphics>.NativeClassPtr);
            m_DrawTexture = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Graphics>.NativeClassPtr, "DrawTexture", "System.Void", new string[] { "UnityEngine.Rect", "UnityEngine.Texture", "UnityEngine.Material", "System.Int32" });
        }

        public Graphics(IntPtr ptr) : base(ptr) { }

        public unsafe static void DrawTexture(Rect screenRect, Texture2D texture, IntPtr material = default, int pass = -1)
        {
            void** args = stackalloc void*[4];
            args[0] = &screenRect;
            args[1] = (void*)IL2CPP.Il2CppObjectBaseToPtrNotNull(texture);
            args[2] = (void*)material;
            args[3] = &pass;
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_DrawTexture, IntPtr.Zero, args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }
    }
}
