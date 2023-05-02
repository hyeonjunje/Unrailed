# Unrailed
Unity3d를 이용한 Unrailed 모작입니다.


## Coding Convention

private, protected 변수 맨 앞에 _붙이고 카멜표기법  private Animator _animator
public 변수 그냥 카멜표기법 public Animator animator
프로퍼티 파스칼표기법 public int PlayerHp {get; set;}

interface 앞에 I 붙이기 IDamagable
Enum 앞에 E붙이기 EState
Coroutine 앞에 Co 붙이기 private IEnumerator FireCo()
bool 변수 is이나 can 붙이기 isDead, canJump, isJump...

함수 이름은 동사로 시작, 맨 처음 글자는 대문자 public void Fire, public void GenerateEnemy...
Constants 쓸거면 다 대문자 public const int MINVALUE = 4;
