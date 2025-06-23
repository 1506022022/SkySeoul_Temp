using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Battle
{
    public class Barricade : CharacterComponent, IEnemy
    {
        HitBoxComponent body;
        Transform model;
        public string ModelPath;

        public SkillComponent exitSkill;
        public Vector3 SkillOffset;
        public Vector3 SkillRotation;

        public override void Initialize()
        {
            base.Initialize();
            saveLocalPosition = transform.localPosition;
            model = transform.Find("Model");
            body = model?.GetComponent<HitBoxComponent>();
            if (model == null || body == null) return;
            for (int i = 0; i < model.childCount; i++)
            {
                Destroy(model.GetChild(i).gameObject);
            }
            var prefab = Resources.Load<GameObject>(ModelPath);
            if (prefab == null) GameObject.CreatePrimitive(PrimitiveType.Cube).transform.SetParent(model);
            else
            {
                GameObject.Instantiate(prefab, model);
            }
            ResetColliderToFitChildren();
        }
        public void ResetColliderToFitChildren()
        {
            BoxCollider collider = model.GetComponent<BoxCollider>();
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogWarning("�ڽĿ� Renderer�� �����ϴ�.");
                return;
            }

            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 center = combinedBounds.center - transform.position;
            collider.center = transform.InverseTransformVector(center);
            collider.size = transform.InverseTransformVector(combinedBounds.size);
        }
        protected override void OnHit()
        {
            base.OnHit();
            StopAllCoroutines();
            transform.localPosition = saveLocalPosition;
            duration = 0.5f;
            magnitude = 0.05f;
            StartCoroutine(HitAnim());
        }
        protected override void OnDie()
        {
            base.OnDie();
            StopAllCoroutines();
            transform.localPosition = saveLocalPosition;
            duration = 1f;
            magnitude = 0.2f;
            StartCoroutine(HitAnim());

            if (exitSkill == null) return;
            if (!IsPrefabInstance(exitSkill.gameObject)) exitSkill = GameObject.Instantiate(exitSkill);
            exitSkill.transform.position = transform.position + SkillOffset;
            exitSkill.transform.eulerAngles = transform.eulerAngles + SkillRotation;
            exitSkill.Fire();
        }

        bool IsPrefabInstance(GameObject obj)
        {
            return PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected ||
                   PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Disconnected;
        }

        float duration = 0.5f;
        float magnitude = 0.05f;
        Vector3 saveLocalPosition;
        IEnumerator HitAnim()
        {
            saveLocalPosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float offsetX = Random.Range(-1f, 1f) * magnitude;
                float offsetY = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = saveLocalPosition + new Vector3(offsetX, offsetY, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = saveLocalPosition;
        }
    }

}
