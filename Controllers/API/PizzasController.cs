using la_mia_pizzeria_static.Database;
using la_mia_pizzeria_static.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace la_mia_pizzeria_static.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetPizzas(string? cercaPizza)
        {
            using (PizzaContext db = new PizzaContext())
            {
                if(cercaPizza == null)
                {
                    return BadRequest("Nessuna stringa di ricerca inserita");
                }

                Pizza? pizze = db.Pizze.Where(pizza => pizza.Nome.ToLower() == cercaPizza.ToLower()).FirstOrDefault();

                return Ok(pizze);
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetPizzaById(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizza = db.Pizze.Where(pizza => pizza.Id == id).FirstOrDefault();

                if(pizza != null)
                {
                    return Ok(pizza);
                }else
                {
                    return NotFound("Nessuna pizza trovata");
                }

            }

        }


        [HttpPost]
        public IActionResult CreatePizza([FromBody] Pizza nuovaPizza)
        {
            using (PizzaContext db = new PizzaContext())
            {
                try
                {
                    db.Add(nuovaPizza);
                    db.SaveChanges();

                    return Ok();

                }catch
                {
                    return BadRequest();
                }
            }

        }


        [HttpPut("{id}")]
        public IActionResult UpdatePizza(int id, [FromBody] Pizza pizzaDaModificare)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? modificaPizza = db.Pizze.Where(pizza => pizza.Id == id).FirstOrDefault();

                if(modificaPizza == null)
                {
                    return NotFound();
                }

                modificaPizza.Nome = pizzaDaModificare.Nome;
                modificaPizza.Prezzo = pizzaDaModificare.Prezzo;
                modificaPizza.Descrizione = pizzaDaModificare.Descrizione;
                modificaPizza.Image = pizzaDaModificare.Image;

                db.SaveChanges();
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePizza(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizzaDaCancellare = db.Pizze.Where(pizza => pizza.Id == id).FirstOrDefault();

                if (pizzaDaCancellare != null)
                {
                    db.Pizze.Remove(pizzaDaCancellare);
                    db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return NotFound("La pizza da eliminare non è stata trovata!");
                }
            }

        }


    }
}
