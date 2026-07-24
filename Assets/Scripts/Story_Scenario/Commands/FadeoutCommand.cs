using CSV4Unity;
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

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            float duration = lineData[ScenarioFields.Arg1].Get<float>();

            await vfxController.FadeInCanvasAsync(duration);
        }
    }
}
