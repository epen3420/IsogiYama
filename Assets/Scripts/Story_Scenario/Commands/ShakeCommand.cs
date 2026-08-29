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
            float duration = lineData[ScenarioFields.Arg1].Get<float>();
            float magnitude = lineData[ScenarioFields.Arg2].Get<float>();

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
