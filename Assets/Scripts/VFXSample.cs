using Cysharp.Threading.Tasks;
using UnityEngine;

public class VFXSample : MonoBehaviour
{

    [SerializeField] string bg = "303";
    [SerializeField] float duration = 0.5f;
    VFXController vfx;

    void Start()
    {
        vfx = InstanceRegister.Get<VFXController>();

        Debug.Log(vfx);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            vfx.ChangeBackground(bg, duration).Forget();
        }
    }
}
