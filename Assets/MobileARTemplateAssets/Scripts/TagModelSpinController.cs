using UnityEngine;

/// <summary>
/// Runtime spin controller that rotates a model around a specified axis using RPM values.
/// Designed to be configured programmatically when an AprilTag model spawns.
/// </summary>
public class TagModelSpinController : MonoBehaviour
{
    [Header("Spin Configuration")]
    [SerializeField]
    [Tooltip("Should the spin logic run automatically?")]
    bool m_SpinEnabled = true;

    [SerializeField]
    [Tooltip("Default RPM applied if no runtime configuration is provided (negative = counter-clockwise).")]
    float m_DefaultRPM = 0f;

    [SerializeField]
    [Tooltip("Axis to rotate around. Defaults to Vector3.up (Y axis).")]
    Vector3 m_RotationAxis = Vector3.up;

    [SerializeField]
    [Tooltip("Whether to rotate in local or world space.")]
    Space m_RotationSpace = Space.Self;

    [SerializeField]
    [Tooltip("Default ramp duration (seconds) when using default RPM.")]
    float m_DefaultRampDuration = 0f;

    float m_StartRPM;
    float m_TargetRPM;
    float m_RampDuration;
    float m_RampElapsed;
    bool m_RuntimeConfigured;

    static readonly float k_DegreesPerMinute = 360f;

    void Awake()
    {
        ResetToDefaults();
    }

    /// <summary>
    /// Configure the spin behaviour at runtime.
    /// </summary>
    /// <param name="enabled">Whether to enable spinning.</param>
    /// <param name="initialRPM">RPM at spawn time (negative = counter-clockwise).</param>
    /// <param name="targetRPM">RPM after ramp completes (negative = counter-clockwise).</param>
    /// <param name="rampDurationSeconds">Time in seconds to ramp from initial to target RPM.</param>
    public void ConfigureSpin(bool enabled, float initialRPM, float targetRPM, float rampDurationSeconds)
    {
        m_SpinEnabled = enabled;
        m_RuntimeConfigured = true;
        m_StartRPM = initialRPM;
        m_TargetRPM = targetRPM;
        m_RampDuration = Mathf.Max(0f, rampDurationSeconds);
        m_RampElapsed = 0f;
    }

    /// <summary>
    /// Disable spinning entirely.
    /// </summary>
    public void DisableSpin()
    {
        m_SpinEnabled = false;
        m_RuntimeConfigured = true;
    }

    void Update()
    {
        if (!m_SpinEnabled)
        {
            return;
        }

        Vector3 axis = m_RotationAxis;
        if (axis.sqrMagnitude < Mathf.Epsilon)
        {
            axis = Vector3.up;
        }
        axis.Normalize();

        float rpm = GetCurrentRPM();
        float degreesPerSecond = (rpm * k_DegreesPerMinute) / 60f;
        float deltaDegrees = degreesPerSecond * Time.deltaTime;

        transform.Rotate(axis * deltaDegrees, m_RotationSpace);

        if (m_RampDuration > 0f && m_RampElapsed < m_RampDuration)
        {
            m_RampElapsed += Time.deltaTime;
        }
    }

    float GetCurrentRPM()
    {
        if (!m_RuntimeConfigured)
        {
            // Use defaults set in inspector
            if (m_DefaultRampDuration > 0f)
            {
                float t = Mathf.Clamp01(m_RampElapsed / m_DefaultRampDuration);
                return Mathf.Lerp(m_StartRPM, m_DefaultRPM, t);
            }
            return m_DefaultRPM;
        }

        if (m_RampDuration <= 0f)
        {
            return m_TargetRPM;
        }

        float tRamp = Mathf.Clamp01(m_RampElapsed / m_RampDuration);
        return Mathf.Lerp(m_StartRPM, m_TargetRPM, tRamp);
    }

    void ResetToDefaults()
    {
        m_StartRPM = m_DefaultRPM;
        m_TargetRPM = m_DefaultRPM;
        m_RampDuration = Mathf.Max(0f, m_DefaultRampDuration);
        m_RampElapsed = 0f;
        m_RuntimeConfigured = false;
    }
}


