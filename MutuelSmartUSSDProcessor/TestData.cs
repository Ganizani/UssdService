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
using Newtonsoft.Json;
using exactmobile.ussdcommon.utility;

namespace exactmobile.ussdservice.processors
{
    public class TestData
    {
        public static Cities ListOfCities()
        {
            var cities = new Cities();
            try
            {
                using (var client = CommonUtility.GetHttpConnection("MutuelleSmartAPI"))
                {


                    var URL = new Uri(client.BaseAddress, "cities");
                    var response = client.GetAsync(URL.ToString()).Result;


                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        cities.status = 200;
                        List<City> cityList = new List<City>();
                        var result = response.Content.ReadAsStringAsync().Result;
                        try
                        {
                            JsonConvert.DeserializeObject<USSD.Entities.Cities>(result).cities.ForEach(n => cityList.Add(new City { city_id = n.city_id, city_name = n.city_name, province_id = n.province_id }));
                            cities.cities = cityList;
                        }
                        catch
                        {

                        }
                    }
                }

            }
            catch (Exception exp)
            {
                return new Cities
                {
                    status = 200,
                    cities = new List<City>
                {
                     new City {city_id=1, city_name="Kolwezi",province_id=2  } ,
                     new City {city_id=2, city_name="Likasi",province_id=2  } ,
                     new City {city_id=3, city_name="Lubumbashi",province_id=2  },
                     new City {city_id=4, city_name="Bukama",province_id=2  } ,
                     new City {city_id=5, city_name="Dilolo",province_id=2  } ,
                     new City {city_id=6, city_name="Fungurume",province_id=2 },
                     new City {city_id=7, city_name="Kabalo",province_id=2  } ,
                     new City {city_id=8, city_name="Kabongo",province_id=9  },
                     new City {city_id=9, city_name="Moba",province_id=9  } ,
                     new City {city_id=10, city_name="Kalemie",province_id=9  },
                     new City {city_id=11, city_name="Kambove",province_id=9  } ,
                     new City {city_id=12, city_name="Kamina",province_id=9  } ,
                     new City {city_id=13, city_name="Beni",province_id=7  } ,
                     new City {city_id=14, city_name="Butembo",province_id=7   },
                     new City {city_id=15, city_name="Goma",province_id=7  } ,
                     new City {city_id=16, city_name="Birambizo",province_id=7   },
                     new City {city_id=17, city_name="Masisi",province_id=8  } ,
                     new City {city_id=18, city_name="Mbandaka",province_id=1   },
                     new City {city_id=19, city_name="Kananga",province_id=3   },
                     new City {city_id=20, city_name="Mbuji-Mayi",province_id=4   },
                     new City {city_id=21, city_name="Kinshasa",province_id=5   },
                     new City {city_id=22, city_name="Matadi",province_id=6   },


                }

                };


            }

