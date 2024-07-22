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
        [SerializeField] private float _playerRotationSensitive = 1.0f; //���x
        [SerializeField] private float _playerSpeed = 1.0f; //�X�s�[�h

        private float _yVelocity = 0f;

        void Start()
        {
            //�v���C���[��]�i���E�j
            this.UpdateAsObservable()
                .Select(_ => Input.GetAxis("Mouse X") * _playerRotationSensitive)
                .Subscribe(x => transform.Rotate(0, x, 0));

            //�v���C���[�ړ�
            this.UpdateAsObservable()
                .Select(_ => UseGravity()) //�d��
                .Select(x => new Vector3(Input.GetAxis("Horizontal"), x, Input.GetAxis("Vertical")) * _playerSpeed)
                .Select(x => transform.TransformDirection(x))
                .Subscribe(x => _characterController.Move(x * Time.deltaTime));
        }

        //�d��
        private float UseGravity()
        {
            if (_characterController.isGrounded) _yVelocity = 0f; //�n��
            else _yVelocity -= _gravity * Time.deltaTime; //��

            return _yVelocity;
        }
    }
}
