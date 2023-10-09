﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace la_mia_pizzeria_static.Models
{
    public class PizzaFormModel
    {

        public Pizza Pizza { get; set; }
        public List<Categoria>? Categorie { get; set; }


        public List<SelectListItem>? Ingredienti { get; set; }
        public List<string>? SelectedIngredientiId { get; set; }

      
    }
}
