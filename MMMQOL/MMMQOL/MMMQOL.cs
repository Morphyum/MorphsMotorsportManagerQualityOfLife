using Harmony;
using System.Reflection;

namespace MMMQOL
{
    public class MMMQOL
    {
        public static void Init() {
            var harmony = HarmonyInstance.Create("de.morphyum.MMMQOL");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
