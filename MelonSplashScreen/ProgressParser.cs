using System.Text.RegularExpressions;

namespace MelonSplashScreen
{
    internal static class ProgressParser
    {
        public static float GetProgress(string newline, ref string progressText, float default_) // TODO flags (il2cpp/mono, cpp2il/il2cppdumper)
        {
            float totalTime = 0;
            float progressTime = 0;
            foreach (var entry in averageStepDurations)
            {
                if (progressTime <= 0f && Regex.IsMatch(newline, entry.message))
                {
                    progressTime = totalTime;
                    progressText = entry.progresstext ?? newline;
                }

                totalTime += entry.weight;
            }

            return progressTime > 0 ? progressTime / totalTime : default_;
        }

        internal static readonly (string message, float weight, string progresstext)[] averageStepDurations = new (string, float, string)[]
        {
            (
                @"Contacting RemoteAPI\.\.\.",
                100f,
                null
            ),
            (
                @"Downloading Unity \S+ Dependencies\.\.\.",
                1000f,
                null
            ),
            (
                @"Extracting .* to .*UnityDpendencies",
                500f,
                null
            ),
            (
                @"Downloading Il2CppDumper\.\.\.",
                500f,
                null
            ),
            (
                @"Extracting .* to .*Il2CppDumper",
                500f,
                null
            ),
            (
                @"Downloading Il2CppAssemblyUnhollower\.\.\.",
                500f,
                null
            ),
            (
                @"Extracting .* to .*Il2CppAssemblyUnhollower",
                500f,
                null
            ),
            (
                @"Downloading Deobfuscation Map\.\.\.",
                500f,
                null
            ),
            (
                @"Checking GameAssembly\.\.\.",
                1000f,
                null
            ),
            // Il2CppDumper
            (
                @"Initializing metadata\.\.\.",
                3500f,
                null
            ),
            (
                @"Initializing il2cpp file\.\.\.",
                1800f,
                null
            ),
            (
                @"Dumping\.\.\.",
                1400f,
                null
            ),
            (
                @"Generate struct\.\.\.",
                26000f,
                null
            ),
            (
                @"Generate dummy dll\.\.\.",
                13000f,
                null
            ),
            // Il2CppAssemblyUnhollower
            (
                @"Reading assemblies\.\.\.",
                170f,
                null
            ),
            (
                @"Reading system assemblies\.\.\.",
                14f,
                null
            ),
            (
                @"Reading unity assemblies\.\.\.",
                29f,
                null
            ),
            (
                @"Creating rewrite assemblies\.\.\.",
                20f,
                null
            ),
            (
                @"Computing renames\.\.\.",
                281f,
                null
            ),
            (
                @"Creating typedefs\.\.\.",
                109f,
                null
            ),
            (
                @"Computing struct blittability\.\.\.",
                10f,
                null
            ),
            (
                @"Filling typedefs\.\.\.",
                27f,
                null
            ),
            (
                @"Filling generic constraints\.\.\.",
                6f,
                null
            ),
            (
                @"Creating members\.\.\.",
                2256f,
                null
            ),
            (
                @"Scanning method cross-references\.\.\.",
                1919f,
                null
            ),
            (
                @"Finalizing method declarations\.\.\.",
                2867f,
                null
            ),
            (
                @"Filling method parameters\.\.\.",
                510f,
                null
            ),
            (
                @"Creating static constructors\.\.\.",
                1237f,
                null
            ),
            (
                @"Creating value type fields\.\.\.",
                186f,
                null
            ),
            (
                @"Creating enums\.\.\.",
                69f,
                null
            ),
            (
                @"Creating IntPtr constructors\.\.\.",
                63f,
                null
            ),
            (
                @"Creating type getters\.\.\.",
                132f,
                null
            ),
            (
                @"Creating non-blittable struct constructors\.\.\.",
                38f,
                null
            ),
            (
                @"Creating generic method static constructors\.\.\.",
                42f,
                null
            ),
            (
                @"Creating field accessors\.\.\.",
                1642f,
                null
            ),
            (
                @"Filling methods\.\.\.",
                2385f,
                null
            ),
            (
                @"Generating implicit conversions\.\.\.",
                121f,
                null
            ),
            (
                @"Creating properties\.\.\.",
                102f,
                null
            ),
            (
                @"Unstripping types\.\.\.",
                44f,
                null
            ),
            (
                @"Unstripping fields\.\.\.",
                5f,
                null
            ),
            (
                @"Unstripping methods\.\.\.",
                241f,
                null
            ),
            (
                @"Unstripping method bodies\.\.\.",
                266f,
                null
            ),
            (
                @"Generating forwarded types\.\.\.",
                4f,
                null
            ),
            (
                @"Writing xref cache\.\.\.",
                1179f,
                null
            ),
            (
                @"Writing assemblies\.\.\.",
                2586f,
                null
            ),
            (
                @"Writing method pointer map\.\.\.",
                89f,
                null
            ),
            // Move files
            (
                @"Moving Il2Cppmscorlib.dll\.\.\.",
                500f,
                null
            ),
        };
    }
}
