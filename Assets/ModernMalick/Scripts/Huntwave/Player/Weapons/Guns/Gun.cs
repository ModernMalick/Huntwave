using Core.LeanTween;
using ModernMalick.Core.Patterns;
using ModernMalick.Huntwave.Components.Health;
using ModernMalick.Huntwave.Components.Timing;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace ModernMalick.Huntwave.Player.Weapons.Guns
{
    public class Gun : AWeapon
    {
        [Header("Gun")]
        [SerializeField] protected Transform muzzle;
        [SerializeField] private bool isAutomatic;
        
        [Header("Pellets")]
        [SerializeField] private ObjectFactory pelletFactory;
        [SerializeField] private int pelletCount = 1;
        [SerializeField] private float pelletsMaxSpreadAngle;
        [SerializeField] private float pelletSpeed = 100;
        
        [Header("Magazine")]
        [SerializeField] private AmmoData ammoData;
        [SerializeField] private int magazineSize = 10;
        [SerializeField] private bool infiniteMagazine;
        [SerializeField] private Cooldown reloadCooldown;
        
        [Header("Gun Events")]
        [Space(10)]
        public UnityEvent<int> onMagazineAmmoChanged;
        
        public bool IsAutomatic => isAutomatic;
        
        private int _currentMagazine;
        public int CurrentMagazine
        {
            get => _currentMagazine;
            set
            {
                _currentMagazine = value;
                onMagazineAmmoChanged.Invoke(value);
            }
        }
        
        private bool _isReloading;

        private void Start()
        {
            CurrentMagazine = magazineSize;
            reloadCooldown.Refill();
            reloadCooldown.onReady.AddListener(CompleteReload);
        }

        private void Update()
        {
            if (!_isReloading) return;
            reloadCooldown.Tick(Time.deltaTime);
        }
        
        public override void TryAttack()
        {
            if (_isReloading || !CanAttack()) return;

            if (CurrentMagazine <= 0 && !infiniteMagazine)
            {
                TryReload();
                return;
            }

            if (!infiniteMagazine)
            {
                CurrentMagazine--;
            }
            
            base.TryAttack();
        }
        
        protected override void ExecuteAttack()
        {
            for (var i = 0; i < pelletCount; i++)
            {                
                var spread = Random.insideUnitCircle * Mathf.Tan(pelletsMaxSpreadAngle * Mathf.Deg2Rad);
                var direction = (playerCamera.transform.forward +
                                 playerCamera.transform.right * spread.x +
                                 playerCamera.transform.up * spread.y).normalized;

                var rayOrigin = playerCamera.transform.position;
                var start = muzzle.position;
                var end = rayOrigin + direction * range;
                
                if (Physics.Raycast(rayOrigin, direction, out var hit, range, mask))
                {
                    end = hit.point;
                }

                var pellet = pelletFactory.Get();
                pellet.transform.position = start;
                pellet.transform.rotation = Quaternion.LookRotation(end - start);
            
                LeanTween.move(pellet.gameObject, end, 0.1f)
                    .setSpeed(pelletSpeed)
                    .setOnComplete(() =>
                    {
                        if(hit.collider != null)
                        { 
                            onHit.Invoke(hit);
                            Health.TryModifyHealth(hit.collider.gameObject, -currentDamage);
                        }
                        pelletFactory.Release(pellet);
                    });
            }
            
            base.ExecuteAttack();
        }
        
        public void TryReload()
        {
            if (_isReloading || CurrentMagazine == magazineSize || !ammoData.HasReserve()) return;
        
            _isReloading = true;
            reloadCooldown.Reset();
        }

        private void CompleteReload()
        {
            if (!_isReloading) return;

            var needed = magazineSize - CurrentMagazine;
            CurrentMagazine += ammoData.PullAmmo(needed);
        
            _isReloading = false;
        }
    }
}
