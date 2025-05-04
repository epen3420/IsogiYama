using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class GotoCommand : CommandBase
    {
        ProgressManager progressManager;

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            progressManager = InstanceRegister.Get<ProgressManager>();

            int targetIndex = lineData.Get<int>(ScenarioFields.PageCtrl);

            progressManager.IncrementIndex();
            // Header•ª‚ğl—¶‚µ‚È‚¢‚ÆOutOfIndex‚É‚È‚è‚©‚Ë‚È‚¢ + Index‚à0‚©‚çn‚Ü‚Á‚Ä‚é‚©‚ç2ˆø‚©‚È‚¢‚Æ‚¢‚¯‚È‚¢
            progressManager.IndexSkip(targetIndex - 2);
            await UniTask.Delay(1);
        }
    }
}