            return cities;

        }
        public static Communes ListOfCommunes()
        {

            Communes communes = new Communes();
            List<Commune> coms = new List<Commune>();
            try
            {
                using (var client = CommonUtility.GetHttpConnection("MutuelleSmartAPI"))
                {


                    var URL = new Uri(client.BaseAddress, "communes");
                    var response = client.GetAsync(URL.ToString()).Result;


                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        communes.status = 200;
                        var result = response.Content.ReadAsStringAsync().Result;
                        try
                        {
                            JsonConvert.DeserializeObject<USSD.Entities.Communes>(result).communes.ForEach(n => coms.Add(new Commune { city_id = n.city_id, commune_name = n.commune_name, commune_id = n.commune_id }));
                        }
                        catch
                        {

                        }
                    }
                }

            }
            catch (Exception exp)
            {
                communes.status = 500;

                coms.Add(new Commune { city_id = 19, commune_id = 1, commune_name = "Kananga" });
                coms.Add(new Commune { city_id = 19, commune_id = 2, commune_name = "Katoka" });
                coms.Add(new Commune { city_id = 20, commune_id = 3, commune_name = "Bipemba" });
                coms.Add(new Commune { city_id = 20, commune_id = 4, commune_name = "Dibindi" });

                coms.Add(new Commune { city_id = 21, commune_id = 5, commune_name = "Barumbu" });
                coms.Add(new Commune { city_id = 21, commune_id = 6, commune_name = "Gombe" });
                coms.Add(new Commune { city_id = 21, commune_id = 7, commune_name = "Masina" });
                coms.Add(new Commune { city_id = 21, commune_id = 8, commune_name = "Matete" });

                coms.Add(new Commune { city_id = 3, commune_id = 9, commune_name = "commune Annexe" });
                coms.Add(new Commune { city_id = 3, commune_id = 10, commune_name = "Kamalondo" });
                coms.Add(new Commune { city_id = 3, commune_id = 11, commune_name = "Lubumbashi" });
                coms.Add(new Commune { city_id = 21, commune_id = 12, commune_name = "Matadi" });
                coms.Add(new Commune { city_id = 21, commune_id = 13, commune_name = "Nzanza" });
                coms.Add(new Commune { city_id = 21, commune_id = 14, commune_name = "Mvuzi" });
                coms.Add(new Commune { city_id = 14, commune_id = 15, commune_name = "Kitotolo" });

                coms.Add(new Commune { city_id = 18, commune_id = 16, commune_name = "Mbandaka" });
                coms.Add(new Commune { city_id = 18, commune_id = 17, commune_name = "Wangata" });

                coms.Add(new Commune { city_id = 13, commune_id = 18, commune_name = "Beni" });
                coms.Add(new Commune { city_id = 13, commune_id = 19, commune_name = "Oicha" });
                coms.Add(new Commune { city_id = 15, commune_id = 20, commune_name = "Goma" });
                coms.Add(new Commune { city_id = 16, commune_id = 21, commune_name = "Birambizo" });
                coms.Add(new Commune { city_id = 17, commune_id = 22, commune_name = "Bagira" });
                coms.Add(new Commune { city_id = 17, commune_id = 23, commune_name = "Ibanda" });
                coms.Add(new Commune { city_id = 17, commune_id = 24, commune_name = "Kadutu" });

                coms.Add(new Commune { city_id = 1, commune_id = 25, commune_name = "Kolwezi" });
                coms.Add(new Commune { city_id = 2, commune_id = 26, commune_name = "Likasi" });
                coms.Add(new Commune { city_id = 3, commune_id = 27, commune_name = "Katuba" });
                coms.Add(new Commune { city_id = 4, commune_id = 28, commune_name = "Bukama" });
                coms.Add(new Commune { city_id = 5, commune_id = 29, commune_name = "Dilolo" });
                coms.Add(new Commune { city_id = 6, commune_id = 30, commune_name = "Fungurume" });
                coms.Add(new Commune { city_id = 7, commune_id = 31, commune_name = "Kabalo" });
            }
            communes.communes = coms;

