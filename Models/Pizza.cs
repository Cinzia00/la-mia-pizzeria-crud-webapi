using la_mia_pizzeria_static.Validazione;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace la_mia_pizzeria_static.Models
{
    public class Pizza
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Il nome della pizza è obbligatorio")]
        [MaxLength(100, ErrorMessage = "La lunghezza massima è di 100 caratteri")]
        public string Nome { get; set; }

        [Column(TypeName = "text")]
        [Required(ErrorMessage = "La descrizione della pizza è obbligatoria!")]
        [ValidazioneValidationAttribute]
        public string Descrizione { get; set; }

        [Url(ErrorMessage = "Devi inserire un link valido ad un'immagine")]
        [MaxLength(500, ErrorMessage = "Il link non può essere lungo più di 500 caratteri")]
        public string Image { get; set; }

        [Range(1, 100, ErrorMessage = "Il prezzo deve essere compreso tra 1€ e 100€")] 
        public int Prezzo { get; set; }


        public Pizza() { }

        public Pizza(string nome, string descrizione, string image, int prezzo)
        {
            Nome = nome;
            Descrizione = descrizione;
            Image = image;
            Prezzo = prezzo;
        }

        public int? CategoriaId { get; set; }
        public Categoria? Catwgoria { get; set; }

        public List<Ingrediente>? Ingredienti { get; set;}
    }
}
