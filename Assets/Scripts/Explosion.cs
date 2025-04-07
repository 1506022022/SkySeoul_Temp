using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private SphereCollider _range;
    [SerializeField] private string _targetTag;
    [SerializeField] private Vector3 _force;

    [Range(0, 100)][SerializeField] private float _waitTime = 3f;
    [Range(0, 10)][SerializeField] private float _minVelocity = 1f;
    [Range(0.01f, 0.99f)][SerializeField] private float _dampingPower = 0.87f;

    private void Awake()
    {
        if (_range == null)
        {
            _range = GetComponent<SphereCollider>();
        }
        _range.isTrigger = true;
        _range.enabled = false;
    }

    public void DoExplosion()
    {
        StartCoroutine(TriggerExplosion());
    }

    private IEnumerator TriggerExplosion()
    {
        _range.enabled = false;
        _range.enabled = true;
        yield return new WaitForFixedUpdate();
        _range.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(_targetTag) && !other.CompareTag(_targetTag))
        {
            return;
        }

        if (other.TryGetComponent<Rigidbody>(out var rigid))
        {
            if (other.GetComponentInParent<Animator>() is var animator)
            {
                animator.enabled = false;
            }

            rigid.isKinematic = false;
            Vector3 direction = (other.transform.position - transform.position).normalized;
            Vector3 explosionForce = Vector3.Scale(_force * _range.radius, direction);
            rigid.AddForceAtPosition(explosionForce, transform.position, ForceMode.Impulse);
            StartCoroutine(ReturnRigidbodySetting(rigid));
        }
    }

    private IEnumerator ReturnRigidbodySetting(Rigidbody rigid)
    {
        yield return new WaitForSeconds(_waitTime);

        while (_minVelocity < rigid.velocity.magnitude)
        {
            yield return new WaitForFixedUpdate();
            rigid.velocity *= _dampingPower;
            rigid.velocity += Time.fixedDeltaTime * Physics.gravity;
        }

        rigid.isKinematic = true;
    }
}