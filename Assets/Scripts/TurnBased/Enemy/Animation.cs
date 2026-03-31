using UnityEngine;

public class Animation : MonoBehaviour
{
    [SerializeField] private CharacterAnimationData animationData;
    public int CharacterID => animationData.characterID;
    public int AnimationID => animationData.animationID;
    public string AnimationName => animationData.animationName;
    public AnimationClip AnimationClip => animationData.animationClip;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string stateName)
    {
        animator.Play(stateName);
    }

    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }
}