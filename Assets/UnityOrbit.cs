using OrbitCalculations;
using DV;
using UnityEngine;
using System;

public class UnityOrbit : MonoBehaviour
{
    private Orbit MyOrbit;
    public bool rotate;
    public SolarSceneController controller;
    private double time_offset;
    private double EffectiveTime => controller.TimeSinceStart - time_offset;

    // Start is called before the first frame update
    void Start()
    {
        MyOrbit = new Orbit(); //standard earth like orbit;
        rotate = false;
        time_offset = 0.0;
    }

    // Update is called once per frame
    void Update()
    {
        DVector pos = MyOrbit.GetPosition(EffectiveTime);
        double multiplier = controller.GetMultiplier();
        float newx = (float)(multiplier*pos.X);
        float newy = (float)(multiplier * pos.Y);
        float newz = (float)(multiplier * pos.Z);
        transform.position = new Vector3(newx, newy, newz);

        // also rotate;
        if (rotate)
        {
            double time_h = controller.TimeSinceStart / (60.0 * 60.0);
            double rotate_timer = time_h % 24.0;
            transform.eulerAngles = new Vector3(0.0f, 0.0f, (float)(360.0 * rotate_timer / 24.0));
        }
    }
    private string fN(double x)
    {
        if (Math.Sign(x) >= 0.0) return " " + Math.Abs(x).ToString("E3");
        else return x.ToString("E3");
    }

    public string GetStateString()
    {
        DVector[] sv = MyOrbit.GetStateVector(EffectiveTime);
        double time = controller.TimeSinceStart / (60.0 * 24.0 * 60.0);
        string state_string = "<mspace=0.6em>T:" + fN(time) + " Days X:" + fN(sv[0].X) + " Y:" + fN(sv[0].Y) + " Z:" + fN(sv[0].Z)
        + " vX:" + fN(sv[1].X) + " vY:" + fN(sv[1].Y) + " vZ:" + fN(sv[1].Z) + " v:" + fN(sv[1].Length) + "</mspace>";
        return state_string;
    }

    public void Accelerate(float dt, float APerS)
    {
        var sv = MyOrbit.GetStateVector(EffectiveTime);
        time_offset = controller.TimeSinceStart;
        sv[1] += dt*APerS*sv[1].Normalized;
        Debug.Log(sv[0]);
        Debug.Log(sv[1]);
        MyOrbit = new Orbit(sv[0], sv[1]);
    }

    public void Deaccelerate(float dt, float APerS)
    {
        Accelerate(-1.0f * dt, APerS);
    }

    public void MoveNormal(float dt, float APerS)
    {
        var sv = MyOrbit.GetStateVector(EffectiveTime);
        time_offset = controller.TimeSinceStart;
        sv[1] += dt * APerS * DVector.Cross(sv[0], sv[1]).Normalized;
        Debug.Log(sv[0]);
        Debug.Log(sv[1]);
        MyOrbit = new Orbit(sv[0], sv[1]);
    }

    public void MoveAntiNormal(float dt, float APerS)
    {
        MoveNormal(-1.0f * dt, APerS);
    }
}
