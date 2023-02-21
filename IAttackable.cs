
namespace MelenitasDev
{
    // Esta es la interfaz que hace que cualquier clase sea Atacable. Las funciones que declaremos aquí dentro,
    // tendrán que estar declaradas obligatoriamente en las clases que implementen esta interfaz.
    // Así nos aseguramos de que cualquier clase atacable tenga la funcionalidad "Herir" en común.
    
    // Nosotros la implementaremos en el enemigo, así cuando ejecutemos el ataque, si detectamos algo atacable,
    // llamaremos a su función "Hurt", que controlará lo que pasa cuando se le hiere.
    public interface IAttackable
    {
        // Declaramos la función que deberán llevar todas las clases atacables. En este caso, Herir.
        // No tenemos que meter código en su interior, solo decirle el nombre de la función.
        // El código que controlará lo que pasa al ser herido lo controlará la clase donde implementemos esta interfaz.
        void Hurt ();
    }
}
