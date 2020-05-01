using DecalSystem;
using System;
using UnityEngine;
using UnityEngine.Serialization;


public class MeshDeformer : MonoBehaviour
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
    [Tooltip("Additional sprite size")]
    public float AdditionalSize = 0f;

    private bool collided = false;
    private Vector3 bulletDirection;

    private void Update()
    {
        if (!collided)
            bulletDirection = gameObject.GetComponent<Rigidbody>().velocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collided = true;
        Vector3 direction = new Vector3();
        if (collision.gameObject != gameObject)
        {
            deformingMesh = collision.gameObject.GetComponent<MeshFilter>().mesh;
            meshvertices = deformingMesh.vertices;
               
            if (tags.Length == 0 || Array.IndexOf(tags, collision.gameObject.tag) != -1)
            {
                int count = 0;
                Vector3 point, point2, deformate, avgPoint = new Vector3(0,0,0);
                float distance, temp;
                //calculation average coordinates of all contact points
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    avgPoint += collision.contacts[i].point;
                }
                avgPoint = avgPoint / collision.contacts.Length;

                for (int i = 0; i < meshvertices.Length; i++)
                {
                    point = avgPoint;
                    point2 = collision.gameObject.transform.TransformPoint(meshvertices[i]);
                    direction = bulletDirection;
                    distance = Vector3.Distance(point, collision.gameObject.transform.TransformPoint(meshvertices[i]));
                    if (distance < radius)
                    {
                        count++;
                        temp = Mathf.Sin((radius - distance) / radius * (90 * Mathf.PI / 180));
                        deformate = point2 + direction * temp * multiply;
                        meshvertices[i] = collision.gameObject.transform.InverseTransformPoint(deformate);
                    }

                }
                deformingMesh.vertices = meshvertices;
                deformingMesh.RecalculateNormals();
                Debug.Log(count);

                //Updating mesh collider
                if(UpdateCollider)
                    GetComponent<MeshCollider>().sharedMesh = deformingMesh;

                //adding decal
                decal = new GameObject("Decal");
                decal.transform.parent = collision.gameObject.transform;
                decal.transform.position = collision.contacts[0].point;
                decal.transform.localScale = decal.transform.localScale * (radius + AdditionalSize);
                decal.transform.LookAt(collision.gameObject.transform);
                Decal decalscript = decal.AddComponent<Decal>();
                decalscript.Material = material;
                decalscript.Sprite = sprite;
                decalscript.MaxAngle = 180f;
                decalscript.Offset = 0.005f;
                decalscript.LayerMask = layermask;
                decalscript.BuildAndSetDirty();
            }
        }
    }
}
