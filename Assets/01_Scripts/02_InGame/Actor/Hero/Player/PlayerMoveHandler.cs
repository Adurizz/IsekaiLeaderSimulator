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
    private PlayerAttackHandler attackHandler;
    private GameObject target;

    void Awake()
    {
        playerScript = GetComponent<Player>();

        playerScript.onStatInitiated.RemoveListener(CheckPlayerStatInitiated);
        playerScript.onStatInitiated.AddListener(CheckPlayerStatInitiated);
        rigid = GetComponent<Rigidbody>();
        attackHandler = GetComponent<PlayerAttackHandler>();
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

        Move();
        Rotate();
    }

    private void Move()
    {
        // 조이스틱에서 input 받아옴
        float x = joy.Horizontal;
        float z = joy.Vertical;

        // 이동
        moveVec = new Vector3(x, 0, z) * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVec);
    }

    private void Rotate()
    {
        // 입력이 없을 때 회전 X
        if (moveVec.sqrMagnitude == 0)
            return;

        if (target == null)
        {
            // 회전
            Quaternion dirQuat = Quaternion.LookRotation(moveVec);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);
            rigid.MoveRotation(moveQuat);
        }
        else
        {
            Quaternion dirQuat = Quaternion.LookRotation(target.transform.position - transform.position);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);
            rigid.MoveRotation(moveQuat);
        }
    }

    void LateUpdate()
    {
        //anim.SetFloat("Move", moveVec.sqrMagnitude);
    }

    public void SetTarget(GameObject obj)
    {
        target = obj;
    }
}
