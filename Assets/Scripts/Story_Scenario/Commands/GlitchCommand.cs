using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class GlitchCommand : CommandBase
    {
        VFXController vfxController;

        public GlitchCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            int duration = lineData.Get<int>(ScenarioFields.Arg1);
            int magnitude = lineData.Get<int>(ScenarioFields.Arg2);
        }
    }
}
