using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class PandaAI : RichPetAI {
    private float idleTime = -1f;
    private float IDLE_ANIM_DURATION = 3.8f;
    private RichPetAI baseAI;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(RandomIdle());
        InvokeRepeating("Wander", 0f, 15f);
    }

    public override void GoToHooman()
    {
        base.GoToHooman();
        audioManager.PlayEvent("react");       
    }

    void OnToyThrown(GameObject toy)
    {
        GoToShinyObject(toy.transform.position);
        audioManager.PlayEvent("react");
    }

    IEnumerator RandomIdle()
    {
        while (true) {
            if (lastPos == transform.position)
            {
                if (idleTime == -1f)
                {
                    idleTime = Time.time;
                }

                // Maybe play a random idle animation.
                else if (Time.time - idleTime >= 5f && Random.Range(0, 3) == 2)
                {
                    base.headAnimating = true;

                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            animator.SetTrigger("IdleB");
                            break;

                        case 1:
                            animator.SetTrigger("IdleC");
                            break;

                        default:
                            animator.SetTrigger("IdleD");
                            break;
                    }

                    idleTime = -1f;
                    yield return new WaitForSeconds(IDLE_ANIM_DURATION);
                    animator.SetTrigger("backToIdleA");
                    base.headAnimating = false;
                }
            }

            yield return new WaitForSeconds(2f);
        }
    }
}
