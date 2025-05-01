using UnityEngine;
using IsogiYama.Commands;

public class CommandFactory
{
    public CommandBase CreateCommandInstance(string cmdRow)
    {
        CommandBase tmp = null;

        switch (cmdRow)
        {
            case "":
                // 1. スキップ
                break;

            case "Text":
                // 2. Text表示
                tmp = new TextCommand();
                break;

            default:
                Debug.LogError("Command Was Not Found");
                break;
        }

        return tmp;
    }
}
