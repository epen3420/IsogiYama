using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class DepthfieldCommand : CommandBase
    {
        VFXController vfxController;

        public DepthfieldCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {

        }
    }
}
