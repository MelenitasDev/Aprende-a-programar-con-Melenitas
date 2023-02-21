using UnityEngine;

namespace MelenitasDev
{
    // Esta clase "Enemigo" implementa la interfaz "IAttackable", lo que le obliga a implementar
    // la función "Herir" y nos permite detectar desde fuera si se le puede atacar.
    public class Enemy : MonoBehaviour, IAttackable // Después de MonoBehaviour, implementamos la interfaz.
    {
        // ----- Fields
        private Animator anim;

        // ----- Unity Callbacks
        void Awake ()
        {
            // Referenciamos el animator del enemigo.
            anim = GetComponent<Animator>();
        }

        // ----- Interface Methods
        
        // Esta función es la que nos obliga a implementar la interfaz "IAttackable". Todas las clases atacables
        // deberán llevarla, y cada una controlará que pasa cuando se le llama.
        public void Hurt ()
        {
            // Ejecutamos la animación de muerte del enemigo.
            anim.SetTrigger("Die");
        }

        // ----- Private Methods
        
        // Esta función se ejecuta en el último frame de la animación de muerte.
        private void Hide ()
        {
            // Desactiva y oculta al enemigo.
            gameObject.SetActive(false);
        }
    }
}
