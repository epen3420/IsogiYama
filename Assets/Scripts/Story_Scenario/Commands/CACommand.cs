using CSV4Unity;
using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class CACommand : CommandBase
    {
        VFXController vfxController;

        public CACommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            string isSet = lineData[ScenarioFields.Arg4].Get<string>();

            if (isSet == "disable")
            {
                vfxController.SetChromaticAberration(false);
            }
            else if (isSet == "enable")
            {
                vfxController.SetChromaticAberration(true);
            }
        }
    }
}
