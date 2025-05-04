using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class DelayCommand : CommandBase
    {
        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            int waitMSec = lineData.Get<int>(ScenarioFields.Arg1);

            await UniTask.Delay(waitMSec);
        }
    }
}