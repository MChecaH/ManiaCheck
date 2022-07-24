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
            CheckerRegistry.RegisterCheck(new CheckSeven());
            CheckerRegistry.RegisterCheck(new checkHN());
            CheckerRegistry.RegisterCheck(new CheckHSDiff());
            CheckerRegistry.RegisterCheck(new CheckEzSv());
        }
    }
}
