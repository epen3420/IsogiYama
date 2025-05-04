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

            await vfxController.ChangeBackgroundAsync(bgNames);
        }
    }
}
