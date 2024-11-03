using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RipplesHandler : MonoBehaviour
{
    void Update()
    {
        // 각 키가 눌린 상태에서 파티클을 재생
        if (Input.GetKey(KeyCode.Alpha1)) TriggerParticleEffect(ColorType.Red);
        if (Input.GetKey(KeyCode.Alpha2)) TriggerParticleEffect(ColorType.Orange);
        if (Input.GetKey(KeyCode.Alpha3)) TriggerParticleEffect(ColorType.Yellow);
        if (Input.GetKey(KeyCode.Alpha4)) TriggerParticleEffect(ColorType.Green);
        if (Input.GetKey(KeyCode.Alpha5)) TriggerParticleEffect(ColorType.Blue);
        if (Input.GetKey(KeyCode.Alpha6)) TriggerParticleEffect(ColorType.Indigo);
        if (Input.GetKey(KeyCode.Alpha7)) TriggerParticleEffect(ColorType.Violet);

        // 키가 떼어질 때 해당 색상을 제거
        if (Input.GetKeyUp(KeyCode.Alpha1)) GameManager.em.RemoveColorFromRipples(ColorType.Red);
        if (Input.GetKeyUp(KeyCode.Alpha2)) GameManager.em.RemoveColorFromRipples(ColorType.Orange);
        if (Input.GetKeyUp(KeyCode.Alpha3)) GameManager.em.RemoveColorFromRipples(ColorType.Yellow);
        if (Input.GetKeyUp(KeyCode.Alpha4)) GameManager.em.RemoveColorFromRipples(ColorType.Green);
        if (Input.GetKeyUp(KeyCode.Alpha5)) GameManager.em.RemoveColorFromRipples(ColorType.Blue);
        if (Input.GetKeyUp(KeyCode.Alpha6)) GameManager.em.RemoveColorFromRipples(ColorType.Indigo);
        if (Input.GetKeyUp(KeyCode.Alpha7)) GameManager.em.RemoveColorFromRipples(ColorType.Violet);

        // 모든 키 입력이 없을 때 파티클을 멈춤
        if (!Input.GetKey(KeyCode.Alpha1) && !Input.GetKey(KeyCode.Alpha2) && !Input.GetKey(KeyCode.Alpha3) &&
            !Input.GetKey(KeyCode.Alpha4) && !Input.GetKey(KeyCode.Alpha5) && !Input.GetKey(KeyCode.Alpha6) && 
            !Input.GetKey(KeyCode.Alpha7))
        {
            GameManager.em.StopRipples();
        }
    }

    private void TriggerParticleEffect(ColorType colorType)
    {
        GameManager.em.TriggerRipples(transform.position, transform, colorType, transform.localScale);
    }
}
