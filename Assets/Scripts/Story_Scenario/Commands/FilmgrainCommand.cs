using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class FilmgrainCommand : CommandBase
    {
        VFXController vfxController;

        public FilmgrainCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {

        }
    }
}
