using UnityEngine;

namespace MelenitasDev
{
	public class AdventurerController : MonoBehaviour
	{
		// ----- Serialized Fields
		[Header("References")]
		[SerializeField] private Transform groundCheck;
		[SerializeField] private LayerMask whatIsGround;
		
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
			Falling
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

		// ----- Private Methods
		private void HandleMovement ()
		{
			// Capturamos las teclas ("A" y "D") y ("←" y "→") para conocer la dirección.
			xInput = Input.GetAxisRaw("Horizontal");

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
			}
		}
	}
}
