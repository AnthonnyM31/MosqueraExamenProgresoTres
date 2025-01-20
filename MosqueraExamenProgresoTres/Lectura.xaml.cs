using Microsoft.Maui.Controls;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MosqueraExamenProgresoTres;

public partial class Lectura : ContentPage
{
    private SQLiteAsyncConnection _db;

    public Lectura()
    {
        InitializeComponent();
        InicializarBaseDeDatos();
        CargarPaisesConsultados();
    }

  
    //SE CREA LA BASE DE DATOS
    private async void InicializarBaseDeDatos()
    {
        string rutaBD = Path.Combine(FileSystem.AppDataDirectory, "paises.db3");
        _db = new SQLiteAsyncConnection(rutaBD);
        await _db.CreateTableAsync<Pais>();
    }

    // se cargan los paises desde la base de datos, no la api
    private async void CargarPaisesConsultados()
    {
        var paises = await _db.Table<Pais>().ToListAsync();

        // los paises tendian a aparecer duplicados, estos lo soluciona filtrandolos por el nombre oficial
        var paisesUnicos = paises.GroupBy(p => p.NombreOficial)
                                 .Select(g => g.First())
                                 .ToList();

        // no se les puede imprimir tradicionalmente, hay que convertirlos a un formato que el listview acepte
        var paisesFormateados = paisesUnicos.Select(p => new
        {
            NombreYRegion = $"Nombre País: {p.NombreOficial}, Región: {p.Region}, Link: {p.LinkGoogleMaps}",
            NombreBD = $"NombreBD: {p.NombreOficial}"
        }).ToList();

       
        listadoPaises.ItemsSource = paisesFormateados;
    }


    private void RefrescarLista_Clicked(object sender, EventArgs e)
    {
        CargarPaisesConsultados();
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
          
        }
    }
}
