using System.Collections.Generic;
using UnityEngine;

namespace Ehaqui.Gameplay
{
    public class TreasurePool : MonoBehaviour
    {
        public static TreasurePool Instance { get; private set; }

        [Header("Prefabs")]
        public GameObject TreasureChestPrefab;
        public GameObject TreasureEffectPrefab;

        [Header("Pool Size")]
        public int InitialSize = 10;

        private Queue<GameObject> _chestPool = new();
        private Queue<GameObject> _effectPool = new();

        private void Awake()
        {
            Instance = this;
            PrewarmPools();
        }

        private void PrewarmPools()
        {
            for (int i = 0; i < InitialSize; i++)
            {
                var chest = CreatePooledObject(TreasureChestPrefab);
                _chestPool.Enqueue(chest);

                var effect = CreatePooledObject(TreasureEffectPrefab);
                _effectPool.Enqueue(effect);
            }
        }

        private GameObject CreatePooledObject(GameObject prefab)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            return obj;
        }

        public GameObject GetChest()
        {
            if (_chestPool.Count > 0)
            {
                var chest = _chestPool.Dequeue();
                chest.SetActive(true);
                return chest;
            }
            var newChest = CreatePooledObject(TreasureChestPrefab);
            newChest.SetActive(true);
            return newChest;
        }

        public void ReturnChest(GameObject chest)
        {
            chest.SetActive(false);
            _chestPool.Enqueue(chest);
        }

        public GameObject GetEffect()
        {
            if (_effectPool.Count > 0)
            {
                var effect = _effectPool.Dequeue();
                effect.SetActive(true);
                return effect;
            }
            var newEffect = CreatePooledObject(TreasureEffectPrefab);
            newEffect.SetActive(true);
            return newEffect;
        }

        public void ReturnEffect(GameObject effect)
        {
            effect.SetActive(false);
            _effectPool.Enqueue(effect);
        }
    }
}
