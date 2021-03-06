﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DSShockwaveEntity
{
    public Vector3 position;
    public float gap = -0.5f;
    public float speed = 15.0f;
    public float fade_speed = 2.0f;
    public float opacity = 1.5f;
    public float scale = 1.0f;
    public float time = 0.0f;
    public Vector4 stretch;

    public Matrix4x4 matrix;
    public Vector4 shockwave_params;

    public void Update()
    {
        time += Time.deltaTime;
        scale += speed * Time.deltaTime;
        opacity -= fade_speed * Time.deltaTime;
        shockwave_params.Set(opacity, gap, gap, gap);
        matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * scale);
    }

    public bool IsDead()
    {
        return opacity <= 0.0f;
    }
}

public class DSShockwaveManager : DSEffectBase
{
    public static DSShockwaveManager s_instance;

    public Material m_material;
    public Mesh m_mesh;
    public List<DSShockwaveEntity> m_entities = new List<DSShockwaveEntity>();
    int m_i_shockwave_params;
    int m_i_stretch_params;
    Action m_render;


    public static void AddEntity(DSShockwaveEntity e)
    {
        if (!s_instance.enabled) return;
        s_instance.m_entities.Add(e);
    }

    public static DSShockwaveEntity AddEntity(
        Vector3 pos, float scale = 5.0f, float speed = 15.0f, float fade_speed = 2.0f, float opacity = 1.5f, float gap = -1.0f)
    {
        if (!s_instance.enabled) return null;
        DSShockwaveEntity e = new DSShockwaveEntity
        {
            position = pos,
            gap = gap,
            speed = speed,
            fade_speed = fade_speed,
            opacity = opacity,
            scale = scale,
        };
        s_instance.m_entities.Add(e);
        return e;
    }


    void OnEnable()
    {
        ResetDSRenderer();
        s_instance = this;
        if (m_render == null)
        {
            m_render = Render;
            GetDSRenderer().AddCallbackPostEffect(m_render, 5000);
            m_i_shockwave_params = Shader.PropertyToID("shockwave_params");
            m_i_stretch_params = Shader.PropertyToID("stretch_params");
        }
    }

    void OnDisable()
    {
        if (s_instance == this) s_instance = null;
    }

    void Update()
    {
        m_entities.ForEach((a) => { a.Update(); });
        m_entities.RemoveAll((a) => { return a.IsDead(); });
    }

    void Render()
    {
        if (!enabled || m_entities.Count == 0) { return; }
        GetDSRenderer().CopyFramebuffer();
        m_entities.ForEach((a) => {
            m_material.SetVector(m_i_shockwave_params, a.shockwave_params);
            m_material.SetVector(m_i_stretch_params, a.stretch);
            m_material.SetPass(0);
            Graphics.DrawMeshNow(m_mesh, a.matrix);
        });
    }
}

