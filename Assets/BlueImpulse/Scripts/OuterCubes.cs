﻿using UnityEngine;
using System.Collections;

public class OuterCubes : CubeRoutine
{
    [System.Serializable]
    public struct IMD
    {
        public Vector3 axis1;
        public Vector3 axis2;
        public float rot_speed1;
        public float rot_speed2;
        public float rot1;
        public float rot2;
        public float speed;
        public float base_y;
    }
    public float m_time;
    public float m_rotation;
    float m_prev_time;
    Vector3 m_axis;
    public IMD[] m_imd;

    float R(float r = 1.0f)
    {
        return Random.Range(-r, r);
    }

    Vector3 RV()
    {
        return new Vector3(R(), R(), R()).normalized;
    }


    public override void Generate()
    {
        const int c1 = 200;
        m_instances = new BatchCubeRenderer.InstanceData[c1];
        m_imd = new IMD[c1];

        Quaternion r = Quaternion.AngleAxis(8.0f, Vector3.forward);
        m_axis = r * Vector3.up;

        for (int i = 0; i < c1; ++i)
        {
            m_imd[i].axis1 = RV();
            m_imd[i].axis2 = RV();
            m_imd[i].rot_speed1 = Random.Range(0.25f, 1.5f) * 20.0f;
            m_imd[i].rot_speed2 = Random.Range(0.25f, 1.5f) * 20.0f;
            m_imd[i].rot1 = R(360.0f);
            m_imd[i].rot2 = R(360.0f);
            m_imd[i].speed = R(1.0f)+4.0f;
            m_imd[i].base_y = 1.5f + R(1.25f);
            m_instances[i].scale = 0.75f + R(0.1f);

            m_instances[i].translation = r * new Vector3(R(), 0.0f, R()).normalized * (12.0f + R(1.25f));
            m_instances[i].translation.y += m_imd[i].base_y;
        }
    }

    public override void Update()
    {
        float dt = m_time - m_prev_time;
        m_prev_time = m_time;

        Quaternion ar = Quaternion.AngleAxis(m_rotation*0.1f*dt, Vector3.forward);
        m_axis = ar * m_axis;

        for (int i = 0; i < m_instances.Length; ++i)
        {
            m_imd[i].rot1 += m_imd[i].rot_speed1 * m_rotation * dt;
            m_imd[i].rot2 += m_imd[i].rot_speed2 * m_rotation * dt;
            Quaternion r = Quaternion.AngleAxis(m_imd[i].rot1, m_imd[i].axis1) * Quaternion.AngleAxis(m_imd[i].rot2, m_imd[i].axis2);
            m_instances[i].rotation = r;
            m_instances[i].translation.y -= m_imd[i].base_y;
            m_instances[i].translation = Quaternion.AngleAxis(m_rotation * dt * m_imd[i].speed, m_axis) * m_instances[i].translation;
            m_instances[i].translation.y += m_imd[i].base_y;
        }
        base.Update();
    }
}
