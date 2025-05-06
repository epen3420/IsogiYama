using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraShakeSample : MonoBehaviour
{
    VFXController vfx;
    public float magnitude;
    public float duration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vfx = InstanceRegister.Get<VFXController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //vfx.ShakeBackgroundAsync(duration, magnitude).Forget();
            //vfx.SetGlitch(true);
            vfx.SetChromaticAberration(true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            //vfx.ShakeBackgroundAsync(duration, magnitude).Forget();
            //vfx.SetGlitch(false);
            vfx.SetChromaticAberration(false);
        }
    }
}
