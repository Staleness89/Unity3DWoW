
using Foole.Mpq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static M2Texture;
using Material = UnityEngine.Material;

public class M2UnityHelper
{
    public M2 loadedModel;
    public GameObject m2GameObject;
    public List<int> activeGeosets = new List<int>();
    public SkinnedMeshRenderer renderer;
    public Texture2D[] textures;
    public Color[] colors;

    public M2UnityHelper(M2 _model)
    {
        loadedModel = _model;
    }

    public void GenerateGameObject()
    {
        Vector3[] vertices = new Vector3[loadedModel.GlobalVertexList.Count];
        Vector3[] normals = new Vector3[loadedModel.GlobalVertexList.Count];
        BoneWeight[] weights = new BoneWeight[loadedModel.GlobalVertexList.Count];
        Vector2[] uv = new Vector2[loadedModel.GlobalVertexList.Count];
        Vector2[] uv2 = new Vector2[loadedModel.GlobalVertexList.Count];

        GameObject model = new GameObject(loadedModel.Name);
        model.AddComponent<SkinnedMeshRenderer>();
        renderer = model.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = new Mesh();
        mesh.name = loadedModel.Name + "_mesh";

        for (int i = 0; i < loadedModel.GlobalVertexList.Count; i++)
        {
            vertices[i] = new Vector3(-loadedModel.GlobalVertexList[i].Position.X / 2, loadedModel.GlobalVertexList[i].Position.Y / 2, loadedModel.GlobalVertexList[i].Position.Z / 2);
            normals[i] = new Vector3(-loadedModel.GlobalVertexList[i].Normal.X, loadedModel.GlobalVertexList[i].Normal.Y, loadedModel.GlobalVertexList[i].Normal.Z);
            BoneWeight weight = new BoneWeight
            {
                boneIndex0 = loadedModel.GlobalVertexList[i].BoneIndices[0],
                boneIndex1 = loadedModel.GlobalVertexList[i].BoneIndices[1],
                boneIndex2 = loadedModel.GlobalVertexList[i].BoneIndices[2],
                boneIndex3 = loadedModel.GlobalVertexList[i].BoneIndices[3],
                weight0 = loadedModel.GlobalVertexList[i].BoneWeights[0] / 255f,
                weight1 = loadedModel.GlobalVertexList[i].BoneWeights[1] / 255f,
                weight2 = loadedModel.GlobalVertexList[i].BoneWeights[2] / 255f,
                weight3 = loadedModel.GlobalVertexList[i].BoneWeights[3] / 255f
            };
            weights[i] = weight;
            uv[i] = new Vector2(loadedModel.GlobalVertexList[i].TexCoords[0].X, 1 - loadedModel.GlobalVertexList[i].TexCoords[0].Y);
            uv2[i] = new Vector2(loadedModel.GlobalVertexList[i].TexCoords[1].X, 1 - loadedModel.GlobalVertexList[i].TexCoords[1].Y);
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.boneWeights = weights;
        mesh.uv = uv;
        mesh.uv2 = uv2;
        mesh.subMeshCount = loadedModel.Views[0].Submeshes.Count;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = new int[loadedModel.Views[0].Submeshes[i].NTriangles];
            for (int j = 0; j < triangles.Length; j++)
            {
                int g = loadedModel.Views[0].Submeshes[i].StartTriangle + j;
                triangles[j] = loadedModel.Views[0].Triangles[g];
            }
            mesh.SetTriangles(triangles, i);
        }

        Transform[] bones = new Transform[loadedModel.Bones.Count];
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = new GameObject("Bone " + loadedModel.Bones[i].KeyBoneId).transform;
            bones[i].position = new Vector3(-loadedModel.Bones[i].Pivot.X / 2, loadedModel.Bones[i].Pivot.Y / 2, loadedModel.Bones[i].Pivot.Z / 2);
        }

        GameObject skeleton = new GameObject("Skeleton");
        for (int i = 0; i < bones.Length; i++)
        {
            if (loadedModel.Bones[i].ParentBone == -1)
            {
                bones[i].parent = skeleton.transform;
            }
            else
            {
                bones[i].parent = bones[loadedModel.Bones[i].ParentBone];
            }
        }

