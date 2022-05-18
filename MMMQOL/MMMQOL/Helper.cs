using Harmony;
using System.Collections.Generic;
using System.Text;
using static GameUtility;

namespace MMMQOL {
    class Helper {

        [HarmonyPatch(typeof(StringBuilderPool), "GetBuilder")]
        public static class StringBuilderPool_GetBuilder_Patch {
            public static bool Prefix(StringBuilderPool __instance, ref StringBuilder __result, ref Stack<StringBuilder> ___builders) {
                if(___builders.Count == 0) {
                    __result = new StringBuilder(640);
                    return false;
                }
                __result = ___builders.Pop();
                return false;
            }
        }

        [HarmonyPatch(typeof(StringBuilderPool), "ReturnBuilder")]
        public static class StringBuilderPool_ReturnBuilder_Patch {
            public static bool Prefix(StringBuilderPool __instance, StringBuilder builder, ref Stack<StringBuilder> ___builders) {
                builder.Length = 0;
                ___builders.Push(builder);
                return false;
            }
        }

        

    }
}
