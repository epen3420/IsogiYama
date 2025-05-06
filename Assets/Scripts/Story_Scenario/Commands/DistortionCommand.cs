using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class DistortionCommand : CommandBase
    {
        VFXController vfxController;

        public DistortionCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {

        }
    }
}
