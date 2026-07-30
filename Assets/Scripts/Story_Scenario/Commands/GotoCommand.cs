using CSV4Unity;
using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class GotoCommand : CommandBase
    {
        ProgressManager progressManager;

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            progressManager = InstanceRegister.Get<ProgressManager>();

            int targetIndex = lineData[ScenarioFields.PageCtrl].Get<int>();

            progressManager.IncrementIndex();
            // Header分を考慮しないとOutOfIndexになりかねない + Indexも0から始まってるから2引かないといけない
            progressManager.IndexSkip(targetIndex - 2);
            await UniTask.Delay(1);
        }
    }
}