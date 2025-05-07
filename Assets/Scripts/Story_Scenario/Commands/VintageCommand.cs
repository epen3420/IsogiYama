using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class VintageCommand : CommandBase
    {
        VFXController vfxController;

        public VintageCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {

        }
    }
}
