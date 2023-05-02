# Unrailed
Unity3d를 이용한 Unrailed 모작입니다.


## Coding Convention

+ private, protected 변수 맨 앞에 _붙이고 카멜표기법      ex) private Animator _animator  <br>
+ public 변수 그냥 카멜표기법                            ex) public Animator animator  <br>
+ 프로퍼티 파스칼표기법                                  ex) public int PlayerHp {get; set;}  <br>

+ interface 앞에 I 붙이기                               ex) IDamagable  <br>
+ Enum 앞에 E붙이기                                     ex) EState  <br>
+ Coroutine 앞에 Co 붙이기                              ex) private IEnumerator FireCo()  <br>
+ bool 변수 is이나 can 붙이기                           ex) isDead, canJump, isJump...  <br>

+ 함수 이름은 동사로 시작, 맨 처음 글자는 대문자          ex) public void Fire, public void GenerateEnemy...  <br>
+ Constants 쓸거면 다 대문자                            ex) public const int MINVALUE = 4;  <br>
