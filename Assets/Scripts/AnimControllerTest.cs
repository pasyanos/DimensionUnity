using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControllerTest : MonoBehaviour
{
    [SerializeField] private List<Animator> animators = new List<Animator>();


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogError(":^)");
            foreach (var animator in animators)
            {
                animator.SetTrigger("AttackTrigger");
            }
        }
    }
}
