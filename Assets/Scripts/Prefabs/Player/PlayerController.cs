using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Prefabs.Player
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly float _gravity = 9.81f;

        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [Header("Parameters")]
        [SerializeField] private float _playerRotationSensitive = 1.0f; //感度
        [SerializeField] private float _playerSpeed = 1.0f; //スピード

        private float _yVelocity = 0f;

        void Start()
        {
            //プレイヤー回転（左右）
            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis("Mouse X") * _playerRotationSensitive)
                .Subscribe(x => transform.Rotate(0, x, 0));

            //プレイヤー移動
            this.UpdateAsObservable()
                .Select(_ => UseGravity()) //重力
                .Select(x => new Vector3(Input.GetAxis("Horizontal"), x, Input.GetAxis("Vertical")) * _playerSpeed)
                .Select(x => transform.TransformDirection(x))
                .Subscribe(x => _characterController.Move(x * Time.deltaTime));
        }

        //重力
        private float UseGravity()
        {
            if (_characterController.isGrounded) _yVelocity = 0f; //地面
            else _yVelocity -= _gravity * Time.deltaTime; //空中

            return _yVelocity;
        }
    }
}
