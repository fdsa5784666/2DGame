using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleModeButton : MonoBehaviour
{
    [SerializeField]
    private GameObject characterChoose;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip clip;

    private void Start()
    {

        characterChoose.SetActive(false);
    }
    public void OnClick_SimpleMode()
    {
        StartCoroutine(CharacterChoosePlay());
    }

    IEnumerator CharacterChoosePlay()
    {
        characterChoose.SetActive(true);
        animator.SetTrigger("Play");

        yield return null;
        yield return new WaitForSecondsRealtime(clip.length);
    }
}