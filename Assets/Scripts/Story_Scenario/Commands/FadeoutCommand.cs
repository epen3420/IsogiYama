using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class FadeoutCommand : CommandBase
    {
        VFXController vfxController;

        public FadeoutCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            float duration = lineData.Get<float>(ScenarioFields.Arg1);

            await vfxController.FadeInCanvasAsync(duration);
        }
    }
}
