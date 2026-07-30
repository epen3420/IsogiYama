using CSV4Unity;
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

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            string isSet = lineData[ScenarioFields.Arg4].Get<string>();

            if(isSet == "disable")
            {
                vfxController.SetBloom(false);
            }
            else if(isSet == "enable")
            {
                // float intensity = lineData[ScenarioFields.Arg1].Get<float>();
                // float threshold = lineData[ScenarioFields.Arg2].Get<float>();
                // float scatter = lineData[ScenarioFields.Arg3].Get<float>();

                // vfxController.SetBloomParameters(intensity, threshold, scatter);
                vfxController.SetBloom(true);
            }
        }
    }
}
