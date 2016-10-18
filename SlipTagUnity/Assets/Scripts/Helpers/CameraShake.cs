using UnityEngine;
using System.Collections;

public class CameraShake : ShakingObj
{
    private static UID freeze_timescale_id = new UID();

    public void Shake(CamShakeParams shake_params)
    {
        FreezeFrames(shake_params.freeze_frames);
        Shake(shake_params.shake_params);
    }
    public void ShakeSmall()
    {
        Shake(0.2f, 0.5f, 1);
    }
    public void ShakeMed()
    {
        FreezeFrames(3);
        Shake(0.2f, 2, 0.3f);
    }
    public void ShakeBig()
    {
        FreezeFrames(3);
        Shake(0.3f, 2, 0.8f);
    }
    public void FreezeFrames(int frames)
    {
        StartCoroutine(Freeze(frames));
    }
    protected override void Awake()
    {
        base.Awake();
    }

    private IEnumerator Freeze(float frames = 3)
    {
        TimeScaleManager.SetFactor(0, freeze_timescale_id);
        for (int i = 0; i < frames; ++i)
        {
            yield return null;
        }
        TimeScaleManager.SetFactor(1, freeze_timescale_id);
    }   
}
public class CamShakeParams
{
    public ShakeParams shake_params;
    public int freeze_frames = 0;
    
    public CamShakeParams()
    {

    }
    public CamShakeParams(float duration, float intensity, float speed, int freeze_frames)
    {
        shake_params = new ShakeParams(duration, intensity, speed);
        this.freeze_frames = freeze_frames;
    }
}

