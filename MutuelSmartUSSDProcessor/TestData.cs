using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.processors;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdcommon;
using exactmobile.ussdservice.common.menu;
using USSD.Entities;
namespace exactmobile.ussdservice.processors
{
    public class TestData
    {
        public static Cities ListOfCities()
        {
            return new Cities
            {
                status = 200,
                cities = new List<City> {

{ new City {city_id=1, city_name="Kolwezi",province_id=3  } },
{ new City {city_id=2, city_name="Likasi",province_id=3  } },
{ new City {city_id=3, city_name="Lubumbashi",province_id=3  } },
{ new City {city_id=4, city_name="Bukama",province_id=3  } },
{ new City {city_id=5, city_name="Dilolo",province_id=3  } },
{ new City {city_id=6, city_name="Fungurume",province_id=3  } },
{ new City {city_id=7, city_name="Kabalo",province_id=3  } },
{ new City {city_id=8, city_name="Kabongo",province_id=3  } },
{ new City {city_id=9, city_name="Moba",province_id=3  } },
{ new City {city_id=10, city_name="Kalemie",province_id=3  } },
{ new City {city_id=11, city_name="Kambove",province_id=3  } },
{ new City {city_id=12, city_name="Kamina",province_id=3  } },
{ new City {city_id=13, city_name="Kaniama",province_id=3  } },
{ new City {city_id=14, city_name="Kansenya",province_id=3  } },
{ new City {city_id=15, city_name="Kapanga",province_id=3  } },
{ new City {city_id=16, city_name="Kasenga",province_id=3  } },
{ new City {city_id=17, city_name="Kasumbalesa",province_id=3  } },
{ new City {city_id=18, city_name="Katuba",province_id=3  } },
{ new City {city_id=19, city_name="Kenya",province_id=3  } },
{ new City {city_id=20, city_name="Kamalondo",province_id=3  } },
{ new City {city_id=21, city_name="Kiambi",province_id=3  } },
{ new City {city_id=22, city_name="Kinkondja",province_id=3  } },
{ new City {city_id=23, city_name="Kipushi",province_id=3  } },
{ new City {city_id=24, city_name="Kilwa",province_id=3  } },
{ new City {city_id=25, city_name="Kongolo",province_id=3  } },
{ new City {city_id=26, city_name="Manono",province_id=3  } },
{ new City {city_id=27, city_name="Mokambo",province_id=3  } },
{ new City {city_id=28, city_name="Mulongo",province_id=3  } },
{ new City {city_id=29, city_name="Nyunzu",province_id=3  } },
{ new City {city_id=30, city_name="Panda",province_id=3  } },
{ new City {city_id=31, city_name="Pweto",province_id=3  } },
{ new City {city_id=32, city_name="Rwashi",province_id=3  } },
{ new City {city_id=33, city_name="Sakania",province_id=3  } },
{ new City {city_id=34, city_name="Shinkolobwe",province_id=3  } },
{ new City {city_id=35, city_name="Songa",province_id=3  } },
{ new City {city_id=36, city_name="Beni",province_id=19  } },
{ new City {city_id=37, city_name="Butembo",province_id=19  } },
{ new City {city_id=38, city_name="Goma",province_id=19  } },
{ new City {city_id=39, city_name="Birambizo",province_id=19  } },
{ new City {city_id=40, city_name="Katwa",province_id=19  } },
{ new City {city_id=41, city_name="Kayna",province_id=19  } },
{ new City {city_id=42, city_name="Kirotshe",province_id=19  } },
{ new City {city_id=43, city_name="Kyondo",province_id=19  } },
{ new City {city_id=44, city_name="Lubero",province_id=19  } },
{ new City {city_id=45, city_name="Mangina",province_id=19  } },
{ new City {city_id=46, city_name="Manguredjipa",province_id=19  } },
{ new City {city_id=47, city_name="Masisi",province_id=19  } },
{ new City {city_id=48, city_name="Musienene",province_id=19  } },
{ new City {city_id=49, city_name="Mutwanga",province_id=19  } },
{ new City {city_id=50, city_name="Mweso",province_id=19  } },
{ new City {city_id=51, city_name="Oicha",province_id=19  } },
{ new City {city_id=52, city_name="Pinga",province_id=19  } },
{ new City {city_id=53, city_name="Rutshuru",province_id=19  } },
{ new City {city_id=54, city_name="Rwanguba",province_id=19  } },
{ new City {city_id=55, city_name="Walikale",province_id=19  } }
                }

            };



        }
        public static Communes ListOfCommunes()
        {
            Communes communes = new Communes();
            List<Commune> coms = new List<Commune>();
            communes.status = 200;

            coms.Add(new Commune { city_id = 36, commune_id = 1, commune_name = "Beni" });
            coms.Add(new Commune { city_id = 36, commune_id = 2, commune_name = "Bungulu" });
            coms.Add(new Commune { city_id = 36, commune_id = 3, commune_name = "Ruwenzori" });
            coms.Add(new Commune { city_id = 36, commune_id = 4, commune_name = "Muhekera" });

            coms.Add(new Commune { city_id = 37, commune_id = 5, commune_name = "Bulengera" });
            coms.Add(new Commune { city_id = 37, commune_id = 6, commune_name = "Kimemi" });
            coms.Add(new Commune { city_id = 37, commune_id = 7, commune_name = "Mususa" });
            coms.Add(new Commune { city_id = 37, commune_id = 8, commune_name = "Vutamba" });

            coms.Add(new Commune { city_id = 12, commune_id = 9, commune_name = "Dimayi" });
            coms.Add(new Commune { city_id = 12, commune_id = 10, commune_name = "Kamina" });
            coms.Add(new Commune { city_id = 12, commune_id = 11, commune_name = "Sobongo" });
            coms.Add(new Commune { city_id = 26, commune_id = 12, commune_name = "Lukushi" });
            coms.Add(new Commune { city_id = 26, commune_id = 13, commune_name = "Kanteba" });
            coms.Add(new Commune { city_id = 26, commune_id = 14, commune_name = "Kaulu-Minono" });
            coms.Add(new Commune { city_id = 26, commune_id = 15, commune_name = "Kitotolo" });


            communes.communes = coms;

            return communes; 
        }

