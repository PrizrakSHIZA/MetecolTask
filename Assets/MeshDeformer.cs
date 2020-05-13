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

    Vector3 point, point2, deformate, avgPoint = new Vector3(0, 0, 0), direction = new Vector3();

    float distance, temp;


    private void Update()
    {
        if (!collided)
            bulletDirection = gameObject.GetComponent<Rigidbody>().velocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collided = true;
        if (collision.gameObject != gameObject)
        {
            deformingMesh = collision.gameObject.GetComponent<MeshFilter>().mesh;
            meshvertices = deformingMesh.vertices;
               
            if (tags.Length == 0 || Array.IndexOf(tags, collision.gameObject.tag) != -1)
            {
                //calculation average coordinates of all contact points
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    avgPoint += collision.contacts[i].point;
                }
                avgPoint = avgPoint / collision.contacts.Length;

                for (int i = 0; i < meshvertices.Length; i++)
                {
                    CalculatePoint(collision, i);
                }
                deformingMesh.vertices = meshvertices;
                deformingMesh.RecalculateNormals();

                //Updating mesh collider
                if(UpdateCollider)
                    collision.gameObject.GetComponent<MeshCollider>().sharedMesh = deformingMesh;

                //adding decal
                PlaceDecal(collision);
            }
        }
    }
    public void CalculatePoint(Collision collision, int i)
    {
        point = avgPoint;
        Debug.DrawLine(new Vector3(0, 0, 0), point, Color.red, 20f);
        point2 = collision.gameObject.transform.TransformPoint(meshvertices[i]);
        direction = (point2 - point).normalized;//bulletDirection;
        distance = (point - collision.gameObject.transform.TransformPoint(meshvertices[i])).sqrMagnitude; //Vector3.Distance(point, collision.gameObject.transform.TransformPoint(meshvertices[i]));
        if (distance < radius * radius)
        {
            //temp = Mathf.Sin((radius - Mathf.Sqrt(distance)) / (radius) * (90 * Mathf.Deg2Rad));
            //deformate = point2 + direction * temp * multiply;

            //deformate = point + direction * radius * multiply; Чому кожну координату окремо коли можна працювати з векторами?
            deformate.x = point.x + direction.x * radius * multiply;
            deformate.y = point.y + direction.y * radius * multiply;
            deformate.z = point.z + direction.z * radius * multiply;
            meshvertices[i] = collision.gameObject.transform.InverseTransformPoint(deformate);
        }
    }

    public void PlaceDecal(Collision collision)
    {
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
