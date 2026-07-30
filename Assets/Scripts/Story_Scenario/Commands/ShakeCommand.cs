using CSV4Unity;
using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class ShakeCommand : CommandBase
    {
        VFXController vfxController;

        public ShakeCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            int duration = lineData[ScenarioFields.Arg1].Get<int>();
            int magnitude = lineData[ScenarioFields.Arg2].Get<int>();

            string isInstant = lineData[ScenarioFields.PageCtrl].Get<string>();

            if (isInstant == "instant")
            {
                vfxController.ShakeBackgroundAsync(duration, magnitude).Forget();
                return;
            }

            await vfxController.ShakeBackgroundAsync(duration, magnitude);
        }
    }
}
