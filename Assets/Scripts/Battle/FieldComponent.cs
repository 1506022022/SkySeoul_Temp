using System;
using UnityEngine;
namespace Battle
{
    public class FieldComponent : MonoBehaviour
    {
        public float _interval = 3;
        public float _t = 0;
        private enum CommanderType { Near, Random, NearStop };
        [SerializeField] private CommanderType type;
        private void ChangeCommander(CommanderType type)
        {
            _commander?.Dispose();
            _commander = type switch
            {
                CommanderType.Near => new NearCommander(FieldRadius,transform.position),
                CommanderType.Random => new RandomCommander(FieldRadius, transform.position),
                CommanderType.NearStop => new NearStopCommander(FieldRadius, transform.position),
                _ => new NearCommander(FieldRadius, transform.position)
            };
        }
        private CommanderType _preType;
        private void Update()
        {
            if (_preType != type)
            {
                ChangeCommander(type);
                _preType = type;
            }
            _t += Time.deltaTime;
            if (_interval < _t)
            {
                _t = 0;
                UpdateCommand();
            }
        }

        [Range(1, 100)] public int FieldRadius = 1;
        private Commander<byte> _commander;

        public void UpdateCommand()
        {
            _commander.UpdateCommand();
        }
        private void Awake()
        {
            ChangeCommander(type);
        }
        private void OnDrawGizmos()
        {
            for (int i = -FieldRadius; i < FieldRadius; i++)
            {
                for (int j = -FieldRadius; j < FieldRadius; j++)
                {
                    var pos = transform.position;
                    pos.x += i;
                    pos.z += j;
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(pos, Vector3.one + Vector3.down * 0.9f);
                }
            }
        }
    }

}
