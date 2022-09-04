using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    [Tooltip("Maximum slope the character can jump on")]
    [Range(5.0f, 60.0f)]
    public float slopeLimit = 45.0f;
    [Tooltip("Move speed in meters/second")]
    public float moveSpeed = 6.0f;
    [Tooltip("Turn speed in degrees/second, left (+) or right (-)")]
    public float turnSpeed = 300.0f;
    [Tooltip("Whether the character can jump")]
    public bool allowJump = true;
    [Tooltip("Upward speed to apply when jumping in meters/second")]
    public float jumpSpeed = 5.0f;
    [Tooltip("Whether the character can shoot or throw")]
    public bool allowBattle = true;
    [Tooltip("The number of bullets character can carry")]
    public int maxBulletNum = 10;
    [Tooltip("Cool time between shooting bullets (seconds)")]
    public float bulletCoolTime = 1.0f;
    [Tooltip("The number of bombs character can carry")]
    public int maxBombNum = 1;
    [Tooltip("Cool time between throwing bombs (seconds)")]
    public float bombCoolTime = 5.0f;
    [Tooltip("Bullet or bomb spawn point")]
    public Transform shootingPoint;
    public Projectile projectile;

    public bool IsGrounded { get; private set; }
    public float ForwardInput { get; set; }
    public float TurnInput { get; set; }
    public bool JumpInput { get; set; }
    public bool ShootInput { get; set; }
    public bool ThrowInput { get; set; }

    new private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private int bulletNum;
    private float bulletCoolingTime = 0.0f;
    private int bombNum;
    private float bombCoolingTime = 0.0f;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        bulletNum = maxBulletNum;
        bombNum = maxBombNum;
    }

    private void FixedUpdate() {
        CheckCrounded();
        ProcessMovements();
        if (allowBattle)
            ProcessBattle();
    }

    private void CheckCrounded() {
        IsGrounded = false;
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2.0f, capsuleCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2.0f);
        float radius = transform.TransformVector(capsuleCollider.radius, 0.0f, 0.0f).magnitude;
        Ray ray = new Ray(capsuleBottom + transform.up * 0.01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5.0f)) {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < slopeLimit) {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + 0.02f;
                if (hit.distance < maxDist)
                    IsGrounded = true;
            }
        }
    }

    private void ProcessMovements() {
        // Process Turning
        if (TurnInput != 0.0f) {
            float angle = Mathf.Clamp(TurnInput, -1.0f, 1.0f) * turnSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }
        // Process Movement/Jumping
        if (IsGrounded) {
            // Reset the velocity
            rigidbody.velocity = Vector3.zero;
            // Check if trying to jump
            if (JumpInput && allowJump) {
                // Apply an upward velocity to jump
                rigidbody.velocity += Vector3.up * jumpSpeed;
            }
            // Apply a forward or backward velocity based on player input
            rigidbody.velocity += transform.forward * Mathf.Clamp(ForwardInput, -1.0f, 1.0f) * moveSpeed;
        }
        else {
            // Check if player is trying to change forward/backward movement while jumping/falling
            if(!Mathf.Approximately(ForwardInput, 0.0f)) {
                // Override just the forward velocity with player input at half speed
                Vector3 verticalVelocity = Vector3.Project(rigidbody.velocity, Vector3.up);
                rigidbody.velocity = verticalVelocity + transform.forward * Mathf.Clamp(ForwardInput, -1.0f, 1.0f) * moveSpeed / 2.0f;
            }
        }
    }

    private void ProcessBattle() {
        if (bulletCoolingTime > 0.0f)
            bulletCoolingTime -= Time.fixedDeltaTime;
        else
            bulletCoolingTime = 0.0f;

        if ((bulletNum > 0) && (bulletCoolingTime <= 0.0f)) {
            bulletNum -= 1;
            bulletCoolingTime = bulletCoolTime;
            ShootBullet();
        }

        if (bombCoolingTime > 0.0f)
            bombCoolingTime -= Time.fixedDeltaTime;
        else
            bombCoolingTime = 0.0f;

        if ((bombNum > 0) && (bombCoolingTime <= 0.0f)) {
            bombNum -= 1;
            bombCoolingTime = bombCoolTime;
            ThrowBumb();
        }
    }

    private void ShootBullet() {
        var layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var direction = this.transform.forward;
        
        var spawnedProjectile = Instantiate(projectile, shootingPoint.position, Quaternion.Euler(0f, -90f, 0f));
        spawnedProjectile.SetDirection(direction);

        Debug.DrawRay(transform.position, direction, Color.blue, 1.0f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 200.0f, layerMask)) {
            // TODO
        }
    }

    private void ThrowBumb() {

    }
}
