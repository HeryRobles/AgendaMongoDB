﻿using AgendaMongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

//Conectar a la base de datos
MongoClient client = new MongoClient("mongodb://localhost:27017");
IMongoDatabase database = client.GetDatabase("agenda");
//Coleccion = tabla
IMongoCollection<Contacto> collection = database.GetCollection<Contacto>("contactos");

-
string directorio = $"{Environment.CurrentDirectory}\\Data";
try
{
    if (!Directory.Exists(directorio))
    {
        throw new Exception($"El directorio {directorio} no existe");
    }

    string[] archivos = Directory
        .GetFiles(directorio)
        .AsEnumerable()
        .Where(a => a.EndsWith(".csv") || a.EndsWith(".json")) 
        .ToArray();

    foreach (var archivo in archivos)
    {
        //Console.WriteLine(archivo); 
        InsertarJson(archivo);
        InsertarCsv(archivo);
    }

    RealizarConsultas(collection);
    ActualizarRegistros(collection);
    EliminarRegistros(collection);
}
catch (Exception ex)
{

    Console.WriteLine(ex.Message);

}

List<Contacto> InsertarCsv(string archivo)
{
    List<Contacto> contactos = new List<Contacto>();
    if (Path.GetExtension(archivo) == ".csv")
    {
        string[] csv = File.ReadAllLines(archivo);

        foreach (string renglon in csv)
        {
            if (!renglon.Contains("Name,DateBirth") && !string.IsNullOrWhiteSpace(renglon))
            {
                //Expresion Regular!
                string[] datos = Regex.Split(renglon, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                Contacto contacto = new Contacto();
                contacto.Name = datos[0].Corregir();
                contacto.DateBirth = Convert.ToDateTime(datos[1].Corregir());
                contacto.Country = datos[2].Corregir();
                contacto.State = datos[3].Corregir();
                contacto.Address = datos[4].Corregir();
                contacto.Email = datos[5].Corregir();
                contacto.Phone = datos[6].Corregir();
                contactos.Add(contacto);
            }
        }
        collection.InsertMany(contactos);
    }

    Console.WriteLine("Proceso Terminado");
    return new List<Contacto>();
}

List<Contacto> InsertarJson(string archivo)
{
    List<Contacto> contactos = new List<Contacto>();
    if (Path.GetExtension(archivo) == ".json")
    {
        string json = File.ReadAllText(archivo);
        contactos = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonData>(json)!.objects;
        collection.InsertMany(contactos);
    }

    Console.WriteLine("Proceso Terminado");
    return new List<Contacto>();
}
static void RealizarConsultas(IMongoCollection<Contacto> collection)
{
    //Obtener todos los contactos en orden ascendente
    var consulta1 = collection.Find(new BsonDocument())
        .Sort(Builders<Contacto>.Sort.Ascending("Name")).ToList();

    //Obtener los contactos de Mexico
    var consulta2 = collection.Find(Builders<Contacto>
        .Filter.Eq("country", "Mexico"))
        .ToList();

    var consulta3 = collection.Find(Builders<Contacto>
        .Filter.Gt("datebirth", new DateTime(1990,1,1)))
        .ToList();

    var consulta4 = collection.Find(Builders<Contacto>
        .Filter.Regex("email", new BsonRegularExpression(".+@.+\'hotmail.com'+")))
        .ToList();

    List<Contacto> consulta5 = collection.Find(c3 => c3.Phone.Contains("123"))
        .ToList();

    //Obtener todos los contactos en orden ascendente
    //List<Contacto> consulta1 = collection.Find(c1 => c1.Name =="name")
    //    .SortBy(c1 => c1.Country)
    //    .ToList();
}

static void ActualizarRegistros(IMongoCollection<Contacto> collection)
{
    //Actualizar el nombre de los contactos con nombre: Melanie
    var filter = Builders<Contacto>.Filter.Eq("name", "Melanie");
    var update = Builders<Contacto>.Update.Set("name", "Wanda");
    collection.UpdateMany(filter, update);
}

static void EliminarRegistros(IMongoCollection<Contacto> collection)
{
    //Eliminar los registros de los que sean Peruanos xd
    var deleteFilter = Builders<Contacto>.Filter.Eq("country", "Peru");
    collection.DeleteMany(deleteFilter);
}











//Contacto contacto= new Contacto();
//contacto.Name = "Putter";
//contacto.Address = "9 3/4";
//contacto.Phone= "1234567890";

////Insert(contacto);


//Contacto contacto2 = new Contacto();
//contacto2.Name = "Sasuke";
//contacto2.Address = "Konoha";
//contacto2.Phone= "1234567890";

////Insert(contacto2);

////Contacto contactoEncontrado = FindByName("Putter Virginio")!;
////Console.WriteLine(contactoEncontrado!.ToString());


//Contacto contactoActualizado = new Contacto()
//{
//    Id = contactoEncontrado.Id,
//    Name = "Putter Virginio",
//    Phone = "234567890",
//};

//Update(contactoEncontrado.Id, contactoActualizado);
//contactoEncontrado = FindById(contactoEncontrado.Id);
//Console.WriteLine(contactoEncontrado.ToString());

//Delete(contactoEncontrado.Id);


//Console.WriteLine( );


//foreach (Contacto c in Get())
//{
//    Console.WriteLine(c.ToString());
//}

//void Insert(Contacto contacto)
//{
//    collection.InsertOne(contacto);
//}

//Contacto FindByName(string name)
//{
//    return collection.Find(c => c.Name == name)
//        .FirstOrDefault<Contacto>();
//}

//Contacto FindById(string id)
//{
//    return collection.Find(c => c.Id == id)
//        .FirstOrDefault<Contacto>();
//}

//List<Contacto> Get()
//{
//    return collection.Find(c => true).ToList();
//}

//void Update(string id, Contacto contacto)
//{
//    Contacto ContactoActualizar = FindById(id);
//    if (ContactoActualizar != null)
//    {
//        collection.ReplaceOne(c => c.Id == ContactoActualizar.Id, contacto);
//    }
//}

//void Delete(string id)
//{
//    Contacto contacto = FindById(id);
//    if (contacto != null)
//    {
//        collection.DeleteOne(c => c.Id == id);
//    }
//}


