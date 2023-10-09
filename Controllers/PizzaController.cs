using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using la_mia_pizzeria_static.Models;
using la_mia_pizzeria_static.Database;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Azure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace la_mia_pizzeria_static.Controllers
{
    [Authorize(Roles = "ADMIN,USER")]
    public class PizzaController : Controller
    {
        private CustomLogger _myLogger;
        private object db;

        public string SelectedIngredientiId { get; private set; }

        public PizzaController()
        {
            _myLogger = new CustomLogger();
        }

        public IActionResult Index()
        {
            _myLogger.WriteLog("L'utente è sulla pagina index");
            using PizzaContext db = new PizzaContext();

            List<Pizza> pizze = db.Pizze.ToList<Pizza>();

                return View("Index", pizze);
        }


        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {
            using (PizzaContext db = new PizzaContext())
            {
                List<Categoria> categorie = db.Categorie.ToList();
                List<SelectListItem> listaIngredienti = new List<SelectListItem>();
                List<Ingrediente> ingredienti = db.Ingredienti.ToList();

                foreach(Ingrediente ingrediente in ingredienti)
                {
                    listaIngredienti.Add(
                        new SelectListItem
                        {
                            Text = ingrediente.Ingredienti ,
                            Value = ingrediente.Id.ToString()
                        });
                }

                PizzaFormModel model = new PizzaFormModel();
                model.Pizza = new Pizza();
                model.Categorie = categorie;
                model.Ingredienti = listaIngredienti;

                return View("Create", model); 
            }
        }


        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PizzaFormModel data)
        {
            if (!ModelState.IsValid)
            {
                using (PizzaContext db = new PizzaContext())
                {
                    List<Categoria> categorie = db.Categorie.ToList();
                    data.Categorie = categorie;

                    List<SelectListItem> listaIngredienti = new List<SelectListItem>();
                    List<Ingrediente> databaseListaIngredienti = db.Ingredienti.ToList();

                    foreach (Ingrediente ingrediente in databaseListaIngredienti)
                    {
                        listaIngredienti.Add(
                            new SelectListItem
                            {
                                Text = ingrediente.Ingredienti,
                                Value = ingrediente.Id.ToString()
                            });
                    }

                    data.Ingredienti = listaIngredienti;
                    return View("Create", data);
                }
            }

            data.Pizza.Ingredienti = new List<Ingrediente>();

            using (PizzaContext db = new PizzaContext())
            {
                if (data.SelectedIngredientiId != null)
                {
                    foreach (string IngredienteSelezionato in data.SelectedIngredientiId)
                    {
                        int intIngredienteSelezionato = int.Parse(IngredienteSelezionato);
                        Ingrediente? IngredienteDb = db.Ingredienti.Where(Ingrediente => Ingrediente.Id == intIngredienteSelezionato).FirstOrDefault();

                        data.Pizza.Ingredienti.Add(IngredienteDb);
                    }
                }

                db.Pizze.Add(data.Pizza);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "ADMIN")]
        [HttpGet] 
        public IActionResult Update(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizzaDaModificare = db.Pizze.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if (pizzaDaModificare == null)
                {
                    return NotFound("La pizza che vuoi modificare non è stata trovata");
                }
                else
                {
                    List<Categoria> categorie = db.Categorie.ToList();
                    List<Ingrediente> ingredienti = db.Ingredienti.ToList();
                    List<SelectListItem> listaIngredienti = new List<SelectListItem>();

                    foreach(Ingrediente ingrediente in ingredienti)
                    {
                        listaIngredienti.Add(
                            new SelectListItem
                            {
                                Text = ingrediente.Ingredienti,
                                Value = ingrediente.Id.ToString(),
                                Selected = pizzaDaModificare.Ingredienti.Any(categoriaAssociata => categoriaAssociata.Id == ingrediente.Id)      
                            });
                    }

                    PizzaFormModel model = new PizzaFormModel { Pizza = pizzaDaModificare, Categorie = categorie, Ingredienti = listaIngredienti };

                    return View("Update", model);
                }
            }
        }


        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, PizzaFormModel data)
        {
            if(!ModelState.IsValid)
            {
                using (PizzaContext db = new PizzaContext())
                {
                    List<Categoria> categorie = db.Categorie.ToList();
                    data.Categorie = categorie;
                    List<Ingrediente> dbIngredienti = db.Ingredienti.ToList();
                    List<SelectListItem> ingredientiSelezionati = new List<SelectListItem>();

                    foreach(Ingrediente ingrediente in dbIngredienti)
                    {
                        ingredientiSelezionati.Add(
                            new SelectListItem
                            {
                                Text = ingrediente.Ingredienti,
                                Value = ingrediente.Id.ToString(),
                            });
                    }
                    return View("Update", data);
                }
            }

            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizzaDaModificare = db.Pizze.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if(pizzaDaModificare != null)
                {
                    pizzaDaModificare.Nome = data.Pizza.Nome;
                    pizzaDaModificare.Descrizione = data.Pizza.Descrizione;
                    pizzaDaModificare.Image = data.Pizza.Image;
                    pizzaDaModificare.Prezzo = data.Pizza.Prezzo;
                    pizzaDaModificare.CategoriaId = data.Pizza.CategoriaId;

                    if (data.SelectedIngredientiId != null)
                    {
                        pizzaDaModificare.Ingredienti.Clear();

                        foreach (string IngredientiId in data.SelectedIngredientiId)
                        {
                            int intIngredienteSelezionato = int.Parse(IngredientiId);

                            Ingrediente? ingredienteInDb = db.Ingredienti.Where(ingrediente => ingrediente.Id == intIngredienteSelezionato).FirstOrDefault();

                            if (ingredienteInDb != null)
                            {
                                pizzaDaModificare.Ingredienti.Add(ingredienteInDb);
                            }
                        }
                    }

                    db.SaveChanges();

                    return RedirectToAction("Index");

                }else
                {
                    return NotFound("Non è stata trovata la pizza da modificare");
                }

            }

        }


        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizzaDaCancellare = db.Pizze.Where(pizza => pizza.Id == id).FirstOrDefault();

                if (pizzaDaCancellare != null)
                {
                    db.Pizze.Remove(pizzaDaCancellare);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound("La pizza da eliminare non è stata trovata!");
                }
            }
        }

        public IActionResult DettaglioPizza(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? dettaglioPizza = db.Pizze.Where(pizze => pizze.Id == id).Include(pizza => pizza.Catwgoria).Include(pizza => pizza.Ingredienti).FirstOrDefault();

                if (dettaglioPizza == null)
                {
                    return NotFound($"La pizza con id {id} non è stata trovata!");
                }
                else
                {
                    return View("DettaglioPizza", dettaglioPizza);
                }
            }
        }
    }




}
