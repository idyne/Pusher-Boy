using System;
using UnityEngine;
using FateGames;

public class Player : CustomizableCharacter
{
    [SerializeField] private float speed = 1;
    [SerializeField] private Transform raycasterTransform = null;
    [SerializeField] private LayerMask obstacleLayerMask = 0;
    private Vector3 lastDir;
    private PlayerState state = PlayerState.IDLE;
    private Vector2 mouseAnchor = Vector2.zero;
    private Vector2 dif = Vector3.zero;
    private float screenWidth = 0;
    private Enemy currentEnemy = null;
    private float pushTime = 0;
    private Board board = null;
    private string nextAnimTrigger = "";


    protected new void Awake()
    {
        base.Awake();
        board = FindObjectOfType<Board>();
        screenWidth = Screen.width;
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameState.IN_GAME)
        {
            CheckInput();
            if (state == PlayerState.PUSHING)
                Push();
        }
    }

    private void LateUpdate()
    {
        if (nextAnimTrigger != "")
        {
            SetAnimatorTrigger(nextAnimTrigger);
            nextAnimTrigger = "";
        }
    }

    private void CheckInput()
    {
        if (state == PlayerState.IDLE)
            CheckMouseInput();
    }

    private void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            mouseAnchor = Input.mousePosition;
        else if (Input.GetMouseButton(0) && mouseAnchor.magnitude != 0)
        {
            dif = (Vector2)Input.mousePosition - mouseAnchor;
            if (dif.magnitude > screenWidth / 10f)
            {
                Vector3 direction;
                if (Mathf.Abs(dif.x) >= Mathf.Abs(dif.y))
                    direction = dif.x >= 0 ? Vector3.right : Vector3.left;
                else
                    direction = dif.y >= 0 ? Vector3.forward : Vector3.back;
                mouseAnchor = Input.mousePosition;
                Slide(direction, direction);
            }
            dif = Vector2.zero;
        }
    }

    private void Slide(Vector3 direction, Vector3 lookDirection)
    {
        if (SendRaycast(out RaycastHit hit, direction))
        {
            bool hitEnemy = hit.transform.CompareTag("Enemy");
            bool hitWall = hit.transform.CompareTag("Wall");
            if (hitEnemy || hitWall)
            {
                Vector3 to = hit.transform.position - direction;
                Action callback;
                if (hitEnemy)
                {
                    callback = () =>
                    {
                        ChangeState(PlayerState.PUSHING);
                        currentEnemy.SetAnimatorTrigger("Push");
                    };
                    currentEnemy = hit.transform.GetComponent<Enemy>();
                }
                else callback = () => { ChangeState(PlayerState.IDLE); };
                lastDir = direction;
                Move(to, lookDirection, hitEnemy, callback);
            }
        }
    }

    private void ChangeState(PlayerState newState)
    {
        state = newState;
        switch (state)
        {
            case PlayerState.IDLE:
                nextAnimTrigger = "Idle";
                break;
            case PlayerState.MOVING:
                nextAnimTrigger = "Move";
                break;
            case PlayerState.PUSHING:
                nextAnimTrigger = "Push";
                break;
        }
    }

    private bool SendRaycast(out RaycastHit hit, Vector3 direction)
    {
        return Physics.Raycast(raycasterTransform.position, direction, out hit, 30, obstacleLayerMask);
    }
    private void Move(Vector3 to, Vector3 lookDirection, bool isEnemy, Action callback, LeanTweenType easeType = LeanTweenType.easeInCubic)
    {
        float distance = Vector3.Distance(_transform.position, to);
        if (!isEnemy && distance < 0.1f) return;
        ChangeState(PlayerState.MOVING);
        _transform.rotation = Quaternion.LookRotation(lookDirection);
        float t = distance / speed;
        _transform.LeanMove(to, t).setEase(easeType);
        LeanTween.delayedCall(t, () => { callback(); });
    }

    private void HitEnemy()
    {
        currentEnemy.Die();
    }

    private void Bounce()
    {
        Slide(-lastDir, lastDir);
    }

    private void Push()
    {
        if (!currentEnemy) return;
        if (fatness == 0 && currentEnemy.Fatness > 0)
        {
            ObjectPooler.Instance.SpawnFromPool("Hit Effect", currentEnemy.transform.position + Vector3.up + currentEnemy.transform.forward * 0.5f, Quaternion.LookRotation(currentEnemy.transform.forward));
            Bounce();
            //ChangeState(PlayerState.IDLE);
            currentEnemy.SetAnimatorTrigger("Idle");
            currentEnemy = null;
            return;
        }
        else if (currentEnemy.Fatness == 0)
        {
            ObjectPooler.Instance.SpawnFromPool("Hit Effect", currentEnemy.transform.position + Vector3.up + currentEnemy.transform.forward * 0.5f, Quaternion.LookRotation(currentEnemy.transform.forward));
            board.FillGap(currentEnemy.transform.position);
            HitEnemy();
            ChangeState(PlayerState.IDLE);
            currentEnemy = null;
            pushTime = 0;
            return;
        }
        currentEnemy.Slip();
        float cd = 0.5f;
        if (pushTime >= cd)
        {
            pushTime -= cd;
            HapticManager.Instance.DoHaptic();
            currentEnemy.LoseWeight();
            LoseWeight();
        }
        pushTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            HapticManager.Instance.DoHaptic();
            other.GetComponent<Collider>().enabled = false;
            other.transform.LeanScale(Vector3.zero, 0.2f);
            SetFatness(fatness + other.GetComponent<Food>().Fat);
        }
    }
    protected override Vector3 CalculateLocalScale()
    {
        return new Vector3(1 + fatness * 4 / 100f, 1 + fatness * 4 / 200f, 1 + fatness * 4 / 100f);
    }
    private enum PlayerState { IDLE, MOVING, PUSHING }
    private enum ObstacleType { ENEMY, WALL }
}
