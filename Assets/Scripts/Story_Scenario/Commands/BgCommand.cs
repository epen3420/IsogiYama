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

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            string bgNames = lineData.Get<string>(ScenarioFields.Arg1);
            string isInstant = lineData.Get<string>(ScenarioFields.Arg2);
            float duration = lineData.Get<float>(ScenarioFields.Arg3);

            if (isInstant == "instant")
            {
                vfxController.ChangeBackgroundAsync(bgNames, duration).Forget();
                return;
            }

            await vfxController.ChangeBackgroundAsync(bgNames, duration);
        }
    }
}
