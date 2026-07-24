using CSV4Unity;
using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class DelayCommand : CommandBase
    {
        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            int waitMSec = lineData[ScenarioFields.Arg1].Get<int>();

            await UniTask.Delay(waitMSec);
        }
    }
}