        Matrix4x4[] bind = new Matrix4x4[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            bind[i] = bones[i].worldToLocalMatrix * model.transform.localToWorldMatrix;
        }

        skeleton.transform.parent = model.transform;
        skeleton.transform.localEulerAngles = new Vector3(-90, 0, 0);
        renderer.materials = new Material[mesh.subMeshCount];
        renderer.sharedMesh = mesh;
        renderer.bones = bones;
        renderer.rootBone = bones[0];
        mesh.bindposes = bind;

        LoadColors();
        textures = new Texture2D[loadedModel.Textures.Count];
        LoadTextures();

        SetMats();
        m2GameObject = model;

        //SetEars();
        //SetHairStyle();
        //SetMustache();
        //SetBeard();
        //SetMats();
    }
    /*public GameObject AddParticleEffect(M2Particle particle)
    {
        //Create gameobject
        GameObject element = new GameObject();
        element.AddComponent<ParticleSystem>();
        ParticleSystem system = element.GetComponent<ParticleSystem>();
        //Setup lifetime and speed in main module
        ParticleSystem.MainModule main = system.main;
        float variation = particle.Lifespan.;
        main.startLifetime = new ParticleSystem.MinMaxCurve((particle.Lifespan - variation) / 2f, (particle.Lifespan + variation) / 2f);
        variation = particle.SpeedVariation * particle.Speed;
        main.startSpeed = new ParticleSystem.MinMaxCurve((particle.Speed - variation) / 2f, (particle.Speed + variation) / 2f);
        //Setup emission rate in emission module
        ParticleSystem.EmissionModule emission = system.emission;
        variation = particle.EmissionVariation * particle.EmissionRate;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve((particle.EmissionRate - variation) / 2f, (particle.EmissionRate + variation) / 2f);
        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(particle.EmissionRate - variation, particle.EmissionRate + variation);
        //Setup shape and scale in shape module
        ParticleSystem.ShapeModule shape = system.shape;
        shape.shapeType = ParticleShape(particle.Type);
        shape.scale = new Vector3(particle.EmissionAreaWidth, particle.EmissionWidth, particle.EmissionLength);
        //Setup color and alpha gradients in color over lifetime module
        ParticleSystem.ColorOverLifetimeModule color = system.colorOverLifetime;
        color.enabled = true;
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[particle.ColorTrack.Values.Count];
        for (int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i] = new GradientColorKey(new Color(particle.ColorTrack.Values[i].X / 255f, particle.ColorTrack.Values[i].Y / 255f, particle.ColorTrack.Values[i].Z / 255f), particle.ColorTrack.Timestamps[i]);
        }
        GradientAlphaKey[] alphaKeys;
        if (particle.Alpha.Values.Length > 8)
        {
            alphaKeys = new GradientAlphaKey[particle.Alpha.Values.Length / 2];
        }
        else
        {
            alphaKeys = new GradientAlphaKey[particle.Alpha.Values.Length];
        }
        for (int i = 0, j = 0; i < alphaKeys.Length; i++, j++)
        {
            if (particle.Alpha.Values.Length > 8)
            {
                j++;
            }
            alphaKeys[i] = new GradientAlphaKey(particle.Alpha.Values[j], particle.Alpha.Timestamps[j]);
        }
        gradient.SetKeys(colorKeys, alphaKeys);
        color.color = gradient;
        //Setup size in size over lifetime module
        ParticleSystem.SizeOverLifetimeModule size = system.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        for (int i = 0; i < particle.Scale.Values.Length; i++)
        {
            curve.AddKey(particle.Scale.Timestamps[i], particle.Scale.Values[i].X);
        }
        size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        //Setup texture sheet in texture sheet animation module
        ParticleSystem.TextureSheetAnimationModule textureSheet = system.textureSheetAnimation;
        textureSheet.enabled = true;
        textureSheet.numTilesX = particle.TileColumns;
        textureSheet.numTilesY = particle.TileRows;
        return element;
    }*/
    public void SetMats()
    {
        for (int i = 0; i < loadedModel.Views[0].TextureUnits.Count; i++)
        {
            if (loadedModel.Views[0].TextureUnits[i].Layer == 0)
            {
                SetMaterial(i);
            }
            else if (loadedModel.Views[0].TextureUnits[i].Layer == 1)
            {
                List<Material> materials = renderer.materials.ToList();
                materials.Add(new Material(materials[0]));
                renderer.materials = materials.ToArray();
                SetLayeredMaterial(i);
            }
        }
    }
    public void LoadColors()
    {
        colors = new Color[loadedModel.Colors.Count];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(loadedModel.Colors[i].Color._defaultValue.X, loadedModel.Colors[i].Color._defaultValue.Y, loadedModel.Colors[i].Color._defaultValue.Z, 1.0f);
        }
    }
    public void SetLayeredMaterial(int i)
    {
        Material material = Resources.Load<Material>($@"Materials\{loadedModel.Views[0].TextureUnits[i].ShaderId}");
        if (material == null)
        {
            Debug.LogError(loadedModel.Views[0].TextureUnits[i].ShaderId);
        }
        int index = renderer.materials.Length - 1;
        renderer.materials[index] = new Material(material.shader);
        renderer.materials[index].shader = material.shader;
        renderer.materials[index].CopyPropertiesFromMaterial(material);
        renderer.materials[index].name = textures[loadedModel.TexLookup[loadedModel.Views[0].TextureUnits[i].Texture]].name;
        //renderer.sharedMesh.OptimizeReorderVertexBuffer();
        SetTexture(renderer.materials[index], i);
    }
    public void SetMaterial(int i)
    {
        Material material = Resources.Load<Material>($@"Materials\{loadedModel.Views[0].TextureUnits[i].ShaderId}");
        if (material == null)
        {
            Debug.LogError(loadedModel.Views[0].TextureUnits[i].ShaderId);
        }
        renderer.materials[loadedModel.Views[0].TextureUnits[i].SubmeshIndex] = new Material(material.shader);
        renderer.materials[loadedModel.Views[0].TextureUnits[i].SubmeshIndex].shader = material.shader;
        renderer.materials[loadedModel.Views[0].TextureUnits[i].SubmeshIndex].CopyPropertiesFromMaterial(material);
        renderer.materials[loadedModel.Views[0].TextureUnits[i].SubmeshIndex].name = textures[loadedModel.TexLookup[loadedModel.Views[0].TextureUnits[i].Texture]].name;
        //renderer.sharedMesh.OptimizeReorderVertexBuffer();
        SetTexture(renderer.materials[loadedModel.Views[0].TextureUnits[i].SubmeshIndex], i);
    }
    public void SetEars(int _int = 702)
    {
        activeGeosets.RemoveAll(x => x > 699 && x < 800);
        activeGeosets.Add(_int);
    }
    public void SetHairStyle(int _int = 1)
    {
        activeGeosets.RemoveAll(x => x > 0 && x < 100);
        activeGeosets.Add(_int);
    }
    public void SetMustache(int _int = 300)
    {
        activeGeosets.RemoveAll(x => x > 299 && x < 400);
        activeGeosets.Add(_int);
    }
    public void SetBeard(int _int = 100)
    {
        activeGeosets.RemoveAll(x => x > 99 && x < 200);
        activeGeosets.Add(_int);
    }
    public void SetEyeColor(int _int = 1700, int _int2 = 3300)
    {
        activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
        activeGeosets.RemoveAll(x => x > 3299 && x < 3400);
        activeGeosets.Add(_int);
        activeGeosets.Add(_int2);
    }
    public void SetEarrings(int _int = 3500)
    {
        activeGeosets.RemoveAll(x => x > 3499 && x < 3600);
        activeGeosets.Add(_int);
    }
    public void LoadTextures()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            M2Texture texture = loadedModel.Textures[i];
            switch (texture.Type)
            {
                case TextureType.None:
                    string fileName = texture.Name;
                    MpqStream f = AppHandler.Instance.SearchMPQ(fileName);

                    Texture2D texture4 = BLPLoader.ToTex(f);
                    if (texture4 == null)
                    {
                        Debug.LogError("Error Loading BLP.");
                    }
                    textures[i] = new Texture2D(texture4.width, texture4.height, TextureFormat.ARGB32, false);
                    textures[i].SetPixels32(texture4.GetPixels32());
                    textures[i].name = fileName;
                    textures[i].wrapModeU = texture.Flags.HasFlag(TextureFlags.WrapX) ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                    textures[i].wrapModeV = texture.Flags.HasFlag(TextureFlags.WrapY) ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                    textures[i].Apply();
                    break;
                case TextureType.Skin:
                case TextureType.ObjectSkin:
                case TextureType.WeaponBlade:
                case TextureType.WeaponHandle:
                case TextureType.Environment:
                case TextureType.CharacterHair:
                case TextureType.CharacterFacialHair:
                case TextureType.SkinExtra:
                case TextureType.UiSkin:
                case TextureType.TaurenMane:
                case TextureType.MonsterSkin1:
                case TextureType.MonsterSkin2:
                case TextureType.MonsterSkin3:
                case TextureType.ItemIcon:
                case TextureType.GuildBackgroundColor:
                case TextureType.GuildEmblemColor:
                case TextureType.GuildBorderColor:
                case TextureType.GuildEmblem:
                    textures[i] = new Texture2D(200, 200);
                    break;
            }
        }
    }
    public void SetTexture(Material material, int i)
    {
        material.SetTexture("_Texture1", textures[loadedModel.TexLookup[loadedModel.Views[0].TextureUnits[i].Texture]]);
        //if (loadedModel.Textures.Skin.Textures[i].TextureCount > 1)
        //{
        //if (material.shader.name == "Custom/32783")
        //{
        //    textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]].wrapMode = TextureWrapMode.Clamp;
        //}
        // material.SetTexture("_Texture2", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]]);
        // }
        //if (helper.Emission == null)
        //{
        //material.SetTexture("_Emission", Texture2D.blackTexture);
        //}
        //else if (Model.Textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]].Type == 1)
        //{
        //    material.SetTexture("_Emission", helper.Emission);
        //}
        //if (Race == 34 && material.shader.name == "Custom/16401")
        //{
        //   material.SetInt("_SrcBlend", (int)BlendMode.SrcColor);
        //    material.SetInt("_DstBlend", (int)BlendMode.One);
        //}
        //else
        //{
        material.SetInt("_SrcBlend", (int)SrcBlend((short)loadedModel.Materials[loadedModel.Views[0].TextureUnits[i].Texture].BlendMode));
        material.SetInt("_DstBlend", (int)DstBlend((short)loadedModel.Materials[loadedModel.Views[0].TextureUnits[i].Texture].BlendMode));
        //}
        material.SetFloat("_AlphaCut", (short)loadedModel.Materials[loadedModel.Views[0].TextureUnits[i].Texture].BlendMode == 1 ? 0.5f : 0f);

        Color color = Color.white;
        if (loadedModel.Views[0].TextureUnits[i].ColorIndex != -1)
        {
            color = colors[loadedModel.Views[0].TextureUnits[i].ColorIndex];
        }
        color.a = loadedModel.Transparencies[loadedModel.TransLookup[loadedModel.Views[0].TextureUnits[i].Transparency]].Weight._defaultValue.Value;
        if (loadedModel.Views[0].TextureUnits[i].Layer > 0)
        {
            color.a *= 0.25f;
        }
        //material.SetColor("_Color", color);
        CullMode cull = ((int)loadedModel.Materials[loadedModel.Views[0].TextureUnits[i].Texture].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Front;
        material.SetInt("_Cull", (int)cull);
        float depth = ((int)loadedModel.Materials[loadedModel.Views[0].TextureUnits[i].Texture].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
    }
    protected BlendMode SrcBlend(short value)
    {
        BlendMode blend = BlendMode.One;
        switch (value)
        {
            case 0:
            case 1:
            case 7:
                blend = BlendMode.One;
                break;
            case 2:
            case 4:
                blend = BlendMode.SrcAlpha;
                break;
            case 3:
                blend = BlendMode.SrcColor;
                break;
            case 5:
            case 6:
                blend = BlendMode.DstColor;
                break;
        }
        return blend;
    }
    protected BlendMode DstBlend(short value)
    {
        BlendMode blend = BlendMode.Zero;
        switch (value)
        {
            case 0:
            case 1:
            case 5:
                blend = BlendMode.Zero;
                break;
            case 2:
            case 7:
                blend = BlendMode.OneMinusSrcAlpha;
                break;
            case 3:
            case 4:
                blend = BlendMode.One;
                break;
            case 6:
                blend = BlendMode.SrcColor;
                break;
        }
        return blend;
    }
}