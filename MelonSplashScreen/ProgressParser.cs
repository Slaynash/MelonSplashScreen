using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MelonSplashScreen
{
    internal static class ProgressParser
    {
        public static float GetProgress(string newline, float default_) // TODO flags (il2cpp/mono, cpp2il/il2cppdumper)
        {
            float totalTime = 0;
            float progressTime = 0;
            foreach (var entry in averageStepDurations)
            {
                if (progressTime <= 0f && Regex.IsMatch(newline, entry.Key))
                    progressTime = totalTime;

                totalTime += entry.Value;
            }

            return progressTime > 0 ? progressTime / totalTime : default_;
        }

        internal static readonly Dictionary<string, float> averageStepDurations = new Dictionary<string, float>()
        {
            {
                @"Contacting RemoteAPI\.\.\.",
                100f
            },
            {
                @"Downloading Unity \S+ Dependencies\.\.\.",
                1000f
            },
            {
                @"Extracting .* to .*UnityDpendencies",
                500f
            },
            {
                @"Downloading Il2CppDumper\.\.\.",
                500f
            },
            {
                @"Extracting .* to .*Il2CppDumper",
                500f
            },
            {
                @"Downloading Il2CppAssemblyUnhollower\.\.\.",
                500f
            },
            {
                @"Extracting .* to .*Il2CppAssemblyUnhollower",
                500f
            },
            {
                @"Downloading Deobfuscation Map\.\.\.",
                500f
            },
            {
                @"Checking GameAssembly\.\.\.",
                1000f
            },
            // Il2CppDumper
            {
                @"Initializing metadata\.\.\.",
                3500f
            },
            {
                @"Initializing il2cpp file\.\.\.",
                1800f
            },
            {
                @"Dumping\.\.\.",
                1400f
            },
            {
                @"Generate struct\.\.\.",
                26000f
            },
            {
                @"Generate dummy dll\.\.\.",
                13000f
            },
            // Il2CppAssemblyUnhollower
            {
                @"Reading assemblies\.\.\.",
                170f
            },
            {
                @"Reading system assemblies\.\.\.",
                14f
            },
            {
                @"Reading unity assemblies\.\.\.",
                29f
            },
            {
                @"Creating rewrite assemblies\.\.\.",
                20f
            },
            {
                @"Computing renames\.\.\.",
                281f
            },
            {
                @"Creating typedefs\.\.\.",
                109f
            },
            {
                @"Computing struct blittability\.\.\.",
                10f
            },
            {
                @"Filling typedefs\.\.\.",
                27f
            },
            {
                @"Filling generic constraints\.\.\.",
                6f
            },
            {
                @"Creating members\.\.\.",
                2256f
            },
            {
                @"Scanning method cross-references\.\.\.",
                1919f
            },
            {
                @"Finalizing method declarations\.\.\.",
                2867f
            },
            {
                @"Filling method parameters\.\.\.",
                510f
            },
            {
                @"Creating static constructors\.\.\.",
                1237f
            },
            {
                @"Creating value type fields\.\.\.",
                186f
            },
            {
                @"Creating enums\.\.\.",
                69f
            },
            {
                @"Creating IntPtr constructors\.\.\.",
                63f
            },
            {
                @"Creating type getters\.\.\.",
                132f
            },
            {
                @"Creating non-blittable struct constructors\.\.\.",
                38f
            },
            {
                @"Creating generic method static constructors\.\.\.",
                42f
            },
            {
                @"Creating field accessors\.\.\.",
                1642f
            },
            {
                @"Filling methods\.\.\.",
                2385f
            },
            {
                @"Generating implicit conversions\.\.\.",
                121f
            },
            {
                @"Creating properties\.\.\.",
                102f
            },
            {
                @"Unstripping types\.\.\.",
                44f
            },
            {
                @"Unstripping fields\.\.\.",
                5f
            },
            {
                @"Unstripping methods\.\.\.",
                241f
            },
            {
                @"Unstripping method bodies\.\.\.",
                266f
            },
            {
                @"Generating forwarded types\.\.\.",
                4f
            },
            {
                @"Writing xref cache\.\.\.",
                1179f
            },
            {
                @"Writing assemblies\.\.\.",
                2586f
            },
            {
                @"Writing method pointer map\.\.\.",
                89f
            },
            // Move files
            {
                @"Moving Il2Cppmscorlib.dll\.\.\.",
                500f
            },
        };
    }
}