        public static Mutuels ListOfMutuels()
        {
            Mutuels mutuels = new Mutuels();
            List<Mutuel> coms = new List<Mutuel>();
            mutuels.status = 200;

            coms.Add(new Mutuel { mutuel_id = 1, commune_id = 4, mutuel_name = "Beni Mutuel", price = 4 });
            coms.Add(new Mutuel { mutuel_id = 2, commune_id = 4, mutuel_name = "Bungulu Mutuel", price = 7 });
            coms.Add(new Mutuel { mutuel_id = 3, commune_id = 4, mutuel_name = "Ruwenzori Mutuel", price = 12 });
            coms.Add(new Mutuel { mutuel_id = 4, commune_id = 4, mutuel_name = "Muhekera Mutuel", price = 20 });
            coms.Add(new Mutuel { mutuel_id = 5, commune_id = 8, mutuel_name = "Bulengera Mutuel", price = 4 });
            coms.Add(new Mutuel { mutuel_id = 6, commune_id = 8, mutuel_name = "Kimemi Mutuel", price = 11 });
            coms.Add(new Mutuel { mutuel_id = 7, commune_id = 8, mutuel_name = "Mususa Mutuel", price = 8 });
            coms.Add(new Mutuel { mutuel_id = 8, commune_id = 8, mutuel_name = "Vutamba Mutuel", price = 45 });
            coms.Add(new Mutuel { mutuel_id = 9, commune_id = 10, mutuel_name = "Dimayi Mutuel", price = 3 });
            coms.Add(new Mutuel { mutuel_id = 10, commune_id = 10, mutuel_name = "Kamina Mutuel", price = 15 });
            coms.Add(new Mutuel { mutuel_id = 11, commune_id = 10, mutuel_name = "Sobongo Mutuel", price = 20 });
            coms.Add(new Mutuel { mutuel_id = 12, commune_id = 10, mutuel_name = "Lukushi Mutuel", price = 4 });
            coms.Add(new Mutuel { mutuel_id = 13, commune_id = 10, mutuel_name = "Kanteba Mutuel", price = 22});
            coms.Add(new Mutuel { mutuel_id = 14, commune_id = 10, mutuel_name = "Kaulu-Minono Mutuel", price = 6 });
            coms.Add(new Mutuel { mutuel_id = 15, commune_id = 10, mutuel_name = "Kitotolo Mutuel", price = 9.99M });


            mutuels.mutuels = coms;

            return mutuels;
        }

    }
}
