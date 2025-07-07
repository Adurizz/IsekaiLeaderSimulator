using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMoveHandler : MonoBehaviour
{
    [SerializeField] private FloatingJoystick joy;
    [SerializeField] private float speed;

    private Rigidbody rigid;
    private Animator anim;
    private Vector3 moveVec;
    private Player playerScript;

    [SerializeField] private bool isStatInitiated = false;

    void Awake()
    {
        playerScript = GetComponent<Player>();

        playerScript.onStatInitiated.RemoveListener(CheckPlayerStatInitiated);
        playerScript.onStatInitiated.AddListener(CheckPlayerStatInitiated);
        rigid = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();
    }

    private void CheckPlayerStatInitiated()
    {
        isStatInitiated = true;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isStatInitiated);
    }

    void FixedUpdate()
    {
        if (!isStatInitiated)
            return;

        // 1. Input Value
        float x = joy.Horizontal;
        float z = joy.Vertical;

        // 2. Move Position 
        moveVec = new Vector3(x, 0, z) * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVec);

        if (moveVec.sqrMagnitude == 0)
            return; // #. No input = No Rotation

        // 3. Move Rotation
        Quaternion dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);
        rigid.MoveRotation(moveQuat);
    }

    void LateUpdate()
    {
        //anim.SetFloat("Move", moveVec.sqrMagnitude);
    }

}
