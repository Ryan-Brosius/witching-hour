using System.Collections.Generic;
using UnityEngine;

// White outline built from copies of this object's sprites and meshes.
// Added automatically by Interactable the first time it's highlighted, no setup needed.
public class InteractOutline : MonoBehaviour
{
    readonly List<GameObject> parts = new List<GameObject>();
    readonly List<SpriteRenderer> sources = new List<SpriteRenderer>();
    readonly List<SpriteRenderer> copies = new List<SpriteRenderer>();
    bool built;

    public void Show(bool show, Material baseMaterial, float width)
    {
        if (show && !built) Build(baseMaterial, width);
        foreach (GameObject part in parts) part.SetActive(show);
        enabled = show; // only sync sprites while visible
    }

    void Build(Material baseMaterial, float width)
    {
        built = true;

        Material spriteMat = new Material(baseMaterial);
        spriteMat.SetFloat("_Cull", 0f);  // sprites: draw both sides, no inflating
        spriteMat.SetFloat("_Width", 0f);

        Material meshMat = new Material(baseMaterial);
        meshMat.SetFloat("_Cull", 1f);    // meshes: back faces only, so just the edge shows
        meshMat.SetFloat("_Width", width);

        // Sprites: 8 white copies nudged around the original, drawn just behind it.
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true))
        {
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI / 4f;
                Vector3 offset = (sr.transform.right * Mathf.Cos(a) + sr.transform.up * Mathf.Sin(a)) * width;

                SpriteRenderer copy = new GameObject("Outline").AddComponent<SpriteRenderer>();
                copy.transform.SetParent(sr.transform, false);
                copy.transform.position = sr.transform.position + offset;
                copy.sharedMaterial = spriteMat;
                copy.sortingLayerID = sr.sortingLayerID;
                copy.sortingOrder = sr.sortingOrder - 1;

                parts.Add(copy.gameObject);
                sources.Add(sr);
                copies.Add(copy);
            }
        }

        // Meshes: a slightly inflated copy showing only its back faces ("inverted hull").
        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>(true))
        {
            if (mf.sharedMesh == null) continue;

            GameObject copy = new GameObject("Outline");
            copy.transform.SetParent(mf.transform, false);
            copy.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
            copy.AddComponent<MeshRenderer>().sharedMaterial = meshMat;
            parts.Add(copy);
        }
    }

    // Keep the sprite copies matching if the sprite changes (lamp on/off, animation, flip).
    void LateUpdate()
    {
        for (int i = 0; i < copies.Count; i++)
        {
            copies[i].sprite = sources[i].sprite;
            copies[i].flipX = sources[i].flipX;
            copies[i].flipY = sources[i].flipY;
        }
    }
}
