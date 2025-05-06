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
            string duration = lineData.Get<string>(ScenarioFields.Arg1);
        }
    }
}
