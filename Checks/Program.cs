using MapsetVerifierFramework;
using System.Globalization;

namespace ManiaChecks
{
    public class Main
    {
        public static void Run() {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CheckerRegistry.RegisterCheck(new CheckOdHp());
            CheckerRegistry.RegisterCheck(new CheckLNL());
        }
    }
}