            return communes;
        }

        public static Mutuels ListOfMutuels()
        {
            Mutuels mutuels = new Mutuels();
            List<Mutuel> muts = new List<Mutuel>();

            try
            {
                using (var client = CommonUtility.GetHttpConnection("MutuelleSmartAPI"))
                {


                    var URL = new Uri(client.BaseAddress, "Mutuels");
                    var response = client.GetAsync(URL.ToString()).Result;


                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        mutuels.status = 200;
                        var result = response.Content.ReadAsStringAsync().Result;
                        try
                        {
                            JsonConvert.DeserializeObject<USSD.Entities.Mutuels>(result).mutuels.ForEach(n => muts.Add(new Mutuel { mutuel_id = n.mutuel_id, mutuel_name = n.mutuel_name, commune_id = n.commune_id, price = n.price , address_id = n.address_id }));
                        }
                        catch
                        {

                        }
                    }
                }

            }
            catch (Exception exp)
            {
                mutuels.status = 200;

                muts.Add(new Mutuel { mutuel_id = 1, commune_id = 1, mutuel_name = "Kananga Mutuel", price = 2, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 2, commune_id = 1, mutuel_name = "Bakwetu Mutuel", price = 1, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 3, commune_id = 2, mutuel_name = "Bakwabo Mutuel", price = 1, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 4, commune_id = 4, mutuel_name = "Dawa Mutuel", price = 0.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 5, commune_id = 4, mutuel_name = "Dibindi Mutuel", price = 2, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 6, commune_id = 3, mutuel_name = "Bipemba Mutuel", price = 1 });
                muts.Add(new Mutuel { mutuel_id = 7, commune_id = 5, mutuel_name = "Mutombo Mutuel", price = 2, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 8, commune_id = 5, mutuel_name = "Mavungu Mutuel", price = 1, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 9, commune_id = 10, mutuel_name = "Dimakewtu Mutuel", price = 0.3M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 10, commune_id = 10, mutuel_name = "Nzee Mutuel", price = 0.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 11, commune_id = 10, mutuel_name = "Sobongo Mutuel", price = 2 });
                muts.Add(new Mutuel { mutuel_id = 12, commune_id = 10, mutuel_name = "Lukushi Mutuel", price = 1 });
                muts.Add(new Mutuel { mutuel_id = 13, commune_id = 11, mutuel_name = "Gecamine Mutuel", price = 2 });
                muts.Add(new Mutuel { mutuel_id = 14, commune_id = 11, mutuel_name = "Sedwe Mutuel", price = 1, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 15, commune_id = 11, mutuel_name = "SNCC Mutuel", price = 0.99M });
                muts.Add(new Mutuel { mutuel_id = 16, commune_id = 16, mutuel_name = "Mahiza Mutuel", price = 0.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 17, commune_id = 16, mutuel_name = "Basankusu Mutuel", price = 2.0M });
                muts.Add(new Mutuel { mutuel_id = 18, commune_id = 17, mutuel_name = "Kitoko Mutuel", price = 0.99M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 19, commune_id = 18, mutuel_name = "Mahiza Mutuel", price = 0.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 20, commune_id = 18, mutuel_name = "Basankusu Mutuel", price = 1.0M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 21, commune_id = 19, mutuel_name = "Kitoko Mutuel", price = 0.99M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 22, commune_id = 6, mutuel_name = "Kitoko Mutuel", price = 1.99M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 23, commune_id = 6, mutuel_name = "Mahiza Mutuel", price = 1.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 24, commune_id = 7, mutuel_name = "Basankusu Mutuel", price = 1.0M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 25, commune_id = 8, mutuel_name = "Kitoko Mutuel", price = 0.99M });
                muts.Add(new Mutuel { mutuel_id = 26, commune_id = 12, mutuel_name = "Mahiza Mutuel", price = 0.5M });
                muts.Add(new Mutuel { mutuel_id = 27, commune_id = 13, mutuel_name = "Basankusu Mutuel", price = 1.0M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 28, commune_id = 14, mutuel_name = "Kitoko Mutuel", price = 1.99M, address_id = 2 });

                muts.Add(new Mutuel { mutuel_id = 29, commune_id = 18, mutuel_name = "Kitoko Mutuel", price = 1.99M });
                muts.Add(new Mutuel { mutuel_id = 30, commune_id = 19, mutuel_name = "Mahiza Mutuel", price = 1.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 31, commune_id = 20, mutuel_name = "Basankusu Mutuel", price = 1.0M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 32, commune_id = 21, mutuel_name = "Kitoko Mutuel", price = 1.99M });
                muts.Add(new Mutuel { mutuel_id = 33, commune_id = 22, mutuel_name = "Mahiza Mutuel", price = 1.5M });
                muts.Add(new Mutuel { mutuel_id = 34, commune_id = 23, mutuel_name = "Basankusu Mutuel", price = 2.0M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 35, commune_id = 24, mutuel_name = "Kitoko Mutuel", price = 1.99M, address_id = 2 });

                muts.Add(new Mutuel { mutuel_id = 36, commune_id = 25, mutuel_name = "Kitoko Mutuel", price = 0.99M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 37, commune_id = 26, mutuel_name = "Mahiza Mutuel", price = 0.5M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 38, commune_id = 27, mutuel_name = "Basankusu Mutuel", price = 2.0M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 39, commune_id = 28, mutuel_name = "Kitoko Mutuel", price = 0.99M, address_id = 2 });
                muts.Add(new Mutuel { mutuel_id = 40, commune_id = 29, mutuel_name = "Mahiza Mutuel", price = 0.5M });
                muts.Add(new Mutuel { mutuel_id = 41, commune_id = 30, mutuel_name = "Basankusu Mutuel", price = 2.0M });
                muts.Add(new Mutuel { mutuel_id = 42, commune_id = 31, mutuel_name = "Kitoko Mutuel", price = 1.99M, address_id = 2 });
            }
            mutuels.mutuels = muts;

            return mutuels;
        }

    }
}
