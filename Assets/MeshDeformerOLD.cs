using DecalSystem;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter))]

public class MeshDeformerOLD : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] meshvertices;
    GameObject decal;

    [Header("Deforming settings")]
    [Tooltip("Radius of mesh deformation")]
    public float radius = 1f;
    [Tooltip("Multuply deplacement value")]
    public float multiply = 1f;
    [Tooltip("Set the object tag that can make effect to collision. If lenght of this var = 0, all collisions will count")]
    public string[] tags;
    [Tooltip("Updating collider mesh to deplaced mesh. Mesh collider required")]
    public bool UpdateCollider = false;

    [Header("Decal system")]
    [Tooltip("Select layermask that will affect on the script. All selected layers will affect on sprite of decal system.")]
    public LayerMask layermask;
    [Tooltip("Material that must contain needed sprite.")]
    public Material material;
    [Tooltip("Sprite that will be used placed on mesh")]
    public Sprite sprite;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        meshvertices = deformingMesh.vertices;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 direction = new Vector3();

        if (tags.Length == 0 || Array.IndexOf(tags, collision.gameObject.tag) != -1)
        {
            int count = 0;
            for (int i = 0; i < meshvertices.Length; i++)
            {
                for (int j = 0; j < collision.contacts.Length; j++)
                {
                    Vector3 point = collision.contacts[j].point;
                    Vector3 point2 = transform.TransformPoint(meshvertices[i]);
                    Debug.DrawLine(point, point2, Color.green, 10f);
                    direction = (transform.position - point2).normalized;
                    float distance = Vector3.Distance(point, transform.TransformPoint(meshvertices[i]));
                    if (distance < radius)
                    {
                        count++;
                        Vector3 deformate = point2 + direction * (radius - distance) * multiply;
                        meshvertices[i] = transform.InverseTransformPoint(deformate);
                    }
                }
            }
            deformingMesh.vertices = meshvertices;
            deformingMesh.RecalculateNormals();
            Debug.Log(count);

            decal = new GameObject("Decal");
            decal.transform.parent = transform;
            decal.transform.position = collision.contacts[0].point;
            decal.transform.localScale = decal.transform.localScale * radius;
            decal.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            //decal.transform.rotation = new Quaternion(90,90,90,0);
            Decal decalscript = decal.AddComponent<Decal>();
            decalscript.Material = material;
            decalscript.Sprite = sprite;
            decalscript.MaxAngle = 180f;
            decalscript.Offset = 0.05f;
            decalscript.LayerMask = layermask;
            decalscript.BuildAndSetDirty();
        }
        //Updating mesh collider
        //GetComponent<MeshCollider>().sharedMesh = deformingMesh;
    }
}
