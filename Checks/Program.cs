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
            CheckerRegistry.RegisterCheck(new CheckVarBPM());
            CheckerRegistry.RegisterCheck(new CheckColumnDistribution());
            CheckerRegistry.RegisterCheck(new CheckConcurrent());
            CheckerRegistry.RegisterCheck(new CheckDrainTime());
            CheckerRegistry.RegisterCheck(new CheckSBHS());
            CheckerRegistry.RegisterCheck(new CheckHSCons());
        }
    }
}
