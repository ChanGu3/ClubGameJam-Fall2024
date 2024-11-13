using UnityEngine;

public class SlimeAnimationEvents : MonoBehaviour
{
    void PlaySplatSound()
    {
        AudioManager.PlaySFX(SFXAudio.SlimeExplode);
    }
}
