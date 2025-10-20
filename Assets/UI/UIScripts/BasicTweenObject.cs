using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// ��Ҫȷ����������canvasGroup
/// </summary>
public class BasicTweenObject : MonoBehaviour
{

    protected event Action<Vector3> RotationChanged;

    private int PositionTweenId = 0;

    private int ScaleTweenId = 0;

    private int LocalRotationId = 0;

    private bool IsTimeScaled = true;

    protected Func<float> GetTimeDelta = GetScaledTimeDelta;

    protected VibrationCustomData vibrationData = new();

    public int GetPId()
    {
        return PositionTweenId;
    }
    /// <summary>
    /// �������DoTween������ֱ�Ӹ������꣬��Ҫʹ����������������쳣���
    /// </summary>
    public virtual void StopEveryTween()
    {
        PositionTweenId++;
        ScaleTweenId++;
        LocalRotationId++;
    }
    public void StopNowScaleTween()
    {
        ScaleTweenId++;
    }
    public void StopNowPositionTween()
    {
        PositionTweenId++;
    }
    public void SetWorldPosition(Vector3 p)
    {
        PositionTweenId++;
        transform.position = p;
    }
    public void SetLocalPosition(Vector3 p)
    {
        PositionTweenId++;
        transform.localPosition = p;
    }
    public void AddLocalPosition(Vector3 p)
    {
        PositionTweenId++;
        transform.localPosition += p;
    }
    public void SetLocalScale(Vector3 scale)
    {
        transform.localScale = scale;
        ScaleTweenId++;
    }
    public void SetLocalRotation(Vector3 rotation)
    {
        transform.localRotation = Quaternion.Euler(rotation);
        RotationChanged?.Invoke(transform.rotation.eulerAngles);
        LocalRotationId = 0;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void DoLocalPositionTween(Vector3 position, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        StartCoroutine(LocalPositionTween(position, time, type, IsCallbackRequiredOnComplete, callBack));
    }
    public void DoPositionTween(Vector3 position, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        StartCoroutine(WorldPositionTween(position, time, callBack, type, IsCallbackRequiredOnComplete));
    }

    public void DoLocalScaleTween(Vector3 scale, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        StartCoroutine(LocalScaleTween(scale, time, type, callBack, IsCallbackRequiredOnComplete));
    }
    public void DoLocalRotationTween(Quaternion quaternion, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad)
    {
        StartCoroutine(LocalRotationTween(quaternion, time, callBack, type));
    }
    public void DoLocalRotationTween(Vector3 rotation, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad)
    {
        Quaternion quaternion = Quaternion.Euler(rotation);
        DoLocalRotationTween(quaternion, time, callBack, type);
    }

    public void DoDelay(float time, Action callBack = null)
    {
        StartCoroutine(Delay(time, callBack));
    }
    IEnumerator Delay(float time, Action callback)
    {
        if (IsTimeScaled == true)
        {
            yield return new WaitForSeconds(time);
        }
        else
        {
            yield return new WaitForSecondsRealtime(time);
        }

        callback?.Invoke();
    }



    IEnumerator LocalPositionTween(Vector3 targetPosition, float time, EaseType type, bool IsCallbackRequiredOnComplete, Action callback)
    {
        PositionTweenId++;
        int id = PositionTweenId;
        Vector3 startPositon = transform.localPosition;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == PositionTweenId)
            {
                transform.localPosition = Vector3.LerpUnclamped(startPositon, targetPosition, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 Delta = targetPosition - startPositon;
            while (nowTime < time && id == PositionTweenId)
            {
                transform.localPosition = startPositon + Vector3.Scale( V3ShakeFunc(nowTime / time) , Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }


        if (id == PositionTweenId)
        {
            if (IsToTarget(type) == true)
            {
                transform.localPosition = targetPosition;
            }
            else
            {
                transform.localPosition = startPositon;
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
            //���ж�
        }
    }
    IEnumerator WorldPositionTween(Vector3 targetPosition, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        PositionTweenId++;
        int id = PositionTweenId;

        Vector3 startPositon = transform.position;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == PositionTweenId)
            {
                transform.position = Vector3.LerpUnclamped(startPositon, targetPosition, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 Delta = targetPosition - startPositon;
            while (nowTime < time && id == PositionTweenId)
            {
                transform.position = startPositon + Vector3.Scale(V3ShakeFunc(nowTime / time), Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }

        if (id == PositionTweenId)
        {
            if (IsToTarget(type) == true)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = startPositon;
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
            //���ж�
        }
    }
    IEnumerator LocalScaleTween(Vector3 TargetScale, float time, EaseType type, Action callback, bool IsCallbackRequiredOnComplete)
    {
        ScaleTweenId++;
        int id = ScaleTweenId;

        Vector3 startScale = transform.localScale;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == ScaleTweenId)
            {
                transform.localScale = Vector3.LerpUnclamped(startScale, TargetScale, EaseFunc(nowTime / time));
                yield return null;
                nowTime += GetTimeDelta();
            }
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 Delta = TargetScale - startScale;
            while (nowTime < time && id == ScaleTweenId)
            {
                transform.localScale = startScale + Vector3.Scale(V3ShakeFunc(nowTime / time), Delta);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }

        if (id == ScaleTweenId)
        {
            if (IsToTarget(type) == true)
            {
                transform.localScale = TargetScale;
            }
            else
            {
                transform.localScale = startScale;
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
        }
    }

    IEnumerator LocalRotationTween(Quaternion targetQuaternion, float time, Action callback, EaseType type )
    {
        LocalRotationId++;
        int id = LocalRotationId;

        Quaternion StartQuaternion = transform.localRotation;

        float nowTime = 0;
        nowTime += GetTimeDelta();

        if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == LocalRotationId)
            {
                transform.localRotation = Quaternion.SlerpUnclamped(StartQuaternion, targetQuaternion, EaseFunc(nowTime / time));
                RotationChanged?.Invoke(transform.rotation.eulerAngles);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }//�ܲ��ң�������Ԫ���Ĺ��򣬲��ܼ򵥵Ľ��ĸ������������������
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector3 startEuler = StartQuaternion.eulerAngles;
            Vector3 endEuler = targetQuaternion.eulerAngles;
            while (nowTime < time && id == LocalRotationId)
            {
                Vector3 v3Shake = V3ShakeFunc(nowTime / time);
                Vector3 resultEuler =
                    new Vector3(
                            LerpAngleUnclamped(startEuler.x, endEuler.x, v3Shake.x),
                            LerpAngleUnclamped(startEuler.y, endEuler.y, v3Shake.y),
                            LerpAngleUnclamped(startEuler.z, endEuler.z, v3Shake.z)
                         );
                transform.localRotation = Quaternion.Euler(resultEuler);
                RotationChanged?.Invoke(transform.rotation.eulerAngles);
                yield return null;
                nowTime += GetTimeDelta();
            }
        }

        if (id == LocalRotationId)
        {
            if (IsToTarget(type) == true)
            {
                transform.localRotation = targetQuaternion;
            }
            else
            {
                transform.localRotation = StartQuaternion;
            }
            RotationChanged?.Invoke(transform.rotation.eulerAngles);
            callback?.Invoke();
        }
        else
        {
            //���ж�
        }
    }
    public void SetTimeScaled(bool IsScaled)
    {
        IsTimeScaled = IsScaled;
        if(IsScaled == false)
        {
            GetTimeDelta = GetUnScaledTimeDelta;
        }
        else
        {
            GetTimeDelta = GetScaledTimeDelta;
        }
    }
    public static float GetScaledTimeDelta()
    {
        return Time.deltaTime;
    }
    public static float GetUnScaledTimeDelta()
    { 
        return Time.unscaledDeltaTime; 
    }

    public Func<float, float> GetEaseFuncByType(EaseType type, float time)
    {
        if (type == EaseType.EaseOutQuad)
        {
            return EaseOutQuad;
        }
        else if (type == EaseType.EaseInQuad)
        {
            return EaseInQuad;
        }
        else if (type == EaseType.EaseInCubic)
        {
            return EaseInCubic;
        }
        else if (type == EaseType.EaseOutBounce)
        {
            return EaseOutBounce;
        }
        else if (type == EaseType.EaseOutElastic)
        {
            return EaseOutElastic;
        }
        else if (type == EaseType.EaseInOutBack)
        {
            return EaseInOutBack;
        }
        else if (type == EaseType.EaseInOutCubic)
        {
            return EaseInOutCubic;
        }
        else if (type == EaseType.EaseOutBack)
        {
            return EaseOutBack;
        }
        else if (type == EaseType.EaseInBack)
        {
            return EaseInBack;
        }
        else if (type == EaseType.M3Spring)
        {
            if (time <= 0.35f)
            {
                return M3SpringFast;
            }
            else if (time < 0.65f)
            {
                return M3SpringDefault;
            }
            else
            {
                return M3SpringSlow;
            }
        }
        else if (type == EaseType.PunchDefault)
        {
            return PunchDefault;
        }
        else if (type == EaseType.PunchToTarget)
        {
            return PunchToTarget;
        }
        else if (type == EaseType.ShakeDefault)
        {
            return ShakeDefault;
        }
        else if (type == EaseType.ShakeToTarget)
        {
            return ShakeToTarget;
        }
        else if (type == EaseType.VibrationCustom)
        {
            return VibrationCustom;
        }
        else
        {
            return EaseOutQuad; //Ĭ��
        }
    }

    public Func<float, Vector3> GetVector3ShakeFuncByType(EaseType type)
    {
        if (type == EaseType.ShakeDefault)
        {
            return V3ShakeDefault;
        }
        else if (type == EaseType.ShakeToTarget)
        {
            return V3ShakeToTarget;
        }
        else if (type == EaseType.VibrationCustom)
        {
            return V3VibrationCustom;
        }
        else
        {
            Debug.Log("�˺�����Ӧ��ִ�е��ˡ�");
            return V3ShakeDefault;
        }
    }
    public bool IsToTarget(EaseType type)
    {
        if (type == EaseType.PunchDefault || type == EaseType.ShakeDefault)
        {
            return false;
        }
        else if(type == EaseType.VibrationCustom)
        {
            if(vibrationData.IsToTarget == false)
            {
                    return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }
    public bool IsRandomShake(EaseType type)
    {
        if(type == EaseType.ShakeDefault || type == EaseType.ShakeToTarget)
        {
            return true;
        }else if(type == EaseType.VibrationCustom)
        {
            if(vibrationData.IsRandom == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static float EaseOutQuad(float x)
    {
        return 1 - Mathf.Pow(1 - x, 2);
    }
    public static float EaseInQuad(float x)
    {
        return x * x;
    }
    public static float EaseInCubic(float x)
    {
        return x * x * x;
    }
    public static float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;
        if (x < 1 / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }
        else
        {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }

    public static float EaseOutElastic(float x)
    {
        float c4 = 2 * MathF.PI * x;

        if (x == 0)
        {
            return 0;
        }
        else if (x == 1)
        {
            return 1;
        }
        else
        {
            return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }
    }

    public static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
    public static float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3* Mathf.Pow(x - 1, 3) + c1* Mathf.Pow(x - 1, 2);
    }
    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return c3 * Mathf.Pow(x , 3) - c1 * Mathf.Pow(x , 2);
    }
    /// <summary>
    /// //����https://m3.material.io/styles/motion/overview/specs 
    /// bezier ����Ϊ��0.42, 1.67, 0.21, 0.90. Duration =  350ms
    /// ��Ϊ������Ͻ������ʵ�ڲ�֪��spring��ԭ��
    /// </summary>
    public static float M3SpringFast(float x) 
    {
        if(x < 0.313f)
        {
            return -17.62056f * Mathf.Pow(x,3) + 4.68289f * Mathf.Pow(x,2) + 3.61229f * x + 0.00547024f;
        }else if( x < 0.5f)
        {
            return -67.31153f * Mathf.Pow(x,4) + 132.13945f * Mathf.Pow(x, 3) - 97.58087f * Mathf.Pow(x, 2) + 31.75381f * x - 2.73107f;
        }
        else
        {
            return -0.628211f * Mathf.Pow(x, 4) + +1.76657f * Mathf.Pow(x, 3) - 1.27041f * Mathf.Pow(x, 2) - 0.130715f * x + 1.2626f;
        }
    }
    /// <summary>
    /// //����https://m3.material.io/styles/motion/overview/specs 
    /// bezier ����Ϊ��	0.38, 1.21, 0.22, 1.00. Duration =  500ms
    /// ��Ϊ������Ͻ������ʵ�ڲ�֪��spring��ԭ��
    /// </summary>
    public static float M3SpringDefault(float x)
    {
        if (x < 0.4f)
        {
            return -12.86932f * Mathf.Pow(x, 3) + 3.81265f * Mathf.Pow(x, 2) + 2.97105f * x + 0.00245274f;
        }
        else if (x < 0.7f)
        {
            return -8.35137f * Mathf.Pow(x, 4) + 21.2143f * Mathf.Pow(x, 3) - 20.33852f * Mathf.Pow(x, 2) + 8.68378f * x - 0.375128f;
        }
        else
        {
            return 0.0969583f * Mathf.Pow(x, 3) - 0.156501f * Mathf.Pow(x, 2) + 0.0233681f * x + 1.03619f;
        }
    }
    /// <summary>
    /// //����https://m3.material.io/styles/motion/overview/specs 
    /// bezier ����Ϊ��	0.39, 1.29, 0.35, 0.98. Duration =  650ms
    /// ��Ϊ������Ͻ������ʵ�ڲ�֪��spring��ԭ��
    /// </summary>
    public static float M3SpringSlow(float x)
    {
        if (x < 0.6f)
        {
            return 15.01864f * Mathf.Pow(x, 4) -18.84855f * Mathf.Pow(x, 3) + 3.77053f * Mathf.Pow(x,2) + 2.96921f *x + 0.0055831f;
        }
        else
        {
            return -1.09108f * Mathf.Pow(x, 4) + 3.88852f * Mathf.Pow(x, 3) - 4.95606f * Mathf.Pow(x, 2) + 2.63687f * x + 0.521703f;
        }
    }

    public static float PunchDefault(float X)
    {
        int frequency = 6;
        float dampingRatio = 3f;
        float angularFrequency = frequency * Mathf.PI; //��ԭ�㿪ʼ
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Sin(angularFrequency * X);
        return result;
    }
    public static float PunchToTarget(float X)
    {
        float frequency = 6.5f;
        float dampingRatio = 3f;
        float angularFrequency = frequency * Mathf.PI; //��ԭ�㿪ʼ
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = 1 + Mathf.Pow(math.E, -dampingFactor * X) *  Mathf.Cos(Mathf.PI + angularFrequency * X);
        return result;
    }
    public static Vector3 V3ShakeDefault(float X)
    {
        Vector3 v3 = new Vector3(ShakeDefault(X,VibrationCustomData.DefaultRandomSeed),
            ShakeDefault(X, VibrationCustomData.DefaultRandomSeed+1f), 
            ShakeDefault(X, VibrationCustomData.DefaultRandomSeed+2f));
        return v3;
    }
    public static float ShakeDefault(float X)
    {
        return ShakeDefault(X, VibrationCustomData.DefaultRandomSeed);
    }
    public static float ShakeDefault(float X , float RandomSeed)
    {
        int frequency = 4;
        float dampingRatio = 5f;
        //��int)������ȡ��
        float frequencyNumber = (int)(X * frequency);
        float angularFrequency = frequency * Mathf.PI; //��ԭ�㿪ʼ
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);  
        float result = Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Sin(angularFrequency * X);
        float nextRandom = 2 * Mathf.PerlinNoise(RandomSeed, frequencyNumber * 0.2f); //����0-1�����
        result = result * nextRandom;
        return result;
    }
    public static Vector3 V3ShakeToTarget(float X)
    {
        Vector3 v3 = new Vector3(ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed), 
            ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed+1f), 
            ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed+2f));
        return v3;
    }
    public static float ShakeToTarget(float X)
    {
        return ShakeToTarget(X, VibrationCustomData.DefaultRandomSeed);
    }
    public static float ShakeToTarget(float X, float RandomSeed)
    {
        float frequency = 4.5f;
        float dampingRatio = 5f;
        //round�Ὣ0.4999������Ϊ0����0.50001��1.49999��Ϊ1������Cos����
        float frequencyNumber = Mathf.Round(X * frequency);
        float angularFrequency = frequency * Mathf.PI; //��ԭ�㿪ʼ
        float dampingFactor = dampingRatio * frequency / (2f * Mathf.PI);
        float result = Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Cos(Mathf.PI + angularFrequency * X);

        float nextRandom = 1;
        if(frequencyNumber > 0)
        {
            nextRandom = 2 * Mathf.PerlinNoise(RandomSeed, frequencyNumber * 0.2f); //����0-1�����
        }
        result = 1 + result * nextRandom;
        return result;
    }
    public Vector3 V3VibrationCustom(float X)
    {
        Vector3 v3 = new(VibrationCustom(X, 0), VibrationCustom(X, 1f), VibrationCustom(X, 2f));
        return v3;
    }
    public float VibrationCustom(float X)
    {
        return vibrationData.VibrationCustom(X);
    }
    public float VibrationCustom(float X, float SeedPlus)
    {
        return vibrationData.VibrationCustom(X, SeedPlus);
    }


    public VibrationCustomData GetVibrationData()
    {
        return vibrationData;
    }
    /// <summary>
    /// ������mathf��lerpAngle��ȡ����t��clamp01����
    /// </summary>
    public static float LerpAngleUnclamped(float a, float b, float t)
    {
        float num = Mathf.Repeat(b - a, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return a + num * t;
    }
}

public class VibrationCustomData
{
    public bool IsRandom = false;
    public bool IsToTarget = false;
    /// <summary>
    /// Frequency����dampingRation����20�������
    /// </summary>
    public float Frequency = 6;
    private float angularFrequency;
    public float DampingRatio = 3f;
    private float dampingFactor;
    /// <summary>
    /// ����������
    /// </summary>
    public static float DefaultRandomSeed = 1.2f;
    public float RandomSeed = DefaultRandomSeed;
    protected Func<float, float> ResultFunc;
    public VibrationCustomData(int Frequency = 6, float DampingRatio = 3f, bool IsRandomSeed = false, bool isRandom = false, bool isToTarget = false)
    {
        Set(Frequency, DampingRatio, IsRandomSeed, isRandom, isToTarget);
    }

    public void Set(int frequency = 6, float dampingRatio = 3f, bool IsRandomSeed = false, bool isRandom = false, bool isToTarget = false)
    {
        if (IsRandomSeed == true)
        {
            SetNewRandomSeed();
        }
        this.Frequency = frequency;
        this.DampingRatio = dampingRatio;
        IsRandom = isRandom;
        IsToTarget = isToTarget;

        angularFrequency = this.Frequency * Mathf.PI; //��ԭ�㿪ʼ
        dampingFactor = this.DampingRatio * this.Frequency / (2f * Mathf.PI);
        if (IsToTarget == true)
        {
            this.Frequency += 0.5f;
            ResultFunc = ToTargetResult;
        }
        else
        {
            ResultFunc = UnToTargetResult;
        }
    }

    public void SetNewRandomSeed()
    {
        RandomSeed = UnityEngine.Random.Range(1f, 100f);
        while (RandomSeed % 1 == 0)
        {
            RandomSeed = UnityEngine.Random.Range(1f, 100f);
        }
    }
    public float UnToTargetResult(float X)
    {
        return Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Sin(angularFrequency * X);
    }
    public float ToTargetResult(float X)
    {
        return Mathf.Pow(math.E, -dampingFactor * X) * Mathf.Cos(Mathf.PI + angularFrequency * X);
    }
    public float VibrationCustom(float X, float SeedPlus = 0)
    {
        float result = ResultFunc(X);

        if (IsRandom == true)
        {
            float frequencyNumber;
            if (IsToTarget == true)
            {
                //round�Ὣ0.4999������Ϊ0����0.50001��1.49999��Ϊ1
                frequencyNumber = MathF.Round(X * Frequency);
            }
            else
            {
                frequencyNumber = (int)(X * Frequency);
            }
            if (IsToTarget == false || frequencyNumber != 0)
            {
                float realRandomSeed = RandomSeed + SeedPlus;
                float nextRandom = 2 * Mathf.PerlinNoise(realRandomSeed, frequencyNumber * 0.2f); //����0-1�����
                result *= nextRandom;
            }
        }
        if (IsToTarget == true)
        {
            result += 1;
        }
        return result;
    }
}


public enum EaseType
{
    EaseOutQuad,
    EaseInQuad,
    EaseInCubic,
    /// <summary>
    /// ��������û���ù�
    /// </summary>
    EaseOutBounce,
    /// <summary>
    /// �������ԣ���û���ù�
    /// </summary>
    EaseOutElastic,
    /// <summary>
    /// �л�����˶�������ʹ��
    /// </summary>
    EaseInOutBack,
    /// <summary>
    /// ���뻺�����η�����û���ù�
    /// </summary>
    EaseInOutCubic,
    /// <summary>
    /// �����0.55���ҵ���1
    /// </summary>
    EaseOutBack,
    EaseInBack,
    /// <summary>
    /// ����ʱ���Ϊ����ease Line 350ms, 500ms, 650ms
    /// С��350ʱ����һ�ε���1Ϊ0.28 ��100ms
    /// 500msʱ����һ�ε���Ϊ0.43 �� 210ms
    /// 650msʱ����һ�ε���Ϊ0.45 �� 300ms
    /// </summary>
    M3Spring,
    /// <summary>
    /// Default������ԭ���𶯣�toTarget��ʾ������ֵ��Ŀ��
    /// </summary>
    PunchDefault,
    PunchToTarget,
    /// <summary>
    /// Punch������ȹ̶���Shake����ÿ������������
    /// </summary>
    ShakeDefault,
    ShakeToTarget,
    VibrationCustom,
}