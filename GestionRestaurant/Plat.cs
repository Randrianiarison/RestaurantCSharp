using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionRestaurant
{
    public class Plat
    {
        public int ID_plat { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public decimal Prix { get; set; }
        public string Categorie { get; set; }
        public int Nombre { get; set; }
        public byte[] Image { get; set; }

        public Plat()
        {

        }
        public Plat(int id, string nom, string description, decimal prix, string categorie, int nombre, byte[] image)
        {
            ID_plat = id;
            Nom = nom;
            Description = description;
            Prix = prix;
            Categorie = categorie;
            Nombre = nombre;
            Image = image;
        }
    }

}
