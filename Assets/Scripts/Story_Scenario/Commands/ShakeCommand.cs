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

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            int duration = lineData.Get<int>(ScenarioFields.Arg1);
            int magnitude = lineData.Get<int>(ScenarioFields.Arg2);

            string isInstant = lineData.Get<string>(ScenarioFields.PageCtrl);

            if (isInstant == "instant")
            {
                vfxController.ShakeBackgroundAsync(duration, magnitude).Forget();
                return;
            }

            await vfxController.ShakeBackgroundAsync(duration, magnitude);
        }
    }
}
