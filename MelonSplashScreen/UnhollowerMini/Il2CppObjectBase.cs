﻿using System;

namespace UnhollowerMini
{
    internal class Il2CppObjectBase
    {
        public IntPtr Pointer
        {
            get
            {
                var handleTarget = IL2CPP.il2cpp_gchandle_get_target(myGcHandle);
                if (handleTarget == IntPtr.Zero) throw new ObjectCollectedException("Object was garbage collected in IL2CPP domain");
                return handleTarget;
            }
        }

        private readonly uint myGcHandle;

        public Il2CppObjectBase(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new NullReferenceException();

            myGcHandle = IL2CPP.il2cpp_gchandle_new(pointer, false);
        }

        ~Il2CppObjectBase()
        {
            IL2CPP.il2cpp_gchandle_free(myGcHandle);
        }
    }
}
