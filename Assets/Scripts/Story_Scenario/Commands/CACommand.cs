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

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            string isSet = lineData.Get<string>(ScenarioFields.Arg4);

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
