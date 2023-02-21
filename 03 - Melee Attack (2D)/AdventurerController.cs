using UnityEngine;

namespace MelenitasDev
{
	public class AdventurerController : MonoBehaviour
	{
		// ----- Serialized Fields
		[Header("References")]
		[SerializeField] private Transform groundCheck;
		[SerializeField] private LayerMask whatIsGround;
		[SerializeField] private Transform attackArea;

		[Header("Settings")]
		[SerializeField] private float speed;
		[SerializeField] private float jumpForce;
		
		// ----- Fields
		private Rigidbody2D rb;
		private Animator anim;
		
		private float xInput;
		private AdventurerState currentState;
		
		// ----- Enums
		private enum AdventurerState
		{
			Idle,
			Running,
			Jumping,
			Falling,
			Attacking
		}
		
		// ----- Unity Callbacks
		void Awake ()
		{
			// Referenciamos los componentes adjuntos al personaje.
			rb = GetComponent<Rigidbody2D>();
			anim = GetComponent<Animator>();
		}

		void Update ()
		{
			// Llamamos a la función que gira al personaje.
			Flip();

			// Detectamos si se presiona la tecla espacio y si el personaje está tocando el suelo.
			if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
			{
				// Ejecutamos el salto.
				Jump();
			}

			// Detectamos el click izquierdo del ratón.
			if (Input.GetMouseButtonDown(0))
			{
				// Empezamos el ataque.
				StartAttack();
			}

			// Si estamos atacando...
			if (currentState == AdventurerState.Attacking)
			{
				// Detenemos el movimiento.
				rb.velocity = Vector2.zero;
				return;
			}
			
			// Si el jugador no está tocando el suelo, ni saltando, ni tocando el suelo...
			if (xInput == 0 && currentState != AdventurerState.Jumping && IsGrounded())
			{
				// Cambiamos el estado a Idle.
				ChangeState(AdventurerState.Idle);
			}
			// Si está tocando el teclado y tocando el suelo...
			else if (Mathf.Abs(xInput) > 0 && IsGrounded())
			{
				// Cambiamos el estado a Corriendo.
				ChangeState(AdventurerState.Running);
			}
			// Si no está tocando el suelo y su velocidad en Y es negativa...
			else if (!IsGrounded() && rb.velocity.y < -0.5f)
			{
				// Cambiamos el estado a Cayendo.
				ChangeState(AdventurerState.Falling);
			}
		}
		
		void FixedUpdate ()
		{
			// Llamamos al control de movimiento.
			HandleMovement();
		}

		// Este evento se ejecuta cuando se dibujan los gizmos en el editor.
		// Lo usaremos para crear nuestro propio gizmo y poder ver el área de ataque.
		void OnDrawGizmos ()
		{
			// Cambiamos el color del gizmo, en este caso, al azul.
			Gizmos.color = new Color(0, 0.35f, 1, 0.3f);
			// Dibujamos la caja que representa el área de ataque para poder ajustarla mejor.
			Gizmos.DrawCube(attackArea.position, attackArea.localScale);
		}

		// ----- Private Methods
		private void HandleMovement ()
		{
			// Capturamos las teclas ("A" y "D") y ("←" y "→") para conocer la dirección.
			xInput = Input.GetAxisRaw("Horizontal");

			// Si estamos atacando, detenemos la función para que el personaje no se mueva.
			if (currentState == AdventurerState.Attacking) return;
			
			// Creamos un vector para el movimiento horizontal multiplicando la dirección por la velocidad.
			// En el eje vertical mantenemos la velocidad del Rigidbody.
			Vector2 move = new Vector2(xInput * speed, rb.velocity.y);

			// Le pasamos el movimiento a la velocidad del Rigidbody.
			rb.velocity = move;
		}

		private void Flip ()
		{
			// Si el personaje se mueve a la derecha...
			if (xInput > 0 && transform.localScale.x < 0)
			{
				// Ponemos la escala en X en positivo (Recuerda cambiar los unos por la escala de tu personaje).
				transform.localScale = new Vector3(1, 1, 1);
			}
			// Si el personaje se mueve a la izquierda...
			else if (xInput < 0 && transform.localScale.x > 0)
			{
				// Ponemos la escala en X en negativo (Recuerda cambiar los unos por la escala de tu personaje).
				transform.localScale = new Vector3(-1, 1, 1);
			}
		}

		private void Jump ()
		{
			// Frenamos cualquier fuerza vertical (como la gravedad) para que no interfiera con el salto.
			rb.velocity = new Vector2(rb.velocity.x, 0);
			
			// Aplicamos una fuerza hacia arriba con la fuerza del salto que elijamos en el inspector.
			rb.AddForce(Vector2.up * (jumpForce * Time.fixedDeltaTime), ForceMode2D.Impulse);
			
			// Cambiamos el estado a Saltando.
			ChangeState(AdventurerState.Jumping);
		}
		
		private void StartAttack ()
		{
			// Si no estamos tocando el suelo, cancelamos el ataque.
			if (!IsGrounded()) return;
			
			// Cambiamos el estado a Atacando.
			ChangeState(AdventurerState.Attacking);
		}
		
		// Esta función se ejecuta desde la animación de ataque.
		private void Attack ()
		{
			// Creamos una caja en la zona del área de ataque y guardamos todos los colisionadores que
			// detectemos en su interior en el array "colliders".
			Collider2D[] colliders = Physics2D.OverlapBoxAll(attackArea.position, 
				attackArea.localScale, 0);

			// Comprobamos todos los colisionadores que hemos guardado en la línea anterior...
			foreach (Collider2D collider in colliders)
			{
				// Y si alguno de ellos es "Atacable"...
				if (collider.TryGetComponent(out IAttackable attackable))
				{
					// Llamamos a su función Herir.
					attackable.Hurt();
				}
			}
		}
		
		// Esta función se ejecuta desde el último frame de la animación de ataque.
		private void FinishAttack ()
		{
			// Cambiamos el estado a Idle.
			ChangeState(AdventurerState.Idle);
		}

		private bool IsGrounded ()
		{
			// Creamos un rayo desde el objeto colocado en los pies del personaje.
			Ray2D ray = new Ray2D(groundCheck.position, Vector2.down);
			
			// Lanzamos el rayo y almacenamos la colisión del rayo con los objetos en la capa "Suelo".
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 0.05f, whatIsGround);

			// Devolvemos "true" si el rayo ha detectado suelo y "false" si no.
			return hit.collider is not null;
		}

		private void ChangeState (AdventurerState newState)
		{
			// Si le pasamos el estado en el que ya está el personaje, paramos la función.
			if (newState == currentState) return;

			// Almacenamos el nuevo estado.
			currentState = newState;

			// Dependiendo del nuevo estado, notificaremos al Trigger del Animator.
			switch (newState)
			{
				case AdventurerState.Idle:
					anim.SetTrigger("Idle");
					break;
				case AdventurerState.Running:
					anim.SetTrigger("Run");
					break;
				case AdventurerState.Jumping:
					anim.SetTrigger("Jump");
					break;
				case AdventurerState.Falling:
					anim.SetTrigger("Fall");
					break;
				case AdventurerState.Attacking:
					anim.SetTrigger("Attack");
					break;
			}
		}
	}
}
