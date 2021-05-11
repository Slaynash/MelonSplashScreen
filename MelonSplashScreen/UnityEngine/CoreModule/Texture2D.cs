using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Texture2D : Il2CppObjectBase
    {
        private static readonly IntPtr m_get_whiteTexture;
        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_SetPixels;
        private static readonly IntPtr m_Apply;

        static Texture2D()
        {
            Il2CppClassPointerStore<Texture2D>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Texture2D");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Texture2D>.NativeClassPtr);

            m_ctor = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, ".ctor", "System.Void", "System.Int32", "System.Int32");

            m_get_whiteTexture = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, "get_whiteTexture", "UnityEngine.Texture2D");

            m_SetPixels = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, "SetPixels", "System.Void", "UnityEngine.Color[]", "System.Int32");
            m_Apply = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, "Apply", "System.Void");
        }

        public Texture2D(IntPtr ptr) : base(ptr) { }

        public unsafe static Texture2D whiteTexture
        {
            get
            {
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_whiteTexture, IntPtr.Zero, (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return intPtr == IntPtr.Zero ? null : new Texture2D(intPtr);
            }
        }

        public unsafe Texture2D(int width, int height) : base(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<Texture2D>.NativeClassPtr))
        {
            void** args = stackalloc void*[2];
            args[0] = &width;
            args[1] = &height;
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_ctor, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe void SetPixels(Color[] colors, int miplevel = 0)
        {
            IntPtr colorsArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Color>.NativeClassPtr, (uint)colors.Length);
            for (var i = 0; i < colors.Length; i++)
                ((Color*)(colorsArrayPtr + 4 * IntPtr.Size))[i] = colors[i];
            void** args = stackalloc void*[2];
            args[0] = (void*)colorsArrayPtr;
            args[1] = &miplevel;
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_SetPixels, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe void Apply()
        {
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_Apply, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }
    }
}
