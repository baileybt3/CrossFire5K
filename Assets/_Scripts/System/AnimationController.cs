using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator anim;
    public float maxSpeed = 4f;
    public float hasGun = 1;

    Vector3 lastPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float mps = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = transform.position;
        float speed01 = Mathf.Clamp01(mps / Mathf.Max(maxSpeed, 0.0001f));
        anim.SetFloat("Speed", speed01);
        float gun = 1.0f;
        anim.SetFloat("hasGun", gun);
    }
}
