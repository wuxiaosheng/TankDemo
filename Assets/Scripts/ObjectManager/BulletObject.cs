using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    // Start is called before the first frame update
    private float _destroyTime = 3.0f;
    private bool _isNeedRemove = false;
    private float _explosionRadius = 5.0f;
    private float _explosionForce = 1000.0f;
    private float _maxDemage = 10.0f;
    private GameObject _explosionParticles;
    void Start()
    {
        _explosionParticles = GameObject.Instantiate(Resources.Load("Prefabs/ShellExplosion"), transform) as GameObject;
    }

    // Update is called once per frame

    public void addForce(float force, Vector3 forward) {
        Rigidbody rigid = transform.GetComponent<Rigidbody>();
        rigid.velocity = force*forward;
        
    }

    public float calculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (_explosionRadius - explosionDistance) / _explosionRadius;
        float damage = relativeDistance * _maxDemage;
        damage = Mathf.Max (0f, damage);
        return damage;
    }

    public float getExplosionForce() {
        return _explosionForce;
    }

    public float getExplosionRadius() {
        return _explosionRadius;
    }

    public bool isNeedRemove() {
        return _isNeedRemove;
    }

    public void update() {
        _destroyTime -= Time.deltaTime;
        if (_destroyTime <= 0.0f) {
            _isNeedRemove = true;
        }
    }

    public void onRemove() {
        GameObject.Destroy(transform.gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        _isNeedRemove = true;
        _explosionParticles.transform.parent = null;
        _explosionParticles.GetComponent<ParticleSystem>().Play();
        ParticleSystem.MainModule mainModule = _explosionParticles.GetComponent<ParticleSystem>().main;
        Destroy (_explosionParticles.gameObject, mainModule.duration);
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, LayerMask.NameToLayer("Players"));
        for (int i = 0; i < colliders.Length; i++) {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();
            if (colliders[i].gameObject.tag == "Bullet") { continue; }
            if (!targetRigidbody) { continue; }
            targetRigidbody.AddExplosionForce (_explosionForce, transform.forward, _explosionRadius);
            Event evt = EventManager.getInstance().createEvent(EventType.EVT_ON_BULLET_COLLISION, "ColliObject", collision.gameObject);
            evt.addArg("bullet", transform.gameObject);
            EventManager.getInstance().broadcast(evt);
        }
    }
}
