using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "https://voyages-dhaussy.com/";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour modifier
        /// </summary>
        private const string PUT = "PUT";

        /// <summary>
        /// méthode HTTP pour supprimer
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre");
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon");
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public");
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre");
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les documents à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Document</returns>
        public List<Document> GetAllDocuments(string idDocument)
        {
            List<Document> lesDocuments = TraitementRecup<Document>(GET, "document/" + idDocument);
            return lesDocuments;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd");
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue");
            return lesRevues;
        }


        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + idDocument);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire/" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false; 
        }

        /// <summary>
        /// Ajout d'un nouveau document dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerDocument(string Id, string Titre, string Image, string IdRayon, string IdPublic, string IdGenre)
        {
            String jsonCreerDocument = "{ \"id\" : \""+Id+ "\", \"titre\" : \""+Titre+ "\", \"image\" : \""+Image+ "\", \"idRayon\" : \""+IdRayon+ "\", \"idPublic\" : \""+IdPublic+ "\", \"idGenre\" : \""+IdGenre+ "\"}";
            Console.WriteLine("jsonCreerDocument" + jsonCreerDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(POST, "document/" + jsonCreerDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modification d'un document dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool ModifierDocument(string Id, string Titre, string Image, string IdGenre, string IdPublic, string IdRayon)
        {
            String jsonModifierDocument = "{ \"id\" : \"" + Id + "\", \"titre\" : \"" + Titre + "\", \"image\" : \"" + Image + "\", \"idGenre\" : \"" + IdGenre + "\", \"idPublic\" : \"" + IdPublic + "\", \"idRayon\" : \"" + IdRayon + "\"}";
            Console.WriteLine("jsonModifierDocument" + jsonModifierDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(PUT, "document/" + Id + "/" + jsonModifierDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Suppression d'un document dans la base de données
        /// </summary>
        /// <param name="Id">Id du document à supprimer</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool SupprimerDocument(string Id)
        {
            String jsonIdDocument = "{ \"id\" : \"" + Id + "\"}";
            Console.WriteLine("jsonIdDocument" + jsonIdDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(DELETE, "document/" + jsonIdDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ajout d'un nouveau livre dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            String jsonCreerLivre = "{ \"id\" : \"" + Id + "\", \"isbn\" : \"" + Isbn + "\", \"auteur\" : \"" + Auteur + "\", \"collection\" : \"" + Collection + "\"}";
            Console.WriteLine("jsonCreerLivre" + jsonCreerLivre);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(POST, "livre/" + jsonCreerLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modification d'un livre dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool ModifierLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            String jsonModifierLivre = "{ \"id\" : \"" + Id + "\", \"isbn\" : \"" + Isbn + "\", \"auteur\" : \"" + Auteur + "\", \"collection\" : \"" + Collection + "\"}";
            Console.WriteLine("jsonModifierLivre" + jsonModifierLivre);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Livre> liste = TraitementRecup<Livre>(PUT, "livre/" + Id + "/" + jsonModifierLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        /// <summary>
        /// Suppression d'un livre dans la base de données
        /// </summary>
        /// <param name="Id">Id du livre à supprimer</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool SupprimerLivre(string Id)
        {
            String jsonIdLivre = "{ \"id\" : \""+Id+ "\"}";
            Console.WriteLine("jsonIdLivre" + jsonIdLivre);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Livre> liste = TraitementRecup<Livre>(DELETE, "livre/"+ jsonIdLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        /// <summary>
        /// Ajout d'un nouveau dvd dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            String jsonCreerDvd = "{ \"id\" : \"" + Id + "\", \"synopsis\" : \"" + Synopsis + "\", \"realisateur\" : \"" + Realisateur + "\", \"duree\" : \"" + Duree + "\"}";
            Console.WriteLine("jsonCreerDvd" + jsonCreerDvd);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Dvd> liste = TraitementRecup<Dvd>(POST, "dvd/" + jsonCreerDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modification d'un dvd dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool ModifierDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            String jsonModifierDvd = "{ \"id\" : \"" + Id + "\", \"synopsis\" : \"" + Synopsis + "\", \"realisateur\" : \"" + Realisateur + "\", \"duree\" : \"" + Duree + "\"}";
            Console.WriteLine("jsonModifierDvd" + jsonModifierDvd);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Dvd> liste = TraitementRecup<Dvd>(PUT, "dvd/" + Id + "/" + jsonModifierDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Suppression d'un dvd dans la base de données
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool SupprimerDvd(string Id)
        {
            String jsonIdDvd = "{ \"id\" : \"" + Id + "\"}";
            Console.WriteLine("jsonIdDvd" + jsonIdDvd);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Dvd> liste = TraitementRecup<Dvd>(DELETE, "dvd/" + jsonIdDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ajout d'une nouvelle revue dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            String jsonCreerRevue = "{ \"id\" : \"" + Id + "\", \"periodicite\" : \"" + Periodicite + "\", \"delaiMiseADispo\" : \"" + DelaiMiseADispo + "\"}";
            Console.WriteLine("jsonCreerRevue" + jsonCreerRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Revue> liste = TraitementRecup<Revue>(POST, "revue/" + jsonCreerRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modification d'une revue dans la base de données
        /// </summary>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool ModifierRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            String jsonModifierRevue = "{ \"id\" : \"" + Id + "\", \"periodicite\" : \"" + Periodicite + "\", \"delaiMiseADispo\" : \"" + DelaiMiseADispo + "\"}";
            Console.WriteLine("jsonModifierRevue" + jsonModifierRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Revue> liste = TraitementRecup<Revue>(PUT, "revue/" + Id + "/" + jsonModifierRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Suppression d'une revue dans la base de données
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool SupprimerRevue(string Id)
        {
            String jsonIdRevue = "{ \"id\" : \"" + Id + "\"}";
            Console.WriteLine("jsonIdRevue" + jsonIdRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Revue> liste = TraitementRecup<Revue>(DELETE, "revue/" + jsonIdRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}
