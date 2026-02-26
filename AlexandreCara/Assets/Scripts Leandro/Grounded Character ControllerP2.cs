using UnityEngine;
using System.Collections;

public class GroundedCharacterControllerP2 : CharacterControllerBase
{
    [SerializeField] int playerID = 2; // Player 2 (Enter)

    [SerializeField] float m_WalkForce = 0.0f;
    [SerializeField] float m_WalkForceApplyLimit = 0.0f;
    [SerializeField] float m_StoppingForce = 0.0f;
    [SerializeField] bool m_ApplyStoppingForceWhenActivelyBraking = false;
    [SerializeField] float m_AirControl = 0.0f;
    [SerializeField] float m_AirForceApplyLimit = 0.0f;
    [SerializeField] float m_DragConstant = 0.0f;
    [SerializeField] float m_Gravity = 0.0f;
    [SerializeField] bool m_ApplyGravityOnGround = false;
    [SerializeField] bool m_ApplyGravityIntoGroundNormal = false;
    [SerializeField] float m_FrictionConstant = 0.0f;
    [SerializeField] bool m_AlignRotationToGroundedNormal = false;

    [SerializeField] float m_JumpVelocity = 0.0f;
    [SerializeField] float m_JumpCutVelocity = 0.0f;
    [SerializeField] float m_MinAllowedJumpCutVelocity = 0.0f;
    [SerializeField] float m_GroundedToleranceTime = 0.0f;
    [SerializeField] float m_JumpCacheTime = 0.0f;
    [SerializeField] float m_JumpAlignedToGroundFactor = 0.0f;
    [SerializeField] float m_HorizontalJumpBoostFactor = 0.0f;
    [SerializeField] bool m_ResetVerticalSpeedOnJumpIfMovingDown = false;

    float m_LastJumpPressedTime;
    bool m_JumpInputIsCached;
    bool m_JumpCutPossible;
    float m_LastJumpTime;
    float m_LastGroundedTime;
    float m_LastTouchingSurfaceTime;

    Vector2 m_LastGroundedNormal;

    public delegate void OnJumpEvent();
    public event OnJumpEvent OnJump;

    protected ButtonInput m_JumpInput;

    void Reset()
    {
        m_WalkForce = 90.0f;
        m_WalkForceApplyLimit = 18.0f;
        m_StoppingForce = 100.0f;
        m_ApplyStoppingForceWhenActivelyBraking = true;
        m_AirControl = 0.6f;
        m_AirForceApplyLimit = 18.0f;
        m_DragConstant = 0.0f;
        m_Gravity = 50.0f;
        m_ApplyGravityOnGround = true;
        m_ApplyGravityIntoGroundNormal = true;
        m_FrictionConstant = 8.0f;
        m_AlignRotationToGroundedNormal = false;
        m_JumpVelocity = 32.0f;
        m_JumpCutVelocity = 0.0f;
        m_MinAllowedJumpCutVelocity = 30.0f;
        m_GroundedToleranceTime = 0.1f;
        m_JumpCacheTime = 0.1f;
        m_JumpAlignedToGroundFactor = 0.0f;
        m_HorizontalJumpBoostFactor = 0.0f;
        m_ResetVerticalSpeedOnJumpIfMovingDown = true;
    }

    protected override void UpdateController()
    {
        bool isGrounded = m_ControlledCollider.IsGrounded();
        if (isGrounded)
        {
            m_LastGroundedTime = Time.fixedTime;
            m_LastGroundedNormal = m_ControlledCollider.GetGroundedInfo().GetNormal();
        }

        if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide)
        {
            m_LastTouchingSurfaceTime = Time.fixedTime;
        }

        if (m_JumpInput != null)
        {
            if (m_JumpInput.m_WasJustPressed)
            {
                m_JumpInput.m_WasJustPressed = false;
                m_LastJumpPressedTime = Time.fixedTime;
                m_JumpInputIsCached = true;
            }

            if (m_JumpInputIsCached)
            {
                if (Time.fixedTime - m_LastJumpPressedTime >= m_JumpCacheTime)
                {
                    m_JumpInputIsCached = false;
                }
            }
        }
    }

    protected override void DefaultUpdateMovement()
    {
        UpdateJumpCut();

        if (TryDefaultJump())
        {
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return;
        }

        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        Vector2 fInput = GetDirectedInputMovement() * GetInputForce();
        fInput = ClampInputVelocity(fInput, currentVel, GetInputForceApplyLimit());

        Vector2 fGravity = GetGravity();
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_DragConstant * currentVel.normalized;
        Vector2 summedF = fInput + fGravity + fDrag;
        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        if (m_ControlledCollider.IsGrounded())
        {
            newVel += GetStoppingForce(newVel, m_StoppingForce);
            Vector2 friction = GetFriction(newVel, summedF, m_FrictionConstant);
            newVel += friction;
        }

        m_ControlledCollider.UpdateWithVelocity(newVel);
        TryAligningWithGround();
    }

    public override void SetPlayerInput(PlayerInput a_PlayerInput)
    {
        base.SetPlayerInput(a_PlayerInput);

        // P2 jump agora é ENTER
        if (a_PlayerInput.GetButton("JumpP2") != null)
        {
            m_JumpInput = a_PlayerInput.GetButton("JumpP2");
        }
        else
        {
            Debug.LogError("Jump input for P2 not set up in character input");
        }
    }

    // O resto do script permanece idêntico ao original para manter a física, colisões e animações
    // ... (Todas as outras funções do seu script original continuam aqui, sem alterações)
}