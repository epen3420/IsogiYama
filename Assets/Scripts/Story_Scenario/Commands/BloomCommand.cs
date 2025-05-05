using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class BloomCommand : CommandBase
    {
        VFXController vfxController;

        public BloomCommand()
        {
            vfxController = InstanceRegister.Get<VFXController>();
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            string isSet = lineData.Get<string>(ScenarioFields.Arg4);

            if(isSet == "disable")
            {
                vfxController.SetBloom(false);
            }
            else
            {
                float intensity = lineData.Get<float>(ScenarioFields.Arg1);
                float threshold = lineData.Get<float>(ScenarioFields.Arg2);
                float scatter = lineData.Get<float>(ScenarioFields.Arg3);

                vfxController.SetBloomParameters(intensity, threshold, scatter);
                vfxController.SetBloom(true);
            }
        }
    }
}
