namespace la_mia_pizzeria_static.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Ingredienti { get; set; }


        public Ingrediente() { }

        public List<Pizza> pizze { get; set; }
    }
}
