using CSV4Unity;
using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class BgCommand : CommandBase
    {
        VFXController vfxController;

        public BgCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            string bgNames = lineData[ScenarioFields.Arg1].Get<string>();
            float duration = lineData[ScenarioFields.Arg2].Get<float>();

            string isInstant = lineData[ScenarioFields.PageCtrl].Get<string>();

            if (isInstant == "instant")
            {
                vfxController.ChangeBackgroundAsync(bgNames, duration).Forget();
                return;
            }

            await vfxController.ChangeBackgroundAsync(bgNames, duration);
        }
    }
}
