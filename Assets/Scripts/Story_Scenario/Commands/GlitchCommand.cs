using CSV4Unity;
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

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            // int duration = lineData[ScenarioFields.Arg1].Get<int>();
            // int magnitude = lineData[ScenarioFields.Arg2].Get<int>();

            string isSet = lineData[ScenarioFields.Arg4].Get<string>();

            // string isInstant = lineData[ScenarioFields.PageCtrl].Get<string>();

            if(isSet == "disable")
            {
                vfxController.SetGlitch(false);
                return;
            }

            vfxController.SetGlitch(true);
            return;
        }
    }
}
