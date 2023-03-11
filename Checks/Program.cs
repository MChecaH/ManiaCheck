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
            //CheckerRegistry.RegisterCheck(new CheckEzSv()); Broken Base BPM calc
            //CheckerRegistry.RegisterCheck(new CheckVarBPM()); Broken Base BPM calc
            CheckerRegistry.RegisterCheck(new CheckConcurrent());
            CheckerRegistry.RegisterCheck(new CheckDrainTime());
            CheckerRegistry.RegisterCheck(new CheckSBHS());
            CheckerRegistry.RegisterCheck(new CheckHSCons());
        }
    }
}
