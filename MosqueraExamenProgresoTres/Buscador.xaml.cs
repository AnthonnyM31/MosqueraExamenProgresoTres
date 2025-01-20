using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using SQLite;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace MosqueraExamenProgresoTres;

public partial class Buscador : TabbedPage
{
    private SQLiteAsyncConnection _db;

    public Buscador()
    {
        InitializeComponent();
        InicializarBaseDeDatos();
    }

    private async void InicializarBaseDeDatos()
    {
        string rutaBD = Path.Combine(FileSystem.AppDataDirectory, "paises.db3");
        _db = new SQLiteAsyncConnection(rutaBD);
        await _db.CreateTableAsync<Pais>();
    }

    private async void BuscarPais_Clicked(object sender, EventArgs e)
    {
        string nombrePais = entradaPais.Text;

        if (string.IsNullOrWhiteSpace(nombrePais))
        {
            await DisplayAlert("Error", "Por favor ingresa un nombre válido.", "OK");
            return;
        }

        try
        {
            using HttpClient cliente = new HttpClient();
            string url = $"https://restcountries.com/v3.1/name/{nombrePais}?fields=name,region,maps";
            string respuesta = await cliente.GetStringAsync(url);

            var paises = JsonConvert.DeserializeObject<List<dynamic>>(respuesta);
            var primerPais = paises?.FirstOrDefault();

            if (primerPais != null)
            {
                string nombreOficial = primerPais.name.official;
                string region = primerPais.region;
                string linkGoogleMaps = primerPais.maps.googleMaps;

                resultadoBusqueda.Text = $"Nombre: {nombreOficial}\nRegión: {region}\nGoogle Maps: {linkGoogleMaps}";

                // Guardar en SQLite
                await _db.InsertAsync(new Pais
                {
                    NombreOficial = nombreOficial,
                    Region = region,
                    LinkGoogleMaps = linkGoogleMaps
                });
            }
            else
            {
                await DisplayAlert("Error", "No se encontró ningún país con ese nombre.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private void LimpiarEntrada_Clicked(object sender, EventArgs e)
    {
        entradaPais.Text = string.Empty;
        resultadoBusqueda.Text = string.Empty;
    }
}

public class Pais
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string NombreOficial { get; set; }
    public string Region { get; set; }
    public string LinkGoogleMaps { get; set; }
}
