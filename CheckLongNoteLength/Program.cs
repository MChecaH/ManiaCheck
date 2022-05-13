using MapsetVerifierFramework;
using System.Globalization;

namespace CheckLongNoteLength
{
    public class Main
    {
        public static void Run() {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CheckerRegistry.RegisterCheck(new CheckLNL());
        }
    }
}
