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
            // int duration = lineData.Get<int>(ScenarioFields.Arg1);
            // int magnitude = lineData.Get<int>(ScenarioFields.Arg2);

            string isSet = lineData.Get<string>(ScenarioFields.Arg4);

            // string isInstant = lineData.Get<string>(ScenarioFields.PageCtrl);

